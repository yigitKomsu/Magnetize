using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{

    [SerializeField]
    private Animator Menu;
    private Vector2 touchdeltaPosition, touchStartPosition;
    private bool slid;
    [SerializeField]
    private Text[] limitText;
    private int index;
    private void Start()
    {
        index = 1;
    }

    public void ChangeLimit(int value)
    {
        LevelManager.Manager.SetLimit(value);
        limitText[index].text = LevelManager.Manager.Limit.ToString("000");
    }

    private void Update()
    {
        if (Input.touches.Length > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                touchStartPosition = Input.GetTouch(0).position;
            }
            if (Input.GetTouch(0).phase == TouchPhase.Moved && !slid)
            {
                touchdeltaPosition = Input.GetTouch(0).position;
                if (touchdeltaPosition.x > touchStartPosition.x + 100)
                {
                    Debug.Log("Slided right");
                    index = Menu.GetInteger("MenuIndex");
                    if (index < 2) index++;
                    Menu.SetInteger("MenuIndex", index);                    
                    LevelManager.Manager.SetType(index);
                    limitText[index].text = LevelManager.Manager.Limit.ToString("000");
                    slid = true;
                }
                else if (touchdeltaPosition.x < touchStartPosition.x - 100)
                {
                    Debug.Log("Slided left");
                    index = Menu.GetInteger("MenuIndex");
                    if (index > 0) index--;
                    LevelManager.Manager.SetType(index);
                    Menu.SetInteger("MenuIndex", index);
                    limitText[index].text = LevelManager.Manager.Limit.ToString("000");
                    slid = true;
                }
            }
        }
        else
        {
            touchdeltaPosition = touchStartPosition;
            slid = false;
        }
    }
}
