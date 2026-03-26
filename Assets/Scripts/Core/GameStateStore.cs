using UnityEngine;

public class GameStateStore : MonoBehaviour
{
    [Header("Runtime IDs")]
    public string RoomId;
    public string LocalPlayerId;

    [Header("Latest Snapshot")]
    public StateResponse LatestState;

    [Header("Debug")]
    public bool autoSetLocalPlayerIdOnStart = false;
    public string fallbackLocalPlayerId = "11111111-1111-1111-1111-111111111111";

    private void Awake()
    {
        // 最小联调用：如果你还没做 join/player 选择，就固定一个本地玩家ID
        if (autoSetLocalPlayerIdOnStart && string.IsNullOrEmpty(LocalPlayerId))
            LocalPlayerId = fallbackLocalPlayerId;
    }

    public void SetRoomId(string roomId) => RoomId = roomId;
    public void SetLocalPlayerId(string playerId) => LocalPlayerId = playerId;
    public void SetLatestState(StateResponse state) => LatestState = state;

    public bool HasRoomAndPlayer()
        => !string.IsNullOrEmpty(RoomId) && !string.IsNullOrEmpty(LocalPlayerId);

    public void ClearRuntime()
    {
        RoomId = null;
        LocalPlayerId = null;
        LatestState = null;
    }
}