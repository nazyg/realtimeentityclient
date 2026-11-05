using System.Globalization;
using UnityEngine;

static public class NetworkClientProcessing
{

    #region Send and Receive Data Functions
    static public void ReceivedMessageFromServer(string msg, TransportPipeline pipeline)
    {
        Debug.Log("Network msg received =  " + msg + ", from pipeline = " + pipeline);

        string[] csv = msg.Split(',');
        if (!int.TryParse(csv[0], out int signifier))
            return;

        if (signifier == ServerToClientSignifiers.SpawnBalloon)
        {
            if (csv.Length == 4 && int.TryParse(csv[1], out int balloonID)
                && TryParsePercent(csv[2], out float xPercent) && TryParsePercent(csv[3], out float yPercent))
                gameLogic.SpawnBalloon(balloonID, new Vector2(xPercent, yPercent));
        }
        else if (signifier == ServerToClientSignifiers.BalloonPopped)
        {
            if (csv.Length == 2 && int.TryParse(csv[1], out int balloonID))
                gameLogic.PopBalloon(balloonID);
        }
    }

    static bool TryParsePercent(string value, out float percent)
    {
        return float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out percent);
    }

    static public void SendMessageToServer(string msg, TransportPipeline pipeline)
    {
        networkClient.SendMessageToServer(msg, pipeline);
    }

    static public void SendPopBalloonRequest(int balloonID)
    {
        if (IsConnectedToServer())
            SendMessageToServer(ClientToServerSignifiers.PopBalloonRequest + "," + balloonID, TransportPipeline.ReliableAndInOrder);
    }

    #endregion

    #region Connection Related Functions and Events
    static public void ConnectionEvent()
    {
        Debug.Log("Network Connection Event!");
    }
    static public void DisconnectionEvent()
    {
        Debug.Log("Network Disconnection Event!");
        gameLogic.RemoveAllBalloons();
    }
    static public bool IsConnectedToServer()
    {
        return networkClient.IsConnected();
    }
    static public void ConnectToServer()
    {
        networkClient.Connect();
    }
    static public void DisconnectFromServer()
    {
        networkClient.Disconnect();
    }

    #endregion

    #region Setup
    static NetworkClient networkClient;
    static GameLogic gameLogic;

    static public void SetNetworkedClient(NetworkClient NetworkClient)
    {
        networkClient = NetworkClient;
    }
    static public NetworkClient GetNetworkedClient()
    {
        return networkClient;
    }
    static public void SetGameLogic(GameLogic GameLogic)
    {
        gameLogic = GameLogic;
    }

    #endregion

}

#region Protocol Signifiers
static public class ClientToServerSignifiers
{
    public const int PopBalloonRequest = 1;     // 1,balloonID
}

static public class ServerToClientSignifiers
{
    public const int SpawnBalloon = 1;          // 1,balloonID,xPercent,yPercent
    public const int BalloonPopped = 2;         // 2,balloonID
}

#endregion
