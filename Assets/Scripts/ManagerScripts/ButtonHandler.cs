using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHandler : MonoBehaviour {

    public void Call()
    {
        Bet.Call();
    }

    public void CallAndIncrease()
    {
        Bet.CallAndIncrease(Bet.YourBet);
    }

    public void RequestVideo()
    {
        AdController.ShowVideo();
    }

    public void ShowAchievements()
    {
        GPGController.ShowAchievements();
    }

    public void WaitingRoomClose()
    {
        GPGController.LeaveRoom();
        LevelManager.GetLevelManager.WaitingPanelClosed();
        LevelManager.GetLevelManager.UpdateOnlineStatusText("CANCELLING");        
    }
}
