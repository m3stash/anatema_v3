using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

    [SerializeField] private DoorEnum doorType;

    public delegate void OnDoorEnter(Door door);
    public static event OnDoorEnter OnChangeRoom;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.name == "Player(Clone)") {
            OnChangeRoom(this);
        }
    }

    public DoorEnum GetDoorType() {
        return doorType;
    }

}
