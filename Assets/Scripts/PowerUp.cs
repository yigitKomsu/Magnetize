using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp", menuName = "PowerUp/PowerUp", order = 1)]
public class PowerUp : ScriptableObject {

    public PowerType powerType;
    public Sprite powerSprite;  

    

    IEnumerator ParticleCoRoutine()
    {
        yield return new WaitForSeconds(0.9f);
    }
}
