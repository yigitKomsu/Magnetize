
public class GPGBytePackager
{
    public static byte[] CreatePackage(string[] data)
    {
        string final = "";
        foreach (var item in data)
        {
            final += item + "/";
        }
        UnityEngine.Debug.Log(final);
        return System.Text.Encoding.ASCII.GetBytes(final);
    }

    public static byte[] CreatePackage(string data)
    {
        return System.Text.Encoding.ASCII.GetBytes(data);
    }

    public static string[] UnpackPackage(byte[] data)
    {
        var dataPackage = System.Text.Encoding.ASCII.GetString(data);
        string[] unpackedData = dataPackage.Split('/');
        return unpackedData;
    }

    public static string GetString(byte[] data)
    {
        var dataPackage = System.Text.Encoding.ASCII.GetString(data);
        return dataPackage;
    }

    public static void ProcessPackage(string[] package)
    {
        var actionType = package[0];
        if(actionType == ProjectConstants.message_call)
        {
            Bet.OpCall();
        }
        else if(actionType == ProjectConstants.message_bet)
        {
            int amount = 0;
            int.TryParse(package[1], out amount);
            Bet.OpCallAndIncrease(amount);
        }
        else if(actionType == ProjectConstants.message_satisfied)
        {
            LevelManager.GetLevelManager.LoadOnlineGame();
        }
        else if(actionType == ProjectConstants.message_played)
        {
            int pos_x = int.Parse(package[1]);
            int pos_y = int.Parse(package[2]);
            int cardNumber = int.Parse(package[3]);
            GameManager.GetGameManager.SpawnCardFromPeer(cardNumber, -(pos_x - 2), -(pos_y - 2));
        }
        else if(actionType == ProjectConstants.message_spawnTile)
        {
            int number = int.Parse(package[1]);
            int index = int.Parse(package[2]);
            GameManager.GetGameManager.UpdateOnlineOpponentCard(index, number);
        }
        else if(actionType == ProjectConstants.message_updateTime)
        {
            int time = int.Parse(package[1]);
            TurnManager.GetTurnManager.SetTime(time, 0);
        }
        else if(actionType == ProjectConstants.message_chargeUpdate)
        {
            int charge = int.Parse(package[1]);
            GameManager.GetGameManager.UpdateScoreHandlerCharge(charge, 1);
        }
        //else if(actionType == ProjectConstants.message_double)
        //{

        //    UnityEngine.Debug.Log("Message received: " + package[0]);
        //    int row = int.Parse(package[1]);
        //    int col = int.Parse(package[2]);
        //    GameManager.GetGameManager.BoardHandler.GameBoardMatrix.row[4 - row].pColumn[4 - col].
        //        PowerThings(GameManager.GetGameManager.BoardHandler.
        //        GameBoardMatrix.row[4 - row].column[4 - col], 4 - row, 4 - col, true);
        //}
        //else if (actionType == ProjectConstants.message_magnet)
        //{
        //    UnityEngine.Debug.Log("Message received: " + package[0]);
        //    int row = int.Parse(package[1]);
        //    int col = int.Parse(package[2]);
        //    GameManager.GetGameManager.BoardHandler.GameBoardMatrix.row[4 - row].pColumn[4 - col].
        //        PowerThings(GameManager.GetGameManager.BoardHandler.
        //        GameBoardMatrix.row[4 - row].column[4 - col], 4 - row, 4 - col, true);
        //}
        //else if (actionType == ProjectConstants.message_refill)
        //{
        //    UnityEngine.Debug.Log("Message received: " + package[0]);
        //    int row = int.Parse(package[1]);
        //    int col = int.Parse(package[2]);
        //    UnityEngine.Debug.Log(GameManager.GetGameManager.BoardHandler.GameBoardMatrix);
        //    UnityEngine.Debug.Log(GameManager.GetGameManager.BoardHandler.GameBoardMatrix.row[4 - row]);
        //    UnityEngine.Debug.Log(GameManager.GetGameManager.BoardHandler.GameBoardMatrix.row[4 - row].pColumn[4 - col]);
        //    GameManager.GetGameManager.BoardHandler.GameBoardMatrix.row[4 - row].pColumn[4 - col].
        //        PowerThings(GameManager.GetGameManager.BoardHandler.
        //        GameBoardMatrix.row[4 - row].column[4 - col], 4 - row, 4 - col, true);
        //}
        else if(actionType == ProjectConstants.message_spawnPowerUp)
        {
            UnityEngine.Debug.Log("Message received: " + package[0]);
            string powerUpType = package[1];
            int row = int.Parse(package[2]);
            int col = int.Parse(package[3]);
            if(powerUpType == PowerType.DoubleScore.ToString())
            {
                GameManager.GetGameManager.BoardHandler.SpawnPowerUp(-row, -col, (int)PowerType.DoubleScore);
            }
            else if (powerUpType == PowerType.MagnetizerBomb.ToString())
            {
                GameManager.GetGameManager.BoardHandler.SpawnPowerUp(-row, -col, (int)PowerType.MagnetizerBomb);
            }
            else if (powerUpType == PowerType.Refill.ToString())
            {
                GameManager.GetGameManager.BoardHandler.SpawnPowerUp(-row, -col, (int)PowerType.Refill);
            }
        }
    }
}
