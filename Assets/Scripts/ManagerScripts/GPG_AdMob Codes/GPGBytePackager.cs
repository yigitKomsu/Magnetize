
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
        return System.Text.ASCIIEncoding.Default.GetBytes(final);
    }

    public static string[] UnpackPackage(byte[] data)
    {
        var dataPackage = System.Text.ASCIIEncoding.Default.GetString(data);
        string[] unpackedData = dataPackage.Split('/');
        return unpackedData;
    }

    public static void ProcessPackage(string[] package)
    {
        var actionType = package[0];
        UnityEngine.Debug.Log("Message received: " + package[0]);
        if(actionType == ProjectConstants.call)
        {
            Bet.OpCall();
        }
        else if(actionType == ProjectConstants.bet)
        {
            int amount = 0;
            int.TryParse(package[1], out amount);
            UnityEngine.Debug.Log("Calling and increasing: " + package[1]);
            Bet.OpCallAndIncrease(amount);
        }
        else if(actionType == ProjectConstants.satisfied)
        {
            UnityEngine.Debug.Log("Both devices are satisfied!!! Total bet is: " + Bet.TotalBet);
            //LevelManager.GetLevelManager.LoadLocalGame();
        }

        //we have an unpacked package of unknown length
        //we need to process it somehow
        //it contains numbers and words
    }
}
