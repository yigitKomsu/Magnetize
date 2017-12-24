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
    public int TurnNumber;
    private int ScoreForWhom;

    IEnumerator WaitRoutine(Vector3 position)
    {
        float speed = Random.Range(3f, 6f);
        while (Vector3.Distance(transform.position, position) > 3f)
        {
            Move(position, speed);
            yield return null;
        }
        _manager.DestroyedNumber(MyNumber - 1, ScoreForWhom - 1);
        Destroy(gameObject);
    }

    private void Move(Vector3 position, float speed)
    {
        transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * speed);
    }

    // Use this for initialization
    void Start()
    {
        _turnManager = TurnManager.Manager;
        _soundManager = SoundManager.Manager;
        _manager = GameManager.Manager;
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
        _myTransform.SetParent(target);
        StartCoroutine(WaitRoutine(target.position));
    }

    public void Fly(Vector3 position)
    {
        StartCoroutine(WaitRoutine(position));
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        _colliding = collision.gameObject.transform;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _colliding = null;
    }

    //Optimize this
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
            _manager.SpawnCard(MyNumber, StartPos, TurnNumber);
            _myTransform.SetParent(_colliding);
            _myTransform.localPosition = Vector3.zero;
            _myTransform.localScale *= 0.85f;
            _manager.UpdateMatrix(Mathf.RoundToInt(_colliding.position.x + 2),
                Mathf.RoundToInt(_colliding.position.y + 2), this);
        }
        _soundManager.DropNumber();
    }

}
