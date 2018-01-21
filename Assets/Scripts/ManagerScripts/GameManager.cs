using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager GetGameManager { get; private set; }
    private SoundManager _soundManager;
    private TurnManager _turnManager;
    [SerializeField]
    private ScoreHandler[] ScoreObject;
    [SerializeField]
    private GameObject NumberBox;
    [SerializeField]
    private Number[] PlayerOneNumbers;
    [SerializeField]
    private Number[] PlayerTwoNumbers;
    [SerializeField]
    private Vector2[] PlayerOnePositions, PlayerTwoPositions;
    public GameBoardHandler BoardHandler;
    public int Limit;
    public int LimitType;
    public bool scored, isOnline;
    private Transform flyPosition;

    private void Awake()
    {
        GetGameManager = this;
    }

    private void OnLevelWasLoaded(int level)
    {
        LimitType = (int)LevelManager.GetLevelManager.Type;
        Limit = LevelManager.GetLevelManager.Limit;
    }

    // Use this for initialization
    void Start()
    {
        _turnManager = TurnManager.GetTurnManager;
        _soundManager = SoundManager.GetSoundManager;
        scored = false;

        SetGame();
    }

    public void SetSecondNonPlayable()
    {
        foreach (var item in PlayerTwoNumbers)
        {
            item.TurnNumber = -1; //he cannot play these then
        }
    }

    public void RefillPower(int value, int target)
    {
        //_soundManager.PlayRefillScore();

        switch (LimitType)
        {
            case (int)LimitTypes.Time:
                _turnManager.AddTime(value * 2, target);
                break;
            case (int)LimitTypes.Score:
                ScoreObject[target].Score += value * 2;
                break;
            case (int)LimitTypes.Charge:
                ScoreObject[target].UpdateCharge(-value * 4);
                break;
            default:
                break;
        }
    }

    public void FinishGame()
    {
        CompareScores();
        BoardHandler.ClearTheBoard();
        StopAllCoroutines();
    }

    public void PrintMessageToTurnField(string message)
    {
        ScoreObject[_turnManager.TurnNumber - 1].PrintToTurnField(message);
    }

    public void DestroyedNumber(int number, int whose)
    {
        ScoreObject[whose].Scored(number);
        ScorePlayer(number + 1, whose);
        if (LimitType == (int)LimitTypes.Score)
            ControlForScore();
    }

    public void ReturnToMenu()
    {
        LevelManager.GetLevelManager.LoadLevelWithoutBanner(ProjectConstants.MainMenu);
        Destroy(LevelManager.GetLevelManager.gameObject);
    }

    public void SpawnCardFromPeer(int number, int pos_x, int pos_y)
    {
        var spawned = Instantiate(NumberBox, new Vector2(pos_x, pos_y), Quaternion.identity);
        spawned.transform.parent = BoardHandler.transform;
        spawned.transform.localPosition = new Vector2(pos_x, pos_y);
        spawned.GetComponent<Number>().MyNumber = number;
        spawned.GetComponent<Number>().TurnNumber = -10; //non playable
        spawned.GetComponent<Number>().PlaceTile();
        BoardHandler.GameBoardMatrix.row[pos_y + 2].column[pos_x + 2] = spawned.GetComponent<Number>();
        ControlMatrix(pos_x + 2, pos_y + 2, spawned.GetComponent<Number>().MyNumber);
    }

    public void SpawnCard(Number number, Vector2 pos, int TurnNumber)
    {
        var spawned = Instantiate(NumberBox, pos, Quaternion.identity);
        int newCardNumber = GetNewNumber(number.MyNumber);
        spawned.GetComponent<Number>().TurnNumber = TurnNumber;
        spawned.GetComponent<Number>().MyNumber = newCardNumber + 1;
        SetNumberIndex(number, spawned.GetComponent<Number>());
        if (LimitType == (int)LimitTypes.Charge && ScoreObject[TurnNumber - 1].Charge >= 0)
        {
            ScoreObject[TurnNumber - 1].UpdateCharge(newCardNumber + 1);
        }
    }

    private void SetNumberIndex(Number number, Number spawned)
    {
        if(number.TurnNumber == 1)
        {
            for (int i = 0; i < PlayerOneNumbers.Length; i++)
            {
                if(PlayerOneNumbers[i] == number)
                {
                    PlayerOneNumbers[i] = spawned;
                    //Send message for PlayerOneNumbers index i to be spawned with number and turnNumber
                    break;
                }
            }
        }
        else if(number.TurnNumber == 2)
        {
            for (int i = 0; i < PlayerTwoNumbers.Length; i++)
            {
                if (PlayerTwoNumbers[i] == number)
                {
                    PlayerTwoNumbers[i] = spawned;
                    //Send message for PlayerTwoNumbers index i to be spawned with number and turnNumber
                    break;
                }
            }
        }
    }

    public void UpdateMatrix(int pos_x, int pos_y, Number card)
    {
        if(isOnline)
        {
            string[] data = new string[]
            {
                ProjectConstants.dropAndSpawnNumber,
                pos_x.ToString(),
                pos_y.ToString(),
                card.MyNumber.ToString()
            };
            GPGController.SendByteMessage(GPGBytePackager.CreatePackage(data), 
                GPGController.GetOpponentId());
        }
        BoardHandler.GameBoardMatrix.row[pos_y].column[pos_x] = card;
        BoardHandler.GivePower(pos_x, pos_y, card);
        ControlMatrix(pos_x, pos_y, card.MyNumber);
    }

    private void SetGame()
    {
        if (LimitType == (int)LimitTypes.Charge)
        {
            ScoreObject[0].Charge = ScoreObject[1].Charge = Limit;
            ScoreObject[0].UpdateCharge(0);
            ScoreObject[1].UpdateCharge(0);
        }
        BoardHandler.SumTheBoard();
    }

    private int GetNewNumber(int number)
    {
        return (BoardHandler.SumTheBoard()) % 3;
    }

    private void UpdateText(int turn)
    {
        ScoreObject[turn].PrintScore();
        BoardHandler.SumTheBoard();
    }

    private void ControlForScore()
    {
        if (ScoreObject[0].Score >= Limit)
        {
            DecideWinner(ProjectConstants.PlayerOne);
            BoardHandler.ClearTheBoard();
        }
        else if (ScoreObject[1].Score >= Limit)
        {
            DecideWinner(ProjectConstants.PlayerTwo);
            BoardHandler.ClearTheBoard();
        }
    }

    private void DecideWinner(int winnerIndex)
    {
        StopAllCoroutines();
        if (winnerIndex > 1 || winnerIndex < 0)
        {
            ScoreObject[0].PrintToTurnField(ProjectConstants.TiedText);
            ScoreObject[1].PrintToTurnField(ProjectConstants.TiedText);
            //Send tied message
            _turnManager.ShowPanels();
            return;
        }

        ScoreObject[winnerIndex].PrintToTurnField(ProjectConstants.WinText);
        if (winnerIndex == 0)
        {
            ScoreObject[1].PrintToTurnField(ProjectConstants.LoseText);
            //Send YOU WON message
        }
        else if (winnerIndex == 1)
        {
            ScoreObject[0].PrintToTurnField(ProjectConstants.LoseText);
            //Send YOU LOST message
        }
        _turnManager.ShowPanels();
        _soundManager.StopLoop();
    }

    private void CompareScores()
    {
        if (LimitType == (int)LimitTypes.Charge)
        {
            ScoreObject[0].Score += ScoreObject[0].Charge;
            ScoreObject[1].Score += ScoreObject[1].Charge;
        }
        if (ScoreObject[0].Score > ScoreObject[1].Score)
        {
            DecideWinner(ProjectConstants.PlayerOne);
        }
        else if (ScoreObject[0].Score < ScoreObject[1].Score)
        {
            DecideWinner(ProjectConstants.PlayerTwo);
        }
        else
        {
            DecideWinner(ProjectConstants.Noone);
        }
    }

    private void ChangeTurn()
    {
        _turnManager.ChangeTurn();
        flyPosition = ScoreObject[_turnManager.TurnNumber - 1].Magnet;
        BoardHandler.flyPosition = flyPosition;
    }

    private void ControlMatrix(int column, int row, int cardNumber)
    {
        Number number = BoardHandler.GameBoardMatrix.row[row].column[column];
        scored = BoardHandler.ControlMatrix(column, row, cardNumber, number);
        if (scored)
            number.Destroy(row, column, _turnManager.TurnNumber, flyPosition);
        scored = false;
        if (LimitType == (int)LimitTypes.Score)
            ControlForScore();
        if (BoardHandler.CheckBoardFull())
        {
            CompareScores();
            BoardHandler.ClearTheBoard();
        }
        else
        {
            ChangeTurn();
        }
    }

    private void ScorePlayer(int score, int turn)
    {
        _soundManager.PlayScore();
        UpdateText(turn);
    }
}
