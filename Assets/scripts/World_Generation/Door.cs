using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

    private Vector2Int position;
    private DoorType doorType;
    private Vector2Int NeighBoorDoor;

    // [SerializeField] private DoorEnum doorType;

    public delegate void OnDoorEnter(Door door);
    public static event OnDoorEnter OnChangeRoom;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.name == "Player") {
            OnChangeRoom(this);
        }
    }

    public DoorType GetDoorType() {
        return doorType;
    }

    public void SetDoorType(DoorType doorType) {
        this.doorType = doorType;
    }

    public void SetPosition(Vector2Int position) {
        this.position = position;
    }

}
