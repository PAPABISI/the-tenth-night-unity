using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardHandPanel : MonoBehaviour
{
    [Header("Refs")]
    public GameStateStore store;
    public GameController controller;
    public Transform handRoot;            // HorizontalLayoutGroup 容器
    public Button cardButtonPrefab;       // 按钮预制体（含 TMP_Text）

    [Header("Auto Refresh")]
    public float refreshInterval = 0.5f;
    private float _timer;
    private string _lastSignature = "";

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < refreshInterval) return;
        _timer = 0f;

        RefreshIfChanged();
    }

    public void RefreshIfChanged()
    {
        var hand = store?.LatestState?.self?.hand;
        if (hand == null) return;

        string sig = BuildSignature(hand);
        if (sig == _lastSignature) return;

        _lastSignature = sig;
        RebuildHand(hand);
    }

    private string BuildSignature(List<CardClientView> hand)
    {
        return string.Join("|", hand.ConvertAll(c => $"{c.cardId}:{c.type}"));
    }

    private void RebuildHand(List<CardClientView> hand)
    {
        for (int i = handRoot.childCount - 1; i >= 0; i--)
            Destroy(handRoot.GetChild(i).gameObject);

        foreach (var c in hand)
        {
            var btn = Instantiate(cardButtonPrefab, handRoot);
            var txt = btn.GetComponentInChildren<TMP_Text>();
            // if (txt != null) txt.text = c.type;
            if (txt != null) txt.text = CardTypeName(c.type);

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnClickCard(c));
        }

        Debug.Log($"[HandPanel] handCount={hand.Count}");
    }

    private void OnClickCard(CardClientView card)
    {
        var publics = store?.LatestState?.publics;
        string targetId = null;

        if (publics != null)
        {
            foreach (var p in publics)
            {
                if (p.playerId != store.LocalPlayerId && p.isAlive)
                {
                    targetId = p.playerId;
                    break;
                }
            }
        }

        controller.UseCard(card.cardId, targetId);
    }

    private string CardTypeName(int t)
    {
        return t switch
        {
            1 => "Attack",
            2 => "Heal",
            3 => "Poison",
            4 => "Shield",
            _ => $"Card-{t}"
        };
    }
}