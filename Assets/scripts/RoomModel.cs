using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomModel {

    public Vector2 gridPos;
    public int type;
    public bool doorTop = false;
    public bool doorBot = false;
    public bool doorLeft = false;
    public bool doorRight = false;
    public Room room;

    public RoomModel(Vector2 gridPos, int type) {
        this.gridPos = gridPos;
        this.type = type;
    }
}