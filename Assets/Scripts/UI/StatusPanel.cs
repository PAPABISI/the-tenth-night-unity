using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatusPanel : MonoBehaviour
{
    [Header("Refs")]
    public GameStateStore store;

    [Header("UI")]
    public TMP_Text nameText;
    public TMP_Text roleText;        // 先显示 Unknown（后端暂未给身份时）
    public TMP_Text phaseText;
    public TMP_Text hpText;
    public TMP_Text apText;
    public Slider hpSlider;          // 可选，不拖也行

    [Header("Auto Refresh")]
    public float refreshInterval = 0.3f;
    private float _timer;

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < refreshInterval) return;
        _timer = 0f;

        RefreshNow();
    }

    public void RefreshNow()
    {
        var s = store?.LatestState;
        var self = s?.self;
        if (self == null) return;

        if (nameText) nameText.text = $"Player: {self.displayName}";
        if (phaseText) phaseText.text = $"Phase: {s.phase}";
        if (hpText) hpText.text = $"HP: {self.currentHp}/{self.maxHp}";
        if (apText) apText.text = $"AP: {self.currentAp}";
        if (roleText) roleText.text = $"Role: {(string.IsNullOrEmpty(self.role) ? "Unknown" : self.role)}";

        if (hpSlider != null)
        {
            hpSlider.maxValue = Mathf.Max(1, self.maxHp);
            hpSlider.value = Mathf.Clamp(self.currentHp, 0, self.maxHp);
        }
    }
}