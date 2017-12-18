using System.Collections;
using UnityEngine;

public class Number : MonoBehaviour
{
    public int MyNumber;
    private bool Dropped;
    private GameObject Colliding;
    private Vector2 StartPos;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip Drop, Pickup;
    public int TurnNumber;

    IEnumerator WaitRoutine(Vector3 position)
    {
        //uçuş animasyonu
        float speed = Random.Range(3f, 6f);
        while(Vector3.Distance(transform.position, position) > 3f)
        {
            Move(position, speed);
            yield return null;
        }
        GameManager.Manager.DestroyedNumber(MyNumber - 1);
        Destroy(gameObject);
    }

    private void Move(Vector3 position, float speed)
    {
        transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * speed);
    }

    // Use this for initialization
    void Start()
    {   
        StartPos = transform.position;
    }

    public void Destroy(int row, int column, Transform position)
    {
        GameManager.Manager.BoardHandler.GameBoardMatrix.row[row].column[column] = null;
        Fly(position);
    }

    public void Fly(Transform target)
    {
        transform.SetParent(target);
        StartCoroutine(WaitRoutine(target.position));
    }

    public void Fly(Vector3 position)
    {
        StartCoroutine(WaitRoutine(position));
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Colliding = collision.gameObject;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Colliding = null;
    }

    private void OnMouseDown()
    {
        if (TurnNumber == GameManager.Manager.TurnNumber)
            audioSource.PlayOneShot(Pickup);
    }

    private void OnMouseDrag()
    {
        if (TurnNumber == GameManager.Manager.TurnNumber)
        {
            Dropped = false;
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        }
    }

    private void OnMouseUp()
    {
        Dropped = true;
        if (Colliding == null || Colliding.transform.childCount == 1)
        {
            audioSource.PlayOneShot(Drop);
            transform.position = StartPos;
        }
        else if (Dropped)
        {
            GameManager.Manager.SpawnCard(MyNumber, StartPos, TurnNumber);
            transform.SetParent(Colliding.transform);
            transform.localPosition = Vector3.zero;
            transform.localScale *= 0.85f;
            GameManager.Manager.UpdateMatrix(Mathf.RoundToInt(Colliding.transform.position.x + 2),
                Mathf.RoundToInt(Colliding.transform.position.y + 2), this);
        }
    }
    
}
