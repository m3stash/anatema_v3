using UnityEngine;

public class RoomUIInputManager {

    public RoomUIInput roomUIInput;

    public RoomUIInputManager() {
        Init();
    }

    private void Init() {
        roomUIInput = new RoomUIInput();
        roomUIInput.Enable();
    }

    void Start() {
        roomUIInput.Modal_RoomMananger.Enable();
    }

    private void OnDestroy() {
        Debug.Log("DESTROYYYY");
        roomUIInput.Modal_RoomMananger.Disable();
    }

    public RoomUIInput GetRoomUIInput() {
        return roomUIInput;
    }
}

