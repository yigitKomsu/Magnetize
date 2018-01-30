using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerHolder : MonoBehaviour {

    public PowerUp MyPower;

    private void Start()
    {
        
    }

    public void PowerThings(Number card, int row, int col, bool isFromOpponent = false)
    {
        switch (MyPower.powerType)
        {
            case PowerType.None:
                break;
            case PowerType.DoubleScore:
                card.DoublePower();
                break;
            case PowerType.MagnetizerBomb:
                card.Magnetize(row, col);
                break;
            case PowerType.Refill:
                card.Refill();
                break;
            default:
                break;
        }
        Destroy(gameObject);
    }

    public void ChangeSprite()
    {
        GetComponent<SpriteRenderer>().sprite = MyPower.powerSprite;
    }

}
