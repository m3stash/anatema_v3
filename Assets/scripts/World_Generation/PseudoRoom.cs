using System.Collections.Generic;
using UnityEngine;

public class PseudoRoom {
    private Vector2Int position;
    private RoomShapeEnum roomShape;
    private bool isStartRoom;
    private bool isEndRoom;
    protected List<Door> doors;

    public PseudoRoom(Vector2Int position, RoomShapeEnum roomShape, bool isStartRoom = false) {
        this.position = position;
        this.roomShape = roomShape;
        this.isStartRoom = isStartRoom;
    }

    // public virtual void GetCurrentNeightboorsByShape(PseudoRoom room) { }

    public virtual void SeachNeightboors(List<PseudoRoom> listOfPseudoRoom, int bound) {}

    public void SetIsEndRoom(bool value) {
        isEndRoom = value;
    }

    public bool GetIsStartRoom() {
        return isStartRoom;
    }

    public bool GetIsEndRoom() {
        return isEndRoom;
    }

    public Vector2Int GetPosition() {
        return position;
    }

    public RoomShapeEnum GetRoomShape() {
        return roomShape;
    }
}