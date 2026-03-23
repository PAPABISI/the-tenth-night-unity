using UnityEngine;
using TMPro;

public class InteractionDetector : MonoBehaviour
{
    [Header("Refs")]
    public Transform localPlayer;
    public TMP_Text promptText;
    public GameController controller;
    public GameStateStore store;

    [Header("Detect")]
    public float chestRange = 1.5f;
    public float playerRange = 1.5f;
    public LayerMask chestLayer;
    public LayerMask playerLayer;

    [Header("Draw")]
    public bool defaultIsRedChest = false;

    private void Update()
    {
        if (localPlayer == null) return;

        bool nearChest = Physics2D.OverlapCircle(localPlayer.position, chestRange, chestLayer);
        Collider2D nearPlayer = Physics2D.OverlapCircle(localPlayer.position, playerRange, playerLayer);

        // 提示文案
        if (promptText != null)
        {
            if (nearChest) promptText.text = "[F] Open Chest";
            else if (nearPlayer != null) promptText.text = "[E] Interact";
            else promptText.text = "";
        }

        // F 开箱抽卡
        if (Input.GetKeyDown(KeyCode.F))
        {
            TryDrawCard(defaultIsRedChest);
        }

        // E 互动（当前先做日志占位）
        if (nearPlayer != null && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("[Interact] E pressed near player. (voice/private chat hook point)");
        }
    }

    private void TryDrawCard(bool isRedChest)
    {
        if (string.IsNullOrEmpty(store.RoomId) || string.IsNullOrEmpty(store.LocalPlayerId))
        {
            Debug.LogWarning("[Draw] roomId/localPlayerId missing.");
            return;
        }

        var body = $"{{\"userId\":\"{store.LocalPlayerId}\",\"isRedChest\":{(isRedChest ? "true" : "false")}}}";
        StartCoroutine(controller.api.PostJson($"/room/{store.RoomId}/action/draw", body,
            onOk: json =>
            {
                Debug.Log("[Draw] OK: " + json);
            },
            onErr: err =>
            {
                Debug.LogError("[Draw] " + err);
            }));
    }

    private void OnDrawGizmosSelected()
    {
        if (localPlayer == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(localPlayer.position, chestRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(localPlayer.position, playerRange);
    }
}