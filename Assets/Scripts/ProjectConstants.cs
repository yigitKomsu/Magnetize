﻿using UnityEngine;

public class ProjectConstants
{
    public const string message_played = "numberdrop";
    public const string message_spawnTile = "spawntile";
    public const string message_scored = "scorenumber";
    public const string updateScore = "updatescore";
    public const string message_chargeUpdate = "updatecharge";
    public const string message_updateTime = "updatetime";
    public const string message_bet = "bet";
    public const string message_call = "call";
    public const string message_satisfied = "satisfied";
    public const string message_tied = "tied";
    public const string message_youwon = "youwon";
    public const string message_double = "double";
    public const string message_spawnPowerUp = "spawnpowerup";
    public const string message_magnet = "magnet";
    public const string message_refill = "refill";
    public const string message_youlost = "youlost";

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
    
    public static int WinRowLimit_TierTwo = 5;
    public static int WinRowLimit_TierOne = 3;
    public static int LoseRowLimit_TierOne = 3;
    public static int LoseRowLimit_TierTwo = 5;
    public static int PowerUpUse_TierOne = 5;
    public static int PowerUpUse_TierTwo = 10;
    public static int PowerUpUse_TierThree = 100;
    public static int MagnetizedRow_TierOne = 5;
    public static int MagnetizedRow_TierTwo = 9;
    public static int CollectRow_TierOne = 5;
    public static int CollectRow_TierTwo = 9;

    public static int PlayerOne = 0;
    public static int PlayerTwo = 1;
    public static int Noone = 2;

    public static int userCredit = PlayerPrefs.HasKey("Credit") ? 
        PlayerPrefs.GetInt("Credit") : 500;
    public static int tempUserCredit;
    public static int betAmount;

    public static void UpdateUserCredit(int value)
    {
        userCredit += value;
    }

    public static void RestoreUserCredit()
    {

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
