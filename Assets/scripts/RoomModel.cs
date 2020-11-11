using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomModel {

    public Vector2Int gridPos;
    public RoomShapeEnum roomShape;
    public bool doorTop = false;
    public bool doorBot = false;
    public bool doorLeft = false;
    public bool doorRight = false;
    public Room room;

    public RoomModel(Vector2Int gridPos, RoomShapeEnum roomShape) {
        this.gridPos = gridPos;
        this.roomShape = roomShape;
    }
}