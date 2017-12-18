using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Manager;
    [SerializeField]
    private Text[] TimerText;
    [SerializeField]
    private AudioClip SoundEffect;
    [SerializeField]
    private ScoreHandler[] ScoreObject;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private Sprite[] Numbers;
    [SerializeField]
    private int[] PlayerTimes;
    [SerializeField]
    private GameObject NumberBox;
    [SerializeField]
    private Vector2[] PlayerOnePositions, PlayerTwoPositions;
    //[SerializeField]
    //private ArrayLayout GameBoardMatrix;
    [SerializeField]
    public GameBoardHandler BoardHandler;
    public int TurnNumber = 1;
    public int Limit;
    public int LimitType;
    public bool scored;
    private Transform flyPosition;
    IEnumerator WaitRoutine(int turn)
    {
        yield return new WaitForSeconds(1);
        UpdateText(turn);
        StopAllCoroutines();
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
        scored = false;
        if (LimitType == (int)LevelManager.LimitTypes.Time)
        {
            TimerText[0].text = "TIME: " + Limit;
            TimerText[1].text = "TIME: " + Limit;
            PlayerTimes[0] = Limit;
            PlayerTimes[1] = Limit;
            InvokeRepeating("UpdateFrame", 0, 1.0f);
        }
        else
        {
            TimerText[0].gameObject.SetActive(false);
            TimerText[1].gameObject.SetActive(false);
        }
        for (int i = 0; i < PlayerOnePositions.Length; i++)
        {
            SpawnCard(i + 1, PlayerOnePositions[i], 1);
        }
        for (int i = 0; i < PlayerTwoPositions.Length; i++)
        {
            SpawnCard(i + 1, PlayerTwoPositions[i], 2);
        }
        StartCoroutine(WaitRoutine(TurnNumber - 1));        
    }

    private void UpdateFrame()
    {
        PlayerTimes[TurnNumber - 1]--;
        TimerText[TurnNumber - 1].text = "TIME: " + PlayerTimes[TurnNumber - 1];
        if (PlayerTimes[TurnNumber - 1] == 0)
        {
            CompareScores();
            BoardHandler.ClearTheBoard();
            CancelInvoke("UpdateFrame");
        }
    }

    public void DestroyedNumber(int number)
    {
        if (TurnNumber == 1)
        {
            ScoreObject[1].Scored(number);
            ScorePlayer(number + 1, 1);
        }
        else
        {
            ScoreObject[0].Scored(number);
            ScorePlayer(number + 1, 0);
        }
    }

    public void ReturnToMenu()
    {
        LevelManager.Manager.LoadLevelWithoutBanner("MainMenu");
        Destroy(LevelManager.Manager.gameObject);
    }    

    public void SpawnCard(int number, Vector2 pos, int turnNumber) //the dropped card's number is sent
    {
        var obj = Instantiate(NumberBox, pos, Quaternion.identity);
        int newCardNumber = GetNewNumber(number); //yeni eklenen sayıyı eklemeyi unutma
        obj.GetComponent<Number>().TurnNumber = turnNumber;
        obj.GetComponent<Number>().MyNumber = newCardNumber + 1;
        obj.GetComponent<SpriteRenderer>().sprite = Numbers[newCardNumber];
        //obj.transform.GetChild(0).GetComponent<SpriteRenderer>().color = 
        //    UnityEngine.Random.ColorHSV(0.3f + ((float)newCardNumber / 10), 0.3f + ((float)newCardNumber / 10), 0.05f, 0.15f, 0.8f, 1f);
    }

    private int GetNewNumber(int number)
    {        
        return (BoardHandler.SumTheBoard(number)) % 3;
    }

    public void UpdateMatrix(int pos_x, int pos_y, Number card)
    {
        BoardHandler.GameBoardMatrix.row[pos_y].column[pos_x] = card;
        ControlMatrix(pos_x, pos_y, card.MyNumber);
    }

    public void UpdateText(int turn)
    {
        ScoreObject[turn].PrintScore();
        BoardHandler.SumTheBoard();
    }

    private void ControlForScore()
    {
        if (ScoreObject[0].Score >= Limit)
        {
            ScoreObject[0].PrintMessage("YOU WIN" + ScoreObject[0].Score);
            ScoreObject[1].PrintMessage("YOU LOSE" + ScoreObject[1].Score);
            BoardHandler.ClearTheBoard();
        }
        else if (ScoreObject[0].Score >= Limit)
        {
            ScoreObject[0].PrintMessage("YOU LOSE" + ScoreObject[0].Score);
            ScoreObject[1].PrintMessage("YOU WIN" + ScoreObject[1].Score);
            BoardHandler.ClearTheBoard();
        }
    }

    private void CompareScores()
    {
        if (ScoreObject[0].Score > ScoreObject[1].Score)
        {
            ScoreObject[0].PrintMessage("YOU WIN" + ScoreObject[0].Score);
            ScoreObject[1].PrintMessage("YOU LOSE" + ScoreObject[1].Score);
        }
        else if (ScoreObject[0].Score < ScoreObject[1].Score)
        {
            ScoreObject[0].PrintMessage("YOU LOSE" + ScoreObject[0].Score);
            ScoreObject[1].PrintMessage("YOU WIN" + ScoreObject[1].Score);
        }
        else
        {
            ScoreObject[0].PrintMessage("TIED" + ScoreObject[0].Score);
            ScoreObject[1].PrintMessage("TIED" + ScoreObject[1].Score);
        }
    }

    private void ChangeTurn()
    {
        if (TurnNumber == 1)
        {
            TurnNumber = 2;
        }
        else
        {
            TurnNumber = 1;
        }
        ScoreObject[TurnNumber - 1].PrintMessage("YOUR TURN");
        flyPosition = ScoreObject[TurnNumber - 1].transform.GetChild(0);
        BoardHandler.flyPosition = flyPosition;
        StartCoroutine(WaitRoutine(TurnNumber - 1));
    }
    
    private void ControlMatrix(int column, int row, int cardNumber)
    {
        Number number = BoardHandler.GameBoardMatrix.row[row].column[column];
        scored = BoardHandler.ControlMatrix(column, row, cardNumber, number);
        if (scored)
            number.Destroy(row, column, flyPosition);
        scored = false;
        ChangeTurn();
        if (LimitType == (int)LevelManager.LimitTypes.Score)
            ControlForScore();
        if (BoardHandler.CheckBoardFull())
        {
            CompareScores();
            BoardHandler.ClearTheBoard();
        }
    }

    private void ScorePlayer(int score, int turn)
    {
        audioSource.PlayOneShot(SoundEffect);
        UpdateText(turn);
    }
}
