using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public int TurnNumber;
    public static TurnManager Manager;
    [SerializeField]
    private Animator[] _animator;
    private void Awake()
    {
        Manager = this;
    }

    // Use this for initialization
    void Start()
    {
        _animator[TurnNumber - 1].SetTrigger("StartTurn");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeTurn()
    {
        if (TurnNumber == 1)
        {
            _animator[TurnNumber - 1].SetTrigger("StopTurn");
            TurnNumber = 2;
            _animator[TurnNumber - 1].SetTrigger("StartTurn");
        }
        else
        {
            _animator[TurnNumber - 1].SetTrigger("StopTurn");
            TurnNumber = 1;
            _animator[TurnNumber - 1].SetTrigger("StartTurn");
        }
    }
}
