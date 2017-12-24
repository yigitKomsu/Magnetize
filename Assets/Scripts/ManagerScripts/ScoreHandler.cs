using UnityEngine;
using UnityEngine.UI;

public class ScoreHandler : MonoBehaviour
{
    [SerializeField]
    private Text _playerText;
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private Animator[] _plusTextAnimator;
    public int Score;

    public void PrintMessage(string message)
    {
        _playerText.text = message;
    }

    public void Scored(int number)
    {
        _animator.SetTrigger("ScoreTrigger");
        Score += number + 1;
        _plusTextAnimator[number].SetTrigger("FadeTrigger");
    }

    public void PrintScore()
    {
        _playerText.text = ProjectConstants.ScoreText + Score;
    }
}
