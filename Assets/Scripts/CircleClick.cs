using UnityEngine;

public class CircleClick : MonoBehaviour
{
    public int BalloonID { get; set; }

    // The balloon isn't destroyed here: the server confirms the pop and tells every client, including this one.
    void OnMouseDown()
    {
        NetworkClientProcessing.SendPopBalloonRequest(BalloonID);
    }
}
