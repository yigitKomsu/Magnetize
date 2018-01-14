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
        if (!platformInstance.localUser.authenticated)
        {
            platformInstance.Authenticate(success => { LevelManager.GetLevelManager.UpdateUsernameText(GetUsername()); });
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

    public void CreateOrJoinQuickMatch(int type)
    {
        try
        {
            LevelManager.GetLevelManager.UpdateOnlineStatusText("CONNECTING");
            const int MinOpponents = 1, MaxOpponents = 1;
            PlayGamesPlatform.Instance.RealTime.CreateQuickGame(MinOpponents, MaxOpponents,
                        (uint)type, this);
        }
        catch (Exception ex)
        {
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
            player_two.participant = participants[1];
            player_two.assignedTurnNumber = 2;
            player_two.isMyself = participants[0].ParticipantId == myself.ParticipantId;
            Debug.Log(player_one);
            Debug.Log(player_two);
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
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }
}
