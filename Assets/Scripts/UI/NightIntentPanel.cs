using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NightIntentPanel : MonoBehaviour
{
    [Header("Refs")]
    public GameStateStore store;
    public GameController controller;

    [Header("UI")]
    public GameObject root;           // 整个弹窗根节点
    public TMP_Text titleText;
    public Button btnSteal;           // “偷宝”
    public Button btnNotSteal;        // “不偷”

    [Header("Hotkey")]
    public KeyCode openKey = KeyCode.Q;

    private bool _submittedThisNight = false;
    private int _lastRound = -1;
    private string _lastPhase = "";

    private void Start()
    {
        if (root != null) root.SetActive(false);

        btnSteal.onClick.AddListener(() => Submit(true));
        btnNotSteal.onClick.AddListener(() => Submit(false));
    }

    private void Update()
    {
        var st = store?.LatestState;
        if (st == null) return;

        // 回合/阶段变化时重置“本夜已提交”标记
        if (st.round != _lastRound || st.phase != _lastPhase)
        {
            _lastRound = st.round;
            _lastPhase = st.phase;

            if (st.phase == "NightPhase")
                _submittedThisNight = false;

            if (st.phase != "NightPhase" && root != null && root.activeSelf)
                root.SetActive(false);
        }

        if (st.phase != "NightPhase") return;
        if (_submittedThisNight) return;

        // 夜晚按 E 打开弹窗
        if (Input.GetKeyDown(openKey))
        {
            if (root != null) root.SetActive(true);
            if (titleText != null) titleText.text = "Night Action: Steal treasure?";
        }
    }

    private void Submit(bool intendToSteal)
    {
        if (_submittedThisNight) return;
        if (store == null || controller == null) return;
        if (string.IsNullOrEmpty(store.RoomId) || string.IsNullOrEmpty(store.LocalPlayerId)) return;

        string body = $"{{\"userId\":\"{store.LocalPlayerId}\",\"intendToSteal\":{(intendToSteal ? "true" : "false")}}}";

        StartCoroutine(controller.api.PostJson(
            $"/room/{store.RoomId}/action/night-intent",
            body,
            onOk: json =>
            {
                _submittedThisNight = true;
                Debug.Log("[NightIntent] OK: " + json);
                if (root != null) root.SetActive(false);
            },
            onErr: err =>
            {
                Debug.LogError("[NightIntent] " + err);
                // 出错时不锁死，允许重试
                _submittedThisNight = false;
            }
        ));
    }
}