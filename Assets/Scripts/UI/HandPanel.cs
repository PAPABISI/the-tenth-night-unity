using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandPanel : MonoBehaviour
{
    public GameStateStore store;
    public GameController controller;

    [Header("UI")]
    public Transform handRoot;
    public Button cardButtonPrefab;
    public TMP_Dropdown targetDropdown;   // 新增：目标下拉框
    public TextMeshProUGUI tipText;

    private readonly List<Button> spawned = new();
    private readonly List<string> targetIds = new(); // dropdown index -> playerId
    private string lastSignature = "";

    void Update()
    {
        RefreshTargetsIfNeeded();
        RefreshHandIfChanged();
    }

    void RefreshTargetsIfNeeded()
    {
        var s = store?.LatestState;
        if (s?.publics == null || targetDropdown == null) return;

        // 生成目标签名，避免每帧重建
        string sig = "targets";
        foreach (var p in s.publics)
            sig += $"|{p.playerId}:{p.isAlive}:{p.displayName}";

        // 利用 lastSignature 的前缀区分（简单做法）
        string targetSigKey = "[T]" + sig;
        if (PlayerPrefs.GetString("HAND_TARGET_SIG", "") == targetSigKey) return;
        PlayerPrefs.SetString("HAND_TARGET_SIG", targetSigKey);

        targetDropdown.ClearOptions();
        targetIds.Clear();

        var options = new List<TMP_Dropdown.OptionData>();

        foreach (var p in s.publics)
        {
            if (p.playerId == store.LocalPlayerId) continue;
            if (!p.isAlive) continue;

            options.Add(new TMP_Dropdown.OptionData($"{p.displayName}"));
            targetIds.Add(p.playerId);
        }

        if (options.Count == 0)
        {
            options.Add(new TMP_Dropdown.OptionData("无可用目标"));
        }

        targetDropdown.AddOptions(options);
        targetDropdown.value = 0;
        targetDropdown.RefreshShownValue();
    }

    void RefreshHandIfChanged()
    {
        var s = store?.LatestState;

        if (s == null)
        {
            if (tipText) tipText.text = "State 为空（还没拉到状态）";
            return;
        }

        if (s.self == null)
        {
            if (tipText) tipText.text = "Self 为空（检查 playerId / 后端state）";
            return;
        }

        if (s.self.hand == null)
        {
            if (tipText) tipText.text = "Hand 为 null（后端未返回手牌数组）";
            return;
        }

        Debug.Log($"[HandPanel] phase={s.phase}, handCount={s.self.hand.Count}");

        string sig = $"{s.round}|{s.phase}|{s.self.hand.Count}";
        foreach (var c in s.self.hand) sig += $"|{c.cardId}:{c.type}";
        if (sig == lastSignature) return;
        lastSignature = sig;

        ClearButtons();

        if (s.self.hand.Count == 0)
        {
            if (tipText) tipText.text = "当前没有手牌";
            return;
        }

        foreach (var card in s.self.hand)
        {
            var btn = Instantiate(cardButtonPrefab, handRoot);
            spawned.Add(btn);

            var txt = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null) txt.text = $"{card.type}\n{card.cardId}";
            else Debug.LogWarning("[HandPanel] 按钮里没找到 TextMeshProUGUI 子组件");

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                string targetId = GetSelectedTargetId();
                if (string.IsNullOrEmpty(targetId))
                {
                    if (tipText) tipText.text = "没有可用目标";
                    return;
                }

                controller.UseCard(card.cardId, targetId);
                if (tipText) tipText.text = $"使用 {card.type} -> {GetSelectedTargetName()}";
            });
        }

        if (tipText) tipText.text = $"已生成 {spawned.Count} 张手牌按钮";
    }

    string GetSelectedTargetId()
    {
        if (targetDropdown == null) return null;
        if (targetIds.Count == 0) return null;

        int idx = targetDropdown.value;
        if (idx < 0 || idx >= targetIds.Count) return null;
        return targetIds[idx];
    }

    string GetSelectedTargetName()
    {
        if (targetDropdown == null) return "";
        if (targetDropdown.options == null || targetDropdown.options.Count == 0) return "";
        int idx = targetDropdown.value;
        if (idx < 0 || idx >= targetDropdown.options.Count) return "";
        return targetDropdown.options[idx].text;
    }

    void ClearButtons()
    {
        foreach (var b in spawned)
        {
            if (b) Destroy(b.gameObject);
        }
        spawned.Clear();
    }
}