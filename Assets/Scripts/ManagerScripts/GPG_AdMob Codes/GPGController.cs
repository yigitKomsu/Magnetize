using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System.Text;
using GooglePlayGames.BasicApi.Multiplayer;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;

public struct Player
{
    public Participant participant;
    public int assignedTurnNumber;
    public bool isMyself;
}

public class GPGController : RealTimeMultiplayerListener
{
    public static GPGController GetGpgController { get { return new GPGController(); } }

    public Player player_one, player_two;

    public static int matchCost;

    private bool IsCloudDataLoaded = false, IsSaving = false;

    public void LoginToGPG()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .EnableSavedGames()
            .Build();
        PlayGamesPlatform.DebugLogEnabled = true;
        Debug.Log(config.ToString());
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
        Login();
    }

    public void AssignTurns()
    {
        List<Participant> participants = PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants();

        if (participants[1].ParticipantId == PlayGamesPlatform.Instance.RealTime.GetSelf().ParticipantId)
            TurnManager.GetTurnManager.ChangeTurn();
    }

    private void Login()
    {
        var platformInstance = PlayGamesPlatform.Instance;
        if (!IsAuthenticated())
        {
            platformInstance.Authenticate(success =>
            {
                
            });
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

    public void AddScoreToLeaderBoard(string leaderboardId, int credit)
    {
        Social.ReportScore(credit, leaderboardId, success => { ProjectConstants.UpdateUserCredit(credit); });
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
    }

    public static void SendByteMessage(byte[] data, string participantId)
    {
        if (GameManager.GetGameManager != null && GameManager.GetGameManager.isOnline && PlayGamesPlatform.Instance.RealTime.IsRoomConnected())
            PlayGamesPlatform.Instance.RealTime.SendMessage(true, participantId, data);
    }

    public static string GetOpponentId()
    {
        if (GameManager.GetGameManager != null && GameManager.GetGameManager.isOnline && PlayGamesPlatform.Instance.RealTime.IsRoomConnected())
        {
            List<Participant> participants = PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants();
            string id = "";
            foreach (var item in participants)
            {
                if (item.ParticipantId != PlayGamesPlatform.Instance.RealTime.GetSelf().ParticipantId)
                {
                    id = item.ParticipantId;
                    break;
                }
            }
            return id;
        }
        return "";
    }

    public static string GetOpponentName()
    {
        List<Participant> participants = PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants();
        string id = "";
        foreach (var item in participants)
        {
            if (item.ParticipantId != PlayGamesPlatform.Instance.RealTime.GetSelf().ParticipantId)
            {
                id = item.DisplayName;
                break;
            }
        }
        return id;
    }

    public void OnRoomConnected(bool success)
    {
        if (success)
        {
            GameManager.GetGameManager.isOnline = true;
            Participant myself = PlayGamesPlatform.Instance.RealTime.GetSelf();
            List<Participant> participants = PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants();
            player_one.participant = participants[0];
            player_one.assignedTurnNumber = 1;
            player_one.isMyself = participants[0].ParticipantId == myself.ParticipantId;

            Bet.myTurn = player_one.isMyself;
            LevelManager.GetLevelManager.ToggleLight(Bet.myTurn);
            player_two.participant = participants[1];
            player_two.assignedTurnNumber = 2;
            player_two.isMyself = participants[1].ParticipantId == myself.ParticipantId;
            LevelManager.GetLevelManager.OpenBetPanel();
        }
        else
        {
            PlayGamesPlatform.Instance.RealTime.LeaveRoom();
        }
    }

    public void OnLeftRoom()
    {
        GameManager.GetGameManager.isOnline = false;
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
        PlayGamesPlatform.Instance.RealTime.LeaveRoom();
    }

    public void OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data)
    {
        var message = GPGBytePackager.UnpackPackage(data);
        GPGBytePackager.ProcessPackage(message);
    }
}
