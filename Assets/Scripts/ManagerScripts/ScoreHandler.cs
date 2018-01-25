using UnityEngine;
using UnityEngine.UI;

public class ScoreHandler : MonoBehaviour
{
    [SerializeField]
    private Text _playerText;
    [SerializeField]
    private Text _messageText;
    [SerializeField]
    private Image _pieceLimit;
    public Transform Magnet;
    public int Charge;
    private float limitPerc;
    private float height = 578.0f;
    public int Score;

    private void Start()
    {
        if (Charge != 0)
            limitPerc = 578 / Charge;
    }

    public void UpdateInternalCharge(int value)
    {
        if(Charge - value > 0)
        {
            Charge -= value;            
        }
            
        else if (Charge - value >= GameManager.GetGameManager.Limit)
        {
            Charge = GameManager.GetGameManager.Limit;
        }
        else
        {
            Charge = 0;
            _pieceLimit.rectTransform.sizeDelta = new Vector2(64, 0);
            GameManager.GetGameManager.FinishGame();
            return;
        }
        _pieceLimit.rectTransform.sizeDelta = new Vector2(64, height - (limitPerc * value));
        height -= (limitPerc * value);
    }

    public void UpdateCharge(int value)
    {
        if (Charge - value > 0)
        {
            Charge -= value;
            string[] data = new string[]
            {
                ProjectConstants.message_chargeUpdate,
                Charge.ToString()
            };
            GPGController.SendByteMessage(GPGBytePackager.CreatePackage(data),
                GPGController.GetOpponentId());
        }
            
        else if (Charge - value >= GameManager.GetGameManager.Limit)
        {
            Charge = GameManager.GetGameManager.Limit;
        }
        else
        {
            Charge = 0;
            _pieceLimit.rectTransform.sizeDelta = new Vector2(64, 0);
            GameManager.GetGameManager.FinishGame();
            string[] data = new string[]
            {
                ProjectConstants.message_chargeUpdate,
                Charge.ToString()
            };
            GPGController.SendByteMessage(GPGBytePackager.CreatePackage(data),
                GPGController.GetOpponentId());
            return;
        }
        _pieceLimit.rectTransform.sizeDelta = new Vector2(64, height - (limitPerc * value));
        height -= (limitPerc * value);
    }

    public void PrintToScoreField(string message)
    {
        _playerText.text = message;
    }

    public void PrintToTurnField(string message)
    {
        _messageText.text = message;
    }

    public void Scored(int number)
    {
        if (GameManager.GetGameManager.LimitType == (int)LimitTypes.Charge)
            UpdateCharge(0);
        Score += number + 1;
    }

    public void PrintScore()
    {
        _playerText.text = Score.ToString("000");
    }
}
