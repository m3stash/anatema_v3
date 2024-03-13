using UnityEngine;

public class RoomUIInputManager : MonoBehaviour {

    public RoomUIInput roomUIInput;

    private void Awake() {
        Init();
    }

    private void Init() {
        roomUIInput = new RoomUIInput();
        roomUIInput.Enable();
    }

    void Start() {
        roomUIInput.Modal_RoomMananger.Enable();
        roomUIInput.Page_Event.Enable();
    }

    private void OnDestroy() {
        roomUIInput.Modal_RoomMananger.Disable();
        roomUIInput.Page_Event.Disable();
    }

    public RoomUIInput GetRoomUIInput() {
        return roomUIInput;
    }
}

