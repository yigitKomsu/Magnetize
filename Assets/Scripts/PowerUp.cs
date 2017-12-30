using System.Collections;
using UnityEngine;



public class PowerUp : MonoBehaviour {

    private PowerType powerType;
    [SerializeField]
    private Sprite[] powerSprites;

    private void Start()
    {
        var sr = GetComponent<SpriteRenderer>();
        powerType = GetRandomPower();
        switch (powerType)
        {
            case PowerType.None:
                break;
            case PowerType.DoubleScore:
                sr.sprite = powerSprites[0];
                break;
            case PowerType.MagnetizerBomb:
                sr.sprite = powerSprites[1];
                break;
            case PowerType.Refill:
                sr.sprite = powerSprites[2];
                break;
            default:
                break;
        }
    }

    private PowerType GetRandomPower()
    {
        int result = Random.Range(0, 3);
        switch (result)
        {
            case 0:
                return PowerType.DoubleScore;
            case 1:
                return PowerType.MagnetizerBomb;
            case 2:
                return PowerType.Refill;
            default:
                return PowerType.MagnetizerBomb;
        }
    }

    public void PowerThings(Number card, int row, int col)
    {
        switch (powerType)
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

    IEnumerator ParticleCoRoutine()
    {
        yield return new WaitForSeconds(0.9f);
        Destroy(gameObject);
    }
}
