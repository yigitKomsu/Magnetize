using UnityEngine;
using UnityEngine.UI;

public class TimeController : MonoBehaviour
{
    [SerializeField]
    private Text TimerText;
    [SerializeField]
    private Animator _animator;
    public int PlayerTime;
    private int TurnNumber;
    private GameManager _manager;
    private TurnManager _turnManager;

    private void Start()
    {
        _manager = GameManager.GetGameManager;
        _turnManager = TurnManager.GetTurnManager;
    }

    public void Init(int _limit, int turnNumber)
    {
        TurnNumber = turnNumber + 1;
        TimerText.text = _limit.ToString();
        PlayerTime = _limit;
        InvokeRepeating("UpdateFrame", 0, 1.0f);
    }

    public void FinishGame()
    {
        CancelInvoke("UpdateFrame");
    }

    public void StartTimer()
    {
        _animator.SetTrigger("TimerStart");
        string[] data = new string[]
        {
            ProjectConstants.message_updateTime,
            PlayerTime.ToString()
        };
        GPGController.SendByteMessage(GPGBytePackager.CreatePackage(data), 
            GPGController.GetOpponentId());
    }

    public void StopTimer()
    {
        _animator.SetTrigger("TimerStop");
    }

    private void UpdateFrame()
    {
        if (_turnManager.TurnNumber == TurnNumber)
        {
            PlayerTime--;
            TimerText.text = PlayerTime.ToString("000");
            if (PlayerTime == 0)
            {
                //_manager.FinishByTime(TurnNumber - 1);
                if (_turnManager.timeOut == true)
                {
                    StopTimer();
                    _manager.FinishGame();
                    return;
                }
                _turnManager.ChangeTurn();
                _turnManager.timeOut = true;
                _manager.PrintMessageToTurnField("OVERTIME");
                FinishGame();
            }
        }
    }
}
