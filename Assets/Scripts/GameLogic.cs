using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    readonly Dictionary<int, GameObject> balloonsByID = new Dictionary<int, GameObject>();
    Sprite circleSprite;

    void Start()
    {
        NetworkClientProcessing.SetGameLogic(this);
    }

    // The server decides where and when balloons appear; the client only shows them.
    public void SpawnBalloon(int balloonID, Vector2 screenPercent)
    {
        if (balloonsByID.ContainsKey(balloonID))
            return;

        if (circleSprite == null)
            circleSprite = Resources.Load<Sprite>("Circle");

        GameObject balloon = new GameObject("Balloon " + balloonID);
        balloon.AddComponent<SpriteRenderer>().sprite = circleSprite;
        balloon.AddComponent<CircleClick>().BalloonID = balloonID;
        balloon.AddComponent<CircleCollider2D>();
        balloon.transform.position = ScreenPercentToWorldPosition(screenPercent);

        balloonsByID.Add(balloonID, balloon);
    }

    public void PopBalloon(int balloonID)
    {
        if (!balloonsByID.TryGetValue(balloonID, out GameObject balloon))
            return;

        Destroy(balloon);
        balloonsByID.Remove(balloonID);
    }

    // Used when the connection to the server is lost: the balloons on screen may no longer match the server.
    public void RemoveAllBalloons()
    {
        foreach (GameObject balloon in balloonsByID.Values)
            Destroy(balloon);
        balloonsByID.Clear();
    }

    static Vector3 ScreenPercentToWorldPosition(Vector2 screenPercent)
    {
        Vector3 screenPosition = new Vector3(screenPercent.x * Screen.width, screenPercent.y * Screen.height, 0);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0;
        return worldPosition;
    }
}
