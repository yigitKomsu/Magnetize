using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public int TurnNumber;
    public static TurnManager Manager;
    [SerializeField]
    private Animator[] _animator;
    [SerializeField]
    private TimeController[] Timer;
    public bool timeOut;
    private void Awake()
    {
        Manager = this;
    }

    // Use this for initialization
    void Start()
    {
        _animator[TurnNumber - 1].SetTrigger("StartTurn");
        if (GameManager.Manager.LimitType == (int)LimitTypes.Time)
        {
            Timer[0].Init(GameManager.Manager.Limit, 0);
            Timer[1].Init(GameManager.Manager.Limit, 1);
            Timer[TurnNumber - 1].StartTimer();
            SoundManager.Manager.StartTimer();
        }
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddTime(int time, int turn)
    {
        Timer[turn].PlayerTime += time;
    }

    public void ChangeTurn()
    {
        if (timeOut)
            return;
        if (GameManager.Manager.LimitType == (int)LimitTypes.Time)
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
        if (GameManager.Manager.LimitType == (int)LimitTypes.Time)
            Timer[TurnNumber - 1].StartTimer();
    }

    public int GetRemainingTime(int TurnIndex)
    {
        return Timer[TurnIndex].PlayerTime;
    }

}
