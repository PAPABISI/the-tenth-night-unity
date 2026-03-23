using System.Collections;
using TMPro;
using UnityEngine;

public class UiToast : MonoBehaviour
{
    public TMP_Text text;
    public float defaultSeconds = 2f;
    private Coroutine _co;

    private void Awake()
    {
        if (text != null) text.gameObject.SetActive(false);
    }

    public void Show(string msg, float seconds = -1f)
    {
        if (text == null) return;
        if (_co != null) StopCoroutine(_co);
        _co = StartCoroutine(CoShow(msg, seconds > 0 ? seconds : defaultSeconds));
    }

    private IEnumerator CoShow(string msg, float seconds)
    {
        text.text = msg;
        text.gameObject.SetActive(true);
        yield return new WaitForSeconds(seconds);
        text.gameObject.SetActive(false);
        _co = null;
    }
}