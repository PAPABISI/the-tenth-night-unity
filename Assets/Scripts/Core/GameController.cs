using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class GameController : MonoBehaviour
{
    public ApiClient api;
    public GameStateStore store;
    public string localDisplayName = "Player";

    [Header("Polling")]
    public float pollInterval = 1.0f;
    private Coroutine pollRoutine;

    public void CreateRoom()
    {
        StartCoroutine(api.PostJson("/room/create", "{}", onOk: json =>
        {
            var res = JsonConvert.DeserializeObject<CreateRoomResponse>(json);
            store.RoomId = res.roomId;
            Debug.Log($"[CreateRoom] roomId={store.RoomId}");
        }, onErr: err =>
        {
            Debug.LogError("[CreateRoom] " + err);
        }));
    }

    public void StartGame()
    {
        if (string.IsNullOrEmpty(store.RoomId))
        {
            Debug.LogError("RoomId 为空，请先 CreateRoom");
            return;
        }

        if (string.IsNullOrEmpty(store.LocalPlayerId))
        {
            Debug.LogError("LocalPlayerId 为空，请先 JoinRoom");
            return;
        }

        var req = new StartGameRequest
        {
            playerIds = new List<string>()
        };

        string body = JsonConvert.SerializeObject(req);

        StartCoroutine(api.PostJson($"/room/{store.RoomId}/start", body, onOk: json =>
        {
            Debug.Log("[StartGame] OK: " + json);
            BeginPolling();
        }, onErr: err =>
        {
            Debug.LogError("[StartGame] " + err);
        }));
    }

    public void JoinRoom()
    {
        if (string.IsNullOrEmpty(store.RoomId))
        {
            Debug.LogError("RoomId 为空，请先 CreateRoom");
            return;
        }

        var req = new JoinRoomRequest { displayName = localDisplayName };
        string body = JsonConvert.SerializeObject(req);

        StartCoroutine(api.PostJson($"/room/{store.RoomId}/join", body, onOk: json =>
        {
            var res = JsonConvert.DeserializeObject<JoinRoomResponse>(json);
            store.LocalPlayerId = res.playerId;
            Debug.Log($"[JoinRoom] roomId={res.roomId}, playerId={res.playerId}, displayName={res.displayName}");
            BeginPolling();
        }, onErr: err =>
        {
            Debug.LogError("[JoinRoom] " + err);
        }));
    }

    public void BeginPolling()
    {
        if (pollRoutine != null) StopCoroutine(pollRoutine);
        pollRoutine = StartCoroutine(PollStateLoop());
    }

    public void StopPolling()
    {
        if (pollRoutine != null) StopCoroutine(pollRoutine);
        pollRoutine = null;
    }

    private IEnumerator PollStateLoop()
    {
        while (true)
        {
            if (!string.IsNullOrEmpty(store.RoomId) && !string.IsNullOrEmpty(store.LocalPlayerId))
            {
                yield return api.Get($"/room/{store.RoomId}/state/{store.LocalPlayerId}", onOk: json =>
                {
                    store.LatestState = JsonConvert.DeserializeObject<StateResponse>(json);
                    Debug.Log($"[State] round={store.LatestState.round}, phase={store.LatestState.phase}");
                }, onErr: err =>
                {
                    Debug.LogError("[State] " + err);
                });
            }

            yield return new WaitForSeconds(pollInterval);
        }
    }

    public void UseCard(string cardId, string targetId)
    {
        if (store.LatestState?.self == null) return;

        var req = new UseCardRequest
        {
            userId = store.LocalPlayerId,
            targetId = targetId,
            cardId = cardId
        };

        string body = JsonConvert.SerializeObject(req);

        StartCoroutine(api.PostJson($"/room/{store.RoomId}/action/use-card", body, onOk: json =>
        {
            Debug.Log("[UseCard] OK: " + json);
            StartCoroutine(RefreshStateCoroutine());
        }, onErr: err =>
        {
            Debug.LogError("[UseCard] " + err);
        }));
    }

    public void DrawCard(bool isRedChest)
    {
        if (string.IsNullOrEmpty(store.RoomId) || string.IsNullOrEmpty(store.LocalPlayerId))
        {
            Debug.LogWarning("[Draw] roomId/localPlayerId missing.");
            return;
        }

        var req = new DrawCardRequest
        {
            userId = store.LocalPlayerId,
            isRedChest = isRedChest
        };

        string body = JsonConvert.SerializeObject(req);
        StartCoroutine(api.PostJson($"/room/{store.RoomId}/action/draw", body, onOk: json =>
        {
            Debug.Log("[Draw] OK: " + json);
            StartCoroutine(RefreshStateCoroutine());
        }, onErr: err =>
        {
            Debug.LogError("[Draw] " + err);
        }));
    }

    public void SendMove(Vector2 position)
    {
        if (string.IsNullOrEmpty(store.RoomId) || string.IsNullOrEmpty(store.LocalPlayerId))
            return;

        var req = new MoveRequest
        {
            userId = store.LocalPlayerId,
            x = position.x,
            y = position.y
        };

        string body = JsonConvert.SerializeObject(req);
        StartCoroutine(api.PostJson($"/room/{store.RoomId}/action/move", body,
            onOk: _ => { },
            onErr: err =>
            {
                Debug.LogError("[Move] " + err);
            }));
    }

    // 按钮回调：推进阶段
    public void NextPhase()
    {
        if (string.IsNullOrEmpty(store.RoomId))
        {
            Debug.LogWarning("[NextPhase] roomId is empty.");
            return;
        }

        StartCoroutine(api.PostJson($"/room/{store.RoomId}/phase/next", "{}", onOk: json =>
        {
            Debug.Log("[NextPhase] OK: " + json);
            StartCoroutine(RefreshStateCoroutine());
        }, onErr: err =>
        {
            Debug.LogError("[NextPhase] " + err);
        }));
    }

    private IEnumerator RefreshStateCoroutine()
    {
        if (string.IsNullOrEmpty(store.RoomId) || string.IsNullOrEmpty(store.LocalPlayerId))
            yield break;

        yield return api.Get($"/room/{store.RoomId}/state/{store.LocalPlayerId}", onOk: sJson =>
        {
            store.LatestState = JsonConvert.DeserializeObject<StateResponse>(sJson);
            Debug.Log($"[State-Refresh] round={store.LatestState.round}, phase={store.LatestState.phase}");
        }, onErr: err =>
        {
            Debug.LogError("[State-Refresh] " + err);
        });
    }
}