using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Room : MonoBehaviour {

    public Vector2Int gridPos;
    public int type;
    public RoomShapeEnum roomShape;
    public bool doorTop = false;
    public bool doorBot = false;
    public bool doorLeft = false;
    public bool doorRight = false;

    public void Setup(Vector2Int gridPos, int type, RoomShapeEnum roomShape) {
        this.gridPos = gridPos;
        this.type = type;
        this.roomShape = roomShape;
    }


}
