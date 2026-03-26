using UnityEngine;
using TMPro;
using System.Linq;

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
    public UiToast toast;

    [Header("Draw")]
    public bool defaultIsRedChest = false;

    private void Update()
    {
        if (localPlayer == null) return;

        var nearChest = FindNearest(localPlayer.position, chestRange, chestLayer);
        var nearPlayer = FindNearest(localPlayer.position, playerRange, playerLayer);
        var phase = store?.LatestState?.phase;

        if (promptText != null)
        {
            if (nearChest != null && phase == "DayExploration") promptText.text = "[F] Open Chest";
            else if (nearPlayer != null) promptText.text = "[E] Interact Player";
            else promptText.text = "";
        }

        if (Input.GetKeyDown(KeyCode.F) && nearChest != null)
        {
            if (phase != "DayExploration")
            {
                ShowToast("You can draw only in DayExploration.");
            }
            else
            {
                controller.DrawCard(defaultIsRedChest);
                ShowToast(defaultIsRedChest ? "Draw from red chest" : "Draw from white chest");
            }
        }

        if (nearPlayer != null && Input.GetKeyDown(KeyCode.E))
        {
            var targetName = nearPlayer.name;
            ShowToast($"Interact with {targetName}");
            Debug.Log($"[Interact] E pressed near player: {targetName}");
        }
    }

    private Collider2D FindNearest(Vector2 origin, float range, LayerMask mask)
    {
        var hits = Physics2D.OverlapCircleAll(origin, range, mask);
        if (hits == null || hits.Length == 0) return null;

        return hits
            .OrderBy(h => Vector2.Distance(origin, h.transform.position))
            .FirstOrDefault();
    }

    private void ShowToast(string msg)
    {
        if (toast != null)
            toast.Show(msg, 1.2f);
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