using UnityEngine;
using TMPro;

public class PhaseFlowController : MonoBehaviour
{
    [Header("Refs")]
    public GameController controller;
    public GameStateStore store;
    public TMP_Text phaseInfoText;   // 可选

    [Header("Auto Poll UI")]
    public float refreshInterval = 0.3f;
    private float _timer;

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= refreshInterval)
        {
            _timer = 0f;
            RefreshPhaseInfo();
        }

        // 快捷键 N：推进阶段
        if (Input.GetKeyDown(KeyCode.N))
        {
            NextPhase();
        }
    }

    public void NextPhase()
    {
        controller.NextPhase();
    }

    public void RefreshPhaseInfo()
    {
        var s = store?.LatestState;
        if (s == null || phaseInfoText == null) return;

        phaseInfoText.text = $"Round {s.round} / Phase: {s.phase}";
    }
}