using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomModel {

    public Vector2Int gridPos;
    public int type;
    public RoomShapeEnum roomShape;
    public bool doorTop = false;
    public bool doorBot = false;
    public bool doorLeft = false;
    public bool doorRight = false;
    public Room room;

    public RoomModel(Vector2Int gridPos, int type, RoomShapeEnum roomShape) {
        this.gridPos = gridPos;
        this.type = type;
        this.roomShape = roomShape;
    }
}