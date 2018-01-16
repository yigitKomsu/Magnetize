using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Multiplayer;
using UnityEngine;
using System.Collections.Generic;

public struct Player
{
    public Participant participant;
    public int assignedTurnNumber;
    public bool isMyself;
}

public class GPGController : RealTimeMultiplayerListener
{
    public static GPGController GpgController { get { return new GPGController(); } }

    public Player player_one, player_two;

    public static int matchCost; //half of total bet

    public static void LoginToGPG()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
        Login();
    }

    private static void Login()
    {
        var platformInstance = PlayGamesPlatform.Instance;
        if (!IsAuthenticated())
        {
            platformInstance.Authenticate(success => { Debug.Log("Logged in to Google Play Services!"); });
        }
    }

    public static void ShowAchievements()
    {
        Social.ShowAchievementsUI();
    }

    public static void IncrementAchievement(string id, double value)
    {
        Social.ReportProgress(id, value, success => { });
    }

    public static void UnlockAchievement(string id)
    {
        Social.ReportProgress(id, 100, success => { });
    }

    public static bool IsAuthenticated()
    {
        return PlayGamesPlatform.Instance.localUser.authenticated;
    }

    public static string GetUsername()
    {
        return PlayGamesPlatform.Instance.localUser.userName;
    }

    public static void WinGame()
    {
        ProjectConstants.UpdateUserCredit(matchCost);
    }

    public static void LoseGame()
    {
        ProjectConstants.UpdateUserCredit(-matchCost);
    }

    public void CreateOrJoinQuickMatch(int type)
    {
        if (!IsAuthenticated())
            return;
        try
        {
            LevelManager.GetLevelManager.UpdateOnlineStatusText("CONNECTING");
            const int MinOpponents = 1, MaxOpponents = 1;
            PlayGamesPlatform.Instance.RealTime.CreateQuickGame(MinOpponents, MaxOpponents,
                        (uint)type, this);
        }
        catch (Exception ex)
        {
            ProjectConstants.UpdateUserCredit(matchCost);
            LevelManager.GetLevelManager.UpdateOnlineStatusText(ex.Message);
        }
    }

    public void OnRoomSetupProgress(float percent)
    {
        LevelManager.GetLevelManager.UpdateOnlineStatusText("SEARCHING FOR ROOM");
    }

    public static void LeaveRoom()
    {
        PlayGamesPlatform.Instance.RealTime.LeaveRoom();
        LevelManager.GetLevelManager.UpdateOnlineStatusText("CANCELLING");
    }

    public static void SendByteMessage(byte[] data, string participantId)
    {
        PlayGamesPlatform.Instance.RealTime.SendMessage(true, participantId, data);
    }

    public static string GetOpponentId()
    {
        List<Participant> participants = PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants();
        string id = "";
        foreach (var item in participants)
        {
            if(item.ParticipantId != PlayGamesPlatform.Instance.RealTime.GetSelf().ParticipantId)
            {
                id = item.ParticipantId;
                break;
            }
        }
        return id;
    }

    public void OnRoomConnected(bool success)
    {
        if (success)
        {
            LevelManager.GetLevelManager.UpdateOnlineStatusText("CONNECTED!");
            Participant myself = PlayGamesPlatform.Instance.RealTime.GetSelf();
            Debug.Log("My participant ID is " + myself.ParticipantId);
            List<Participant> participants = PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants();
            player_one.participant = participants[0];
            player_one.assignedTurnNumber = 1;
            player_one.isMyself = participants[0].ParticipantId == myself.ParticipantId;

            Bet.myTurn = player_one.isMyself;

            player_two.participant = participants[1];
            player_two.assignedTurnNumber = 2;
            player_two.isMyself = participants[0].ParticipantId == myself.ParticipantId;
            Debug.Log(player_one);
            Debug.Log(player_two);
            LevelManager.GetLevelManager.OpenBetPanel();
        }
        else
        {
            LevelManager.GetLevelManager.UpdateOnlineStatusText("FAILED TO CONNECT");
            PlayGamesPlatform.Instance.RealTime.LeaveRoom();
        }
    }

    public void OnLeftRoom()
    {
        LevelManager.GetLevelManager.WaitingPanelClosed();
    }

    public void OnParticipantLeft(Participant participant)
    {
        LevelManager.GetLevelManager.UpdateOnlineStatusText("PARTICIPANT LEFT");
        PlayGamesPlatform.Instance.RealTime.LeaveRoom();
    }

    public void OnPeersConnected(string[] participantIds)
    {
        throw new NotImplementedException();
    }

    public void OnPeersDisconnected(string[] participantIds)
    {
        throw new NotImplementedException();
    }

    public void OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data)
    {
        var message = GPGBytePackager.UnpackPackage(data);
        Debug.Log("I received message from: " + senderId + " ");
        GPGBytePackager.ProcessPackage(message);
        
    }
}
