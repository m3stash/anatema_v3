using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomModel {

    public DoorWithVector2[] doorsToCheck;
    public Vector2Int rootPos;
    public Vector2Int worldPosition;
    public RoomShapeEnum roomShape;
    public int id;
    public Room room;
    public bool isStartRoom = false;
    public bool isRootRoom = false;

    public RoomModel(Vector2Int rootPos, RoomShapeEnum roomShape) {
        this.rootPos = rootPos;
        this.roomShape = roomShape;
    }
}