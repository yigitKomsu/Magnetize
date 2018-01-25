﻿
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
        UnityEngine.Debug.Log("Message received: " + package[0]);
        if(actionType == ProjectConstants.message_call)
        {
            Bet.OpCall();
        }
        else if(actionType == ProjectConstants.message_bet)
        {
            int amount = 0;
            int.TryParse(package[1], out amount);
            UnityEngine.Debug.Log("Calling and increasing: " + package[1]);
            Bet.OpCallAndIncrease(amount);
        }
        else if(actionType == ProjectConstants.message_satisfied)
        {
            UnityEngine.Debug.Log("Both devices are satisfied!!! Total bet is: " + Bet.TotalBet);
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
        //we have an unpacked package of unknown length
        //we need to process it somehow
        //it contains numbers and words
    }
}
