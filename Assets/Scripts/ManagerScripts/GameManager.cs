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
    private GameObject[] PlayerOneDoors;
    [SerializeField]
    private GameObject[] PlayerTwoDoors;
    [SerializeField]
    private Vector2[] PlayerOnePositions, PlayerTwoPositions;
    [SerializeField]
    private Sprite[] Numbers;
    [SerializeField]
    private GameObject[] MagnetizeRows;
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

    public void MagnetizeRow(int index)
    {
        MagnetizeRows[index].GetComponent<Animator>().SetTrigger("magnetize");
    }

    public void UpdateScoreHandlerCharge(int charge, int turn)
    {
        ScoreObject[turn].UpdateInternalCharge(ScoreObject[turn].Charge - charge);
    }

    public void SetSecondNonPlayable()
    {
        foreach (var item in PlayerTwoNumbers)
        {
            item.TurnNumber = -1; //he cannot play these then
        }
    }

    public void SetFirstNonPlayable()
    {
        foreach (var item in PlayerOneNumbers)
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
        spawned.GetComponent<Number>().PlaceTile();
        spawned.GetComponent<Number>().TurnNumber = -10; //non playable
        spawned.GetComponent<Number>().SkipSpawn(number);
        BoardHandler.GameBoardMatrix.row[pos_y + 2].column[pos_x + 2] = spawned.GetComponent<Number>();
        ControlMatrix(pos_x + 2, pos_y + 2, spawned.GetComponent<Number>().MyNumber, true);
        BoardHandler.GivePower(pos_x + 2, pos_y + 2, spawned.GetComponent<Number>());
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

    public void AnimateDoor(int turn, int index)
    {
        switch (turn)
        {
            case 1:
                PlayerOneDoors[index].GetComponent<Animator>().SetTrigger("start");
                break;
            case 2:
                PlayerTwoDoors[index].GetComponent<Animator>().SetTrigger("start");
                break;
            default:
                break;
        }
    }

    public void UpdateOnlineOpponentCard(int index, int number)
    {
        PlayerTwoNumbers[index].GetComponent<Animator>().runtimeAnimatorController =
            Resources.Load("NumberBox" + number) as RuntimeAnimatorController;
        if(PlayerTwoNumbers[index].MyNumber == number)
            PlayerTwoDoors[index].GetComponent<Animator>().SetTrigger("restart");
        PlayerTwoDoors[index].GetComponent<Animator>().SetTrigger("start");
    }

    private void SetNumberIndex(Number number, Number spawned)
    {
        if (number.TurnNumber == 1)
        {
            for (int i = 0; i < PlayerOneNumbers.Length; i++)
            {
                if (PlayerOneNumbers[i] == number)
                {
                    PlayerOneNumbers[i] = spawned;
                    AnimateDoor(1, i);
                    if (isOnline)
                    {
                        string[] data = new string[]
                        {
                            ProjectConstants.message_spawnTile,
                            spawned.MyNumber.ToString(),
                            i.ToString()
                        };
                        GPGController.SendByteMessage(GPGBytePackager.CreatePackage(data),
                            GPGController.GetOpponentId());
                    }
                    break;
                }
            }
        }
        else if (number.TurnNumber == 2)
        {
            for (int i = 0; i < PlayerTwoNumbers.Length; i++)
            {
                if (PlayerTwoNumbers[i] == number)
                {
                    PlayerTwoNumbers[i] = spawned;
                    AnimateDoor(2, i);
                    
                    break;
                }
            }
        }
    }

    public bool UpdateMatrix(int pos_x, int pos_y, Number card)
    {
        
        if(BoardHandler.GameBoardMatrix.row[pos_y].column[pos_x] != null)
        {
            return false;
        }
        if (isOnline)
        {
            string[] data = new string[]
            {
                ProjectConstants.message_played,
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
        return true;
    }

    private void SetGame()
    {
        if (LimitType == (int)LimitTypes.Charge && ScoreObject.Length > 0)
        {
            ScoreObject[0].Charge = ScoreObject[1].Charge = Limit;
            ScoreObject[0].UpdateCharge(0);
            ScoreObject[1].UpdateCharge(0);
        }
        if(BoardHandler != null)
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
            //Send YOU LOST message because I won
        }
        else if (winnerIndex == 1)
        {
            ScoreObject[0].PrintToTurnField(ProjectConstants.LoseText);
            //Send YOU WON message because I lost
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

    private void ControlMatrix(int column, int row, int cardNumber, bool isFromOpponent = false)
    {
        Number number = BoardHandler.GameBoardMatrix.row[row].column[column];
        scored = BoardHandler.ControlMatrix(column, row, cardNumber, number, isFromOpponent);
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
