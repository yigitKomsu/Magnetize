public class Bet
{
    public static int YourBet = 0;
    public static int OpBet = 0;
    public static int TableBet = 10; //default
    public static bool myTurn = false;
    public static int TotalBet { get { return YourBet + OpBet + TableBet; } }

    public static int Call()
    {
        if (!myTurn) return 0;
        LevelManager.GetLevelManager.ToggleLight(myTurn);

        myTurn = false;
        if ((YourBet == 0 && OpBet == 0) && ProjectConstants.userCredit - TableBet > 0)
        {
            YourBet += TableBet;
            ProjectConstants.userCredit -= TableBet;
            var data = new string[]
            {
                ProjectConstants.message_call
            };
            GPGController.SendByteMessage(GPGBytePackager.CreatePackage(data),
                GPGController.GetOpponentId());
            LevelManager.GetLevelManager.BetEventText("You called " + TableBet);
            LevelManager.GetLevelManager.BetText();
            return TableBet;
        }
        else if ((YourBet < OpBet) && ProjectConstants.userCredit - (OpBet - YourBet) > 0)
        {
            int difference = OpBet - YourBet;
            YourBet += difference;
            ProjectConstants.userCredit -= difference;
            var data = new string[]
            {
                ProjectConstants.message_call
            };
            GPGController.SendByteMessage(GPGBytePackager.CreatePackage(data),
                GPGController.GetOpponentId());
            LevelManager.GetLevelManager.BetEventText("You called " + TableBet);
            LevelManager.GetLevelManager.BetText();
            return difference;
        }
        return 0;
    }

    public static int CallAndIncrease(int increaseAmount)
    {
        if (!myTurn) return 0;
        LevelManager.GetLevelManager.ToggleLight(myTurn);
        myTurn = false;
        
        if ((YourBet == 0 && OpBet == 0) && 
            ProjectConstants.userCredit - TableBet - increaseAmount > 0)
        {
            YourBet += TableBet;
            ProjectConstants.userCredit -= TableBet;
            YourBet += increaseAmount;
            ProjectConstants.userCredit -= increaseAmount;
            var data = new string[]
        {
            ProjectConstants.message_bet,
            increaseAmount.ToString()
        };
            GPGController.SendByteMessage(GPGBytePackager.CreatePackage(data),
                GPGController.GetOpponentId());
            LevelManager.GetLevelManager.BetEventText("You called " + TableBet);
            LevelManager.GetLevelManager.BetText();
        }
        else if ((YourBet < OpBet) && 
            ProjectConstants.userCredit - (OpBet - YourBet) - increaseAmount > 0)
        {
            int difference = OpBet - YourBet;
            YourBet += difference;
            ProjectConstants.userCredit -= difference;
            YourBet += increaseAmount;
            ProjectConstants.userCredit -= increaseAmount;
            var data = new string[]
        {
            ProjectConstants.message_bet,
            increaseAmount.ToString()
        };
            GPGController.SendByteMessage(GPGBytePackager.CreatePackage(data),
                GPGController.GetOpponentId());
            LevelManager.GetLevelManager.BetEventText("You called " + difference);
            LevelManager.GetLevelManager.BetText();
        }        
        
        return increaseAmount;
    }

    public static int OpCall()
    {
        LevelManager.GetLevelManager.ToggleLight(myTurn);

        if (YourBet == 0 && OpBet == 0)
        {
            OpBet += TableBet;
            LevelManager.GetLevelManager.BetEventText(GPGController.GetOpponentName() + " called " + TableBet);
            LevelManager.GetLevelManager.BetText();
            myTurn = true;
            return TableBet;
        }
        else if (YourBet > OpBet)
        {
            int difference = YourBet - OpBet;
            OpBet += difference;
            LevelManager.GetLevelManager.BetText();
            LevelManager.GetLevelManager.BetEventText(GPGController.GetOpponentName() + " called  " + difference);

            myTurn = true;
            if (IsSatisfied())
            {
                LevelManager.GetLevelManager.BetEventText("Game is starting".ToUpper());
                var msg = new string[]
                {
                ProjectConstants.message_satisfied
                };
                GPGController.SendByteMessage(GPGBytePackager.CreatePackage(msg), GPGController.GetOpponentId());
                UnityEngine.Debug.Log("Both devices are satisfied!!! Total bet is: " + Bet.TotalBet);
                LevelManager.GetLevelManager.LoadOnlineGame();
            }
            return difference;
        }

        return 0;
    }

    public static int OpCallAndIncrease(int increaseAmount)
    {
        LevelManager.GetLevelManager.ToggleLight(myTurn);

        int difference = 0;
        if (OpBet == 0 && YourBet == 0)
        {
            OpBet += TableBet;
        }
        else if (OpBet < YourBet)
        {
            difference = YourBet - OpBet;
            OpBet += difference;
        }
        OpBet += increaseAmount;
        myTurn = true;
        LevelManager.GetLevelManager.BetText();
        LevelManager.GetLevelManager.BetEventText(GPGController.GetOpponentName() +
            " called " + difference + " and increased the bet by " + increaseAmount);
        return increaseAmount;

    }

    public static bool IsSatisfied()
    {
        return YourBet == OpBet;
    }
}
