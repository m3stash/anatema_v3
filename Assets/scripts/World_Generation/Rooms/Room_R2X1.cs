using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room_R2X1 : IRoom_shape {

    public Vector2Int[] GetDirections(Vector2Int vector) {
        Vector2Int[] dir = {
            new Vector2Int(vector.x - 2, vector.y),
            new Vector2Int(vector.x + 1, vector.y),
            new Vector2Int(vector.x, vector.y + 1),
            new Vector2Int(vector.x, vector.y - 1)
        };
        return dir;
    }

    public Vector2Int[] GetCellToVerify(Vector2Int vector) {
        Vector2Int[] cells = {
            new Vector2Int(vector.x, vector.y),
            new Vector2Int(vector.x + 1, vector.y)
        };
        return cells;
    }

    public Vector2Int[] GetNeighborsCells(Vector2Int vector) {
        Vector2Int tl = new Vector2Int(vector.x, vector.y + 1);
        Vector2Int tr = new Vector2Int(vector.x + 1, vector.y + 1);
        Vector2Int bl = new Vector2Int(vector.x, vector.y - 1);
        Vector2Int br = new Vector2Int(vector.x + 1, vector.y - 1);
        Vector2Int l = new Vector2Int(vector.x - 1, vector.y);
        Vector2Int r = new Vector2Int(vector.x + 2, vector.y);
        return new Vector2Int[] { tl, tr, bl, br, l, r };
    }

}
