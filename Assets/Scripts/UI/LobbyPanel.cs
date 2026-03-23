using UnityEngine;

public class LobbyPanel : MonoBehaviour
{
    public GameController controller;

    public void OnClickCreateRoom() => controller.CreateRoom();
    public void OnClickStartGame() => controller.StartGame();
}