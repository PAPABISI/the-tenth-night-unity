using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ApiClient : MonoBehaviour
{
    [Header("Server")]
    public string baseUrl = "http://localhost:5000";

    public IEnumerator Get(string path, Action<string> onOk, Action<string> onErr = null)
    {
        using var req = UnityWebRequest.Get(baseUrl + path);
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
            onOk?.Invoke(req.downloadHandler.text);
        else
            onErr?.Invoke($"{req.error}\n{req.downloadHandler.text}");
    }

    public IEnumerator PostJson(string path, string json, Action<string> onOk, Action<string> onErr = null)
    {
        var req = new UnityWebRequest(baseUrl + path, "POST");
        byte[] raw = Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(raw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
            onOk?.Invoke(req.downloadHandler.text);
        else
            onErr?.Invoke($"{req.error}\n{req.downloadHandler.text}");
    }
}