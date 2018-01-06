using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

public class GPGController
{
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
}
