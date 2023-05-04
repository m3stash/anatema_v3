using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
    //toDO remove serialisable field..
    [SerializeField] private DoorType doorType;
    [SerializeField] private Vector2Int NeighBoorDoor;
    [SerializeField] private DirectionalEnum direction;
    [SerializeField] private Vector3Int localPosition;
    [SerializeField] private Sprite sprite;

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

    public void SetDirection(DirectionalEnum direction) {
        this.direction = direction;
    }

    public DirectionalEnum GetDirection() {
        return direction;
    }

    public void SetLocalPosition(Vector3Int worldPosition) {
        Debug.Log("worldPosition" + worldPosition);
        localPosition = worldPosition;
    }
    public Vector3 GetLocalPosition() {
        print(transform.localPosition.y);
        print(transform.localPosition.x);
        return transform.localPosition;
    }

    public void SetSprite(Sprite sprite) {
        this.sprite = sprite;
    }

}
