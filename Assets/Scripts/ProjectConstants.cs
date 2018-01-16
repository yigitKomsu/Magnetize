using UnityEngine;

public class ProjectConstants
{
    public const string dropAndSpawnNumber = "numberdrop";
    public const string scoreNumber = "scorenumber";
    public const string updateScore = "updatescore";
    public const string updateCharge = "updatecharge";
    public const string updateTime = "updatetime";
    public const string bet = "bet";
    public const string call = "call";
    public const string satisfied = "satisfied";

    public const string WinText = "YOU WIN";
    public const string LoseText = "YOU LOSE";
    public const string TiedText = "TIED";
    public const string TimeText = "TIME: ";
    public const string TurnText = "YOUR TURN";
    public const string ScoreText = "SCORE: ";

    public const string MainMenu = "MainMenu";
    public const string CouchPlay = "CouchPlay";
    public const string Tutorial = "Tutorial";

    public const string AppId = "ca-app-pub-9092467960609637~5369739817";
    public const string BannerAdId = "ca-app-pub-9092467960609637/7045783564";
    public const string TestBannerAdId = "ca-app-pub-3940256099942544/6300978111";
    public const string VideoAdId = "ca-app-pub-9092467960609637/2061050197";
    public const string TestVideoAdId = "ca-app-pub-3940256099942544/5224354917";   

    public static int PlayerOne = 0;
    public static int PlayerTwo = 1;
    public static int Noone = 2;

    private static int userCredit;
    public static int tempUserCredit;
    public static int betAmount;

    public static bool UpdateUserCredit(int value)
    {
        int currentCredit = userCredit = PlayerPrefs.GetInt("Credit");
        currentCredit += value;
        if(currentCredit < 0)
        {
            return false;
        }
        PlayerPrefs.SetInt("Credit", currentCredit);
        Debug.Log("User credits: " + currentCredit);
        return true;
    }

    public static bool Bet(int value)
    {
        userCredit = PlayerPrefs.GetInt("Credit");
        if (tempUserCredit == 0) tempUserCredit = userCredit;

        if ((tempUserCredit -= value) > 0)
        {
            betAmount += value;
            return true;
        }

        return false;
    }

    public static void RestoreUserCredit()
    {
        PlayerPrefs.SetInt("Credit", userCredit);
        Debug.Log("User credits: " + userCredit);
    }
}

public enum PowerType
{
    None = -1,
    DoubleScore = 0,
    MagnetizerBomb = 1,
    Refill = 2
}

public enum LimitTypes
{
    Charge = 0,
    Time = 1,
    Score = 2
}
