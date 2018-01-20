using System.Collections;
using UnityEngine;

public class Number : MonoBehaviour
{
    public int MyNumber;
    private bool Dropped;
    private Transform _colliding;
    private Transform _myTransform;
    private TurnManager _turnManager;
    private GameManager _manager;
    private Vector2 StartPos;
    private SoundManager _soundManager;
    private Animator _animator;
    public int TurnNumber;
    private int ScoreForWhom;
    private bool isDouble;
    public bool isMagnetized;
    private bool isRefill;
    private SpriteRenderer sr;
    private GameObject DoorObject;
    protected AnimatorOverrideController animatorOverrideController;
    

    IEnumerator WaitRoutine(Transform target)
    {
        float speed = Random.Range(7f, 12f);
        Vector3 offset = new Vector3(Random.Range(-0.4f, 0.4f), 0, 0);
        Vector3 pos = target.position;
        pos += offset;
        if (isDouble) MyNumber *= 2;
        while (Vector3.Distance(transform.position, pos) > 3f)
        {
            Move(pos, speed);
            yield return null;
        }
        _manager.DestroyedNumber(MyNumber - 1, ScoreForWhom - 1);

        if (isMagnetized) _manager.BoardHandler.DestroyMagnetized(ScoreForWhom - 1, target);

        if (isRefill) _manager.RefillPower(MyNumber, ScoreForWhom - 1);

        TurnNumber = -1;
        GetComponent<BoxCollider2D>().enabled = false;
        StopAllCoroutines();
    }

    private void Move(Vector3 position, float speed)
    {
        transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * speed);
    }

    // Use this for initialization
    void Start()
    {
        transform.GetChild(0).parent = null;
        sr = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _animator.runtimeAnimatorController = Resources.Load("NumberBox" + MyNumber) as RuntimeAnimatorController;
        _turnManager = TurnManager.GetTurnManager;
        _soundManager = SoundManager.GetSoundManager;
        _manager = GameManager.GetGameManager;
        _myTransform = transform;
        StartPos = _myTransform.position;
    }

    public void Destroy(int row, int column, int playerNum, Transform position)
    {
        ScoreForWhom = playerNum;
        _manager.BoardHandler.GameBoardMatrix.row[row].column[column] = null;
        Fly(position);
    }

    public void Fly(Transform target)
    {
        transform.parent = null;
        StartCoroutine(WaitRoutine(target));
    }

    public void DoublePower()
    {
        _animator.SetTrigger("double");
        isDouble = true;
    }

    public void Magnetize(int? row = null, int? col = null)
    {
        _animator.SetTrigger("magnetize");
        if (row != null)
            _manager.BoardHandler.MagnetizeRow(row, col);
        isMagnetized = true;
    }

    public void Refill()
    {
        //_soundManager.PlayRefill();
        _animator.SetTrigger("refill");
        isRefill = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        _colliding = collision.gameObject.transform;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _colliding = null;
    }

    private void OnMouseDown()
    {
        if (TurnNumber == _turnManager.TurnNumber)
            _soundManager.PickNumber();
    }

    private void OnMouseDrag()
    {
        if (TurnNumber == _turnManager.TurnNumber)
        {
            Dropped = false;
            _myTransform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        }
    }

    private void OnMouseUp()
    {
        Dropped = true;
        if (_colliding == null || _colliding.childCount == 1)
        {
            _myTransform.position = StartPos;
        }
        else if (Dropped)
        {
            _myTransform.SetParent(_colliding);
            _myTransform.localPosition = Vector3.zero;
            _myTransform.localScale *= 0.85f;
            _manager.UpdateMatrix(Mathf.RoundToInt(_colliding.position.x + 2),
                Mathf.RoundToInt(_colliding.position.y + 2), this);
            _manager.SpawnCard(MyNumber, StartPos, TurnNumber);
            sr.sortingOrder = 0;
        }
        _soundManager.DropNumber();
    }
}
