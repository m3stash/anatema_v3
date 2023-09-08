using RoomNs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room_R1X2 : PseudoRoom {

    public override Vector2Int GetSizeOfRoom() {
        return new Vector2Int(1, 2);
    }

    public Room_R1X2() {
        roomShape = RoomShapeEnum.R1X2;
    }

    public override Vector2Int[] GetDirections(Vector2Int vector) {
        Vector2Int[] dir = {
            new Vector2Int(vector.x - 1, vector.y),
            new Vector2Int(vector.x + 1, vector.y),
            new Vector2Int(vector.x, vector.y + 1),
            new Vector2Int(vector.x, vector.y - 2)
        };
        return dir;
    }

    public override Vector2Int[] GetOccupiedCells(Vector2Int vector) {
        Vector2Int[] cells = {
            new Vector2Int(vector.x, vector.y),
            new Vector2Int(vector.x, vector.y + 1)
        };
        return cells;
    }

    public override Vector2Int[] GetNeighborsCells(Vector2Int vector) {
        Vector2Int t = new Vector2Int(vector.x, vector.y + 2);
        Vector2Int b = new Vector2Int(vector.x, vector.y - 1);
        Vector2Int lt = new Vector2Int(vector.x - 1, vector.y + 1);
        Vector2Int lb = new Vector2Int(vector.x - 1, vector.y);
        Vector2Int rt = new Vector2Int(vector.x + 1, vector.y + 1);
        Vector2Int rb = new Vector2Int(vector.x + 1, vector.y);
        return new Vector2Int[] { t, b, lt, lb, rt, rb };
    }

}
