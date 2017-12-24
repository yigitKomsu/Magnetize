using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{    
    public static GameManager Manager;
    [SerializeField]
    private TimeController[] Timer;
    private SoundManager _soundManager;
    private TurnManager _turnManager;
    [SerializeField]
    private ScoreHandler[] ScoreObject;
    [SerializeField]
    private Sprite[] Numbers;
    [SerializeField]
    private GameObject NumberBox;
    [SerializeField]
    private Vector2[] PlayerOnePositions, PlayerTwoPositions;
    public GameBoardHandler BoardHandler;
    public int Limit;
    public int LimitType;
    public bool scored;
    private Transform flyPosition;

    IEnumerator WaitRoutine(int turn)
    {
        yield return new WaitForSeconds(1);
        UpdateText(turn);
        StopCoroutine(WaitRoutine(turn));
    }

    private void Awake()
    {
        Manager = this;
    }

    private void OnLevelWasLoaded(int level)
    {
        LimitType = (int)LevelManager.Manager.Type;
        Limit = LevelManager.Manager.Limit;
    }

    // Use this for initialization
    void Start()
    {
        _turnManager = TurnManager.Manager;
        _soundManager = SoundManager.Manager;
        scored = false;
        if (LimitType == (int)LevelManager.LimitTypes.Time)
        {
            Timer[0].Init(Limit, 0);
            Timer[1].Init(Limit, 1);
            Timer[_turnManager.TurnNumber - 1].StartTimer();
            _soundManager.StartTimer();
        }

        SetGame();        
    }
    
    public void FinishByTime(int timeOut)
    {
        if(timeOut == 0)
        {
            ScoreObject[1].Score += Timer[1].PlayerTime;
        }
        else
        {
            ScoreObject[0].Score += Timer[0].PlayerTime;
        }        
        CompareScores();
        BoardHandler.ClearTheBoard();
    }
    
    public void DestroyedNumber(int number, int whose)
    {
        ScoreObject[whose].Scored(number);
        ScorePlayer(number + 1, whose);
        if (LimitType == (int)LevelManager.LimitTypes.Score)
            ControlForScore();
    }
    
    public void ReturnToMenu()
    {
        LevelManager.Manager.LoadLevelWithoutBanner(ProjectConstants.MainMenu);
        Destroy(LevelManager.Manager.gameObject);
    }

    public void SpawnCard(int number, Vector2 pos, int TurnNumber)
    {
        var obj = Instantiate(NumberBox, pos, Quaternion.identity);
        int newCardNumber = GetNewNumber(number); //yeni eklenen sayıyı eklemeyi unutma
        obj.GetComponent<Number>().TurnNumber = TurnNumber;
        obj.GetComponent<Number>().MyNumber = newCardNumber + 1;
        obj.GetComponent<SpriteRenderer>().sprite = Numbers[newCardNumber];
    }
    
    public void UpdateMatrix(int pos_x, int pos_y, Number card)
    {
        BoardHandler.GameBoardMatrix.row[pos_y].column[pos_x] = card;
        ControlMatrix(pos_x, pos_y, card.MyNumber);
    }

    private void SetGame()
    {
        for (int i = 0; i < PlayerOnePositions.Length; i++)
        {
            SpawnCard(i + 1, PlayerOnePositions[i], 1);
        }
        for (int i = 0; i < PlayerTwoPositions.Length; i++)
        {
            SpawnCard(i + 1, PlayerTwoPositions[i], 2);
        }
        StartCoroutine(WaitRoutine(_turnManager.TurnNumber - 1));
        BoardHandler.SumTheBoard();
    }

    private int GetNewNumber(int number)
    {
        return (BoardHandler.SumTheBoard(number)) % 3;
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
            ScoreObject[0].PrintMessage(ProjectConstants.TiedText + ScoreObject[0].Score);
            ScoreObject[1].PrintMessage(ProjectConstants.TiedText + ScoreObject[1].Score);
            return;
        }

        ScoreObject[winnerIndex].PrintMessage(ProjectConstants.WinText + ScoreObject[winnerIndex].Score);
        if (winnerIndex == 0)
        {
            ScoreObject[1].PrintMessage(ProjectConstants.LoseText + ScoreObject[1].Score);
        }
        else if (winnerIndex == 1)
        {
            ScoreObject[0].PrintMessage(ProjectConstants.LoseText + ScoreObject[0].Score);
        }
    }

    private void CompareScores()
    {
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
        if(LimitType == (int)LevelManager.LimitTypes.Time)
            Timer[_turnManager.TurnNumber - 1].StopTimer();
        _turnManager.ChangeTurn();
        ScoreObject[_turnManager.TurnNumber - 1].PrintMessage(ProjectConstants.TurnText);
        if (LimitType == (int)LevelManager.LimitTypes.Time)
            Timer[_turnManager.TurnNumber - 1].StartTimer();
        flyPosition = ScoreObject[_turnManager.TurnNumber - 1].transform.GetChild(3);
        BoardHandler.flyPosition = flyPosition;
        StartCoroutine(WaitRoutine(_turnManager.TurnNumber - 1));
    }

    private void ControlMatrix(int column, int row, int cardNumber)
    {
        Number number = BoardHandler.GameBoardMatrix.row[row].column[column];
        scored = BoardHandler.ControlMatrix(column, row, cardNumber, number);
        if (scored)
            number.Destroy(row, column, _turnManager.TurnNumber, flyPosition);
        scored = false;
        if (LimitType == (int)LevelManager.LimitTypes.Score)
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
