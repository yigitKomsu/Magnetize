using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public int TurnNumber;
    public static TurnManager GetTurnManager { get; private set; }
    [SerializeField]
    private Animator[] _animator;
    [SerializeField]
    private TimeController[] Timer;
    public bool timeOut;
    private void Awake()
    {
        GetTurnManager = this;
    }

    // Use this for initialization
    void Start()
    {
        _animator[TurnNumber - 1].SetTrigger("StartTurn");
        if (GameManager.GetGameManager.LimitType == (int)LimitTypes.Time)
        {
            Timer[0].Init(GameManager.GetGameManager.Limit, 0);
            Timer[1].Init(GameManager.GetGameManager.Limit, 1);
            Timer[TurnNumber - 1].StartTimer();
            SoundManager.GetSoundManager.StartTimer();
        }
    }

    public void AddTime(int time, int turn)
    {
        Timer[turn].PlayerTime += time;
    }

    public void SetTime(int time, int turn)
    {
        Timer[turn].PlayerTime = time;
    }

    public void ChangeTurn()
    {
        if (timeOut)
            return;
        if (GameManager.GetGameManager.LimitType == (int)LimitTypes.Time)
            Timer[TurnNumber - 1].StopTimer();
        _animator[TurnNumber - 1].SetTrigger("StopTurn");
        if (TurnNumber == 1)
        {
            TurnNumber = 2;
        }
        else
        {
            TurnNumber = 1;
        }
        _animator[TurnNumber - 1].SetTrigger("StartTurn");
        if (GameManager.GetGameManager.LimitType == (int)LimitTypes.Time)
            Timer[TurnNumber - 1].StartTimer();

        Debug.Log("Turn number is: " + TurnNumber);
    }

    public void ShowPanels()
    {
        _animator[0].SetTrigger("StartTurn");
        _animator[1].SetTrigger("StartTurn");
    }
    public int GetRemainingTime(int TurnIndex)
    {
        return Timer[TurnIndex].PlayerTime;
    }

}
