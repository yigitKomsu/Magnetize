using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager GetGameManager { get; private set; }
    private SoundManager _soundManager;
    private TurnManager _turnManager;
    [SerializeField]
    private ScoreHandler[] ScoreObject;
    [SerializeField]
    public Sprite[] Numbers, Magnetized;
    [SerializeField]
    private GameObject NumberBox;
    [SerializeField]
    private Vector2[] PlayerOnePositions, PlayerTwoPositions;
    public GameBoardHandler BoardHandler;
    public int Limit;
    public int LimitType;
    public bool scored;
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

    public void SpawnCard(int number, Vector2 pos, int TurnNumber)
    {
        var obj = Instantiate(NumberBox, pos, Quaternion.identity);
        int newCardNumber = GetNewNumber(number); //yeni eklenen sayıyı eklemeyi unutma
        obj.GetComponent<Number>().TurnNumber = TurnNumber;
        obj.GetComponent<Number>().MyNumber = newCardNumber + 1;
        if (LimitType == (int)LimitTypes.Charge && ScoreObject[TurnNumber - 1].Charge >= 0)
        {
            ScoreObject[TurnNumber - 1].UpdateCharge(newCardNumber + 1);
        }
    }

    public void UpdateMatrix(int pos_x, int pos_y, Number card)
    {
        //Post this information to the connected participant for update        
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
            _turnManager.ShowPanels();
            return;
        }

        ScoreObject[winnerIndex].PrintToTurnField(ProjectConstants.WinText);
        if (winnerIndex == 0)
        {
            ScoreObject[1].PrintToTurnField(ProjectConstants.LoseText);
        }
        else if (winnerIndex == 1)
        {
            ScoreObject[0].PrintToTurnField(ProjectConstants.LoseText);
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
