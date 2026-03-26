using UnityEngine;
using TMPro;

public class LobbyPanel : MonoBehaviour
{
    public GameController controller;
    public TMP_InputField roomIdInput;
    public TMP_InputField displayNameInput;

    private void Start()
    {
        if (controller == null || controller.store == null) return;

        if (roomIdInput != null)
            roomIdInput.text = controller.store.RoomId ?? string.Empty;

        if (displayNameInput != null)
            displayNameInput.text = controller.localDisplayName ?? string.Empty;
    }

    public void OnRoomIdChanged(string value)
    {
        if (controller == null || controller.store == null) return;
        controller.store.RoomId = value?.Trim();
    }

    public void OnDisplayNameChanged(string value)
    {
        if (controller == null) return;
        var next = value?.Trim();
        controller.localDisplayName = string.IsNullOrWhiteSpace(next) ? "Player" : next;
    }

    public void OnClickCreateRoom() => controller.CreateRoom();
    public void OnClickJoinRoom() => controller.JoinRoom();
    public void OnClickStartGame() => controller.StartGame();
}