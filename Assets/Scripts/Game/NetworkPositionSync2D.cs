using UnityEngine;

public class NetworkPositionSync2D : MonoBehaviour
{
    [Header("Refs")]
    public Transform localPlayer;
    public GameController controller;

    [Header("Sync")]
    public float sendInterval = 0.1f;
    public float minDistanceToSend = 0.03f;
    public bool sendWhenStationary = true;

    private float _timer;
    private Vector2 _lastSent;
    private bool _hasSent;

    private void Update()
    {
        if (localPlayer == null || controller == null) return;

        _timer += Time.deltaTime;
        if (_timer < sendInterval) return;
        _timer = 0f;

        var pos = (Vector2)localPlayer.position;
        if (!_hasSent)
        {
            controller.SendMove(pos);
            _lastSent = pos;
            _hasSent = true;
            return;
        }

        var moved = Vector2.Distance(_lastSent, pos) >= minDistanceToSend;
        if (moved || sendWhenStationary)
        {
            controller.SendMove(pos);
            _lastSent = pos;
        }
    }
}
