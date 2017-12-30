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
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private Animator[] _plusTextAnimator;
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

    public void UpdateCharge(int value)
    {
        if (Charge - value > 0)
            Charge -= value;
        else
        {
            Charge = 0;
            _pieceLimit.rectTransform.sizeDelta = new Vector2(64, 0);
            GameManager.Manager.FinishGame();
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
        //_animator.SetTrigger("ScoreTrigger");
        Score += number + 1;
        //_plusTextAnimator[number].SetTrigger("FadeTrigger");
    }

    public void PrintScore()
    {
        _playerText.text = Score.ToString("000");
    }
}
