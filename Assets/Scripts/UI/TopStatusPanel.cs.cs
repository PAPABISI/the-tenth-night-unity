using TMPro;
using UnityEngine;

public class TopStatusPanel : MonoBehaviour
{
    public GameStateStore store;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI phaseText;
    public TextMeshProUGUI hpText;

    void Update()
    {
        var s = store?.LatestState;

        roundText.text = s != null ? $"Round: {s.round}" : "Round: -";
        phaseText.text = s != null ? $"Phase: {s.phase}" : "Phase: -";

        if (s?.self != null)
            hpText.text = $"HP: {s.self.currentHp}/{s.self.maxHp}";
        else
            hpText.text = "HP: -";
    }
}