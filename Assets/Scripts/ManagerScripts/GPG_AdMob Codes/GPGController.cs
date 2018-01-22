using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System.Text;
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
    public static GPGController GetGpgController { get { return new GPGController(); } }

    public Player player_one, player_two;

    public static int matchCost; //half of total bet

    private const string SAVE_NAME = "Save";

    private bool IsCloudDataLoaded = false, IsSaving = false;

    public void LoginToGPG()
    {
        if (!PlayerPrefs.HasKey(SAVE_NAME))
        {
            PlayerPrefs.SetString(SAVE_NAME, "500");
        }
        if (!PlayerPrefs.HasKey("IsFirstTime"))
        {
            PlayerPrefs.SetInt("IsFirstTime", 1);
        }

        LoadLocal();
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
            platformInstance.Authenticate(success => {
                LoadData();
            });
        }
    }

    #region Save
    private string GameDataToString()
    {
        return ProjectConstants.userCredit.ToString();
    }

    private void StringToGameData(string localData, string cloudData)
    {
        if (PlayerPrefs.GetInt("IsFirstTime") == 1)
        {
            PlayerPrefs.SetInt("IsFirstTime", 0);
            if (int.Parse(cloudData) > int.Parse(localData))
            {
                PlayerPrefs.SetString(SAVE_NAME, cloudData);
            }
        }
        else
        {
            if (int.Parse(localData) > int.Parse(cloudData))
            {
                ProjectConstants.userCredit = int.Parse(localData);
                AddScoreToLeaderBoard(GPGSIds.leaderboard_credits, ProjectConstants.userCredit);
                IsCloudDataLoaded = true;
                SaveData();
            }
        }
        ProjectConstants.userCredit = int.Parse(cloudData);
        IsCloudDataLoaded = true;
    }

    private void StringToGameData(string localData)
    {
        ProjectConstants.userCredit = int.Parse(localData);
    }

    public void LoadData()
    {
        if (IsAuthenticated())
        {
            Debug.Log("Load data called");
            IsSaving = false;
            ((PlayGamesPlatform)Social.Active).SavedGame.
                OpenWithManualConflictResolution(SAVE_NAME,
                DataSource.ReadCacheOrNetwork, true, ResolveConflict, OnSavedGameOpened);
        }
        else
        {
            LoadLocal();
        }
    }

    private void LoadLocal()
    {
        StringToGameData(PlayerPrefs.GetString(SAVE_NAME));
    }

    private void SaveLocal()
    {
        Debug.Log(ProjectConstants.userCredit);
        PlayerPrefs.SetString(SAVE_NAME, GameDataToString());
    }

    public void SaveData()
    {
        if (!IsCloudDataLoaded)
        {
            SaveLocal();
            return;
        }
        if (IsAuthenticated())
        {
            IsSaving = true;
            Debug.Log(ProjectConstants.userCredit);
            ((PlayGamesPlatform)Social.Active).SavedGame.
                OpenWithManualConflictResolution(SAVE_NAME,
                DataSource.ReadCacheOrNetwork, true, ResolveConflict, OnSavedGameOpened);
        }
        else
        {
            SaveLocal();
        }
    }

    private void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            if (!IsSaving)
            {
                LoadGame(game);
            }
            else
            {
                SaveGame(game);
            }
        }
        else
        {
            if (!IsSaving)
            {
                LoadLocal();
            }
            else
            {
                SaveLocal();
            }
        }
    }

    private void LoadGame(ISavedGameMetadata game)
    {
        ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(game, OnSavedGameDataRead);
    }

    private void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] data)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            string cloudData;
            if (data.Length == 0)
                cloudData = "500";
            else
                cloudData = GPGBytePackager.GetString(data);
            string localData = PlayerPrefs.GetString(SAVE_NAME);
            StringToGameData(localData, cloudData);
        }
    }

    private void SaveGame(ISavedGameMetadata game)
    {
        string stringToSave = GameDataToString();
        SaveLocal();
        var data = GPGBytePackager.CreatePackage(stringToSave);

        SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder().Build();
        ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(game, update,
            data, OnGameSaved);
    }

    private void OnGameSaved(SavedGameRequestStatus status, ISavedGameMetadata game)
    {

    }

    private void ResolveConflict(IConflictResolver resolver,
        ISavedGameMetadata original, byte[] originalData,
        ISavedGameMetadata unmerged, byte[] unmergetData)
    {
        Debug.Log("Resolving conflict");

        if (originalData == null)
        {
            Debug.Log("Getting unmerged");
            resolver.ChooseMetadata(unmerged);
        }
        else if (unmergetData == null)
        {
            Debug.Log("Getting original");
            resolver.ChooseMetadata(original);
        }
        else
        {
            var originalDataStr = GPGBytePackager.GetString(originalData);
            var unmergedDataStr = GPGBytePackager.GetString(unmergetData);
            Debug.Log("Comparing unmerged and original");
            if (int.Parse(originalDataStr) > int.Parse(unmergedDataStr))
            {
                resolver.ChooseMetadata(original);
                return;
            }
            else if (int.Parse(originalDataStr) > int.Parse(unmergedDataStr))
            {
                resolver.ChooseMetadata(unmerged);
                return;
            }
            resolver.ChooseMetadata(original);
        }
    }
    #endregion

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

    public void AddScoreToLeaderBoard(string leaderboardId, long credit)
    {
        Social.ReportScore(credit, leaderboardId, success => { });
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
            if (item.ParticipantId != PlayGamesPlatform.Instance.RealTime.GetSelf().ParticipantId)
            {
                id = item.ParticipantId;
                break;
            }
        }
        return id;
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
            player_two.isMyself = participants[1].ParticipantId == myself.ParticipantId;
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
        PlayGamesPlatform.Instance.RealTime.LeaveRoom();
    }

    public void OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data)
    {
        var message = GPGBytePackager.UnpackPackage(data);
        Debug.Log("I received message from: " + senderId + " ");
        GPGBytePackager.ProcessPackage(message);

    }
}
