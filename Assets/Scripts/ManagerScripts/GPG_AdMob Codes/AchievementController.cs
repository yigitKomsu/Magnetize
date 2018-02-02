

public class AchievementController
{

    public static int WinsInARow { get; set; }
    public static int LosesInARow { get; set; }

    public static int PowerUpUseCount { get; set; }

    public static int MagnetizedTileCount { get; set; }
    public static int CollectedTileCount { get; set; }

    public static void WinLoseAchievements(bool isVictory, bool isScore)
    {
        PowerUpUseCount = 0;
        if (isVictory)
        {
            LosesInARow = 0;
            if (isScore)
                GPGController.UnlockAchievement(GPGSIds.achievement_scoring_up);
            else
                GPGController.UnlockAchievement(GPGSIds.achievement_durable_batteries);

            GPGController.IncrementAchievement(GPGSIds.achievement_winner_of_the_internet, 20);
            GPGController.IncrementAchievement(GPGSIds.achievement_champion_of_the_internet, 10);
            WinsInARow++;
            if (WinsInARow == ProjectConstants.WinRowLimit_TierOne)
            {
                GPGController.UnlockAchievement(GPGSIds.achievement_gamer);
            }
            else if (WinsInARow == ProjectConstants.WinRowLimit_TierTwo)
            {
                GPGController.UnlockAchievement(GPGSIds.achievement_esports_player);
            }
        }
        else
        {
            WinsInARow = 0;
            LosesInARow++;
            if (LosesInARow == ProjectConstants.LoseRowLimit_TierOne)
            {
                GPGController.UnlockAchievement(GPGSIds.achievement_loser);
            }
            else if (LosesInARow == ProjectConstants.LoseRowLimit_TierTwo)
            {
                GPGController.UnlockAchievement(GPGSIds.achievement_pathetic);
            }
        }
    }

    public static void PowerUpCounter()
    {
        PowerUpUseCount++;
        if(PowerUpUseCount == ProjectConstants.PowerUpUse_TierOne)
        {
            GPGController.UnlockAchievement(GPGSIds.achievement_opportunist);
        }
        else if(PowerUpUseCount == ProjectConstants.PowerUpUse_TierTwo)
        {
            GPGController.UnlockAchievement(GPGSIds.achievement_power);
        }
        else if (PowerUpUseCount == ProjectConstants.PowerUpUse_TierThree)
        {
            GPGController.UnlockAchievement(GPGSIds.achievement_unlimited_power);
        }
    }

    public static void MagnetizingRow()
    {
        MagnetizedTileCount++;
        if(MagnetizedTileCount == ProjectConstants.MagnetizedRow_TierOne)
        {
            GPGController.UnlockAchievement(GPGSIds.achievement_attractive_gameplay);
        }
        if (MagnetizedTileCount == ProjectConstants.MagnetizedRow_TierTwo)
        {
            GPGController.UnlockAchievement(GPGSIds.achievement_magnetize);
        }
    }

    public static void CollectingTiles()
    {
        CollectedTileCount++;
        if (CollectedTileCount == ProjectConstants.CollectRow_TierOne)
        {
            GPGController.UnlockAchievement(GPGSIds.achievement_whole_row);
        }
        if (CollectedTileCount == ProjectConstants.CollectRow_TierTwo)
        {
            GPGController.UnlockAchievement(GPGSIds.achievement_a_fine_addition);
        }
    }

}
