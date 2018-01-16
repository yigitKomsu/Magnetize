public class Bet
{
    public static int YourBet = 0;
    public static int OpBet = 0;
    public const int TableBet = 10;
    public static bool myTurn = false;
    public static int TotalBet { get { return YourBet + OpBet + TableBet; } }

    public static int Call()
    {
        if (!myTurn) return 0;

        myTurn = false;
        if (YourBet == 0 && OpBet == 0)
        {
            YourBet += TableBet;
            var data = new string[]
            {
                ProjectConstants.call
            };
            GPGController.SendByteMessage(GPGBytePackager.CreatePackage(data),
                GPGController.GetOpponentId());
            LevelManager.GetLevelManager.BetText();
            return TableBet;
        }
        else if (YourBet < OpBet)
        {
            int difference = OpBet - YourBet;
            YourBet += difference;
            var data = new string[]
            {
                ProjectConstants.call
            };
            GPGController.SendByteMessage(GPGBytePackager.CreatePackage(data),
                GPGController.GetOpponentId());
            LevelManager.GetLevelManager.BetText();
            return difference;
        }
        return 0;
    }

    public static int CallAndIncrease(int increaseAmount)
    {
        if (!myTurn) return 0;

        myTurn = false;
        if (YourBet == 0 && OpBet == 0)
        {
            YourBet += TableBet;
        }
        else if (YourBet < OpBet)
        {
            int difference = OpBet - YourBet;
            YourBet += difference;
        }
        YourBet += increaseAmount;
        var data = new string[]
        {
            ProjectConstants.bet,
            increaseAmount.ToString()
        };
        GPGController.SendByteMessage(GPGBytePackager.CreatePackage(data),
            GPGController.GetOpponentId());
        LevelManager.GetLevelManager.BetText();
        return increaseAmount;
    }

    public static int OpCall()
    {
        if (YourBet == 0 && OpBet == 0)
        {
            OpBet += TableBet;
            LevelManager.GetLevelManager.BetText();
            myTurn = true;
            return TableBet;
        }
        else if (YourBet > OpBet)
        {
            int difference = YourBet - OpBet;
            OpBet += difference;
            LevelManager.GetLevelManager.BetText();
            myTurn = true;
            if (IsSatisfied())
            {
                var msg = new string[]
                {
                ProjectConstants.satisfied
                };
                GPGController.SendByteMessage(GPGBytePackager.CreatePackage(msg), GPGController.GetOpponentId());
                UnityEngine.Debug.Log("Both devices are satisfied!!! Total bet is: " + Bet.TotalBet);
                LevelManager.GetLevelManager.LoadLocalGame();
            }
            return difference;
        }
        
        return 0;
    }

    public static int OpCallAndIncrease(int increaseAmount)
    {
        if (OpBet == 0 && YourBet == 0)
        {
            OpBet += TableBet;
        }
        else if (OpBet < YourBet)
        {
            int difference = YourBet - OpBet;
            OpBet += difference;
        }
        OpBet += increaseAmount;
        myTurn = true;
        LevelManager.GetLevelManager.BetText();
        return increaseAmount;

    }

    public static bool IsSatisfied()
    {
        return YourBet == OpBet;
    }
}
