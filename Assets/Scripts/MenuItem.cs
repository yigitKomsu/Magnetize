using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuItem : MonoBehaviour {

    public int Limit;

    public void ChangeLimitValue(MenuItem item, int value)
    {
        item.Limit += value;
    }

    public void SendLimitValue(LimitTypes Type)
    {
        LevelManager.GetLevelManager.Limit = Limit;
        LevelManager.GetLevelManager.Type = Type;
    }
}
