using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using RoomNs;

public static class WorldUtils {

    public static Vector2Int[] GetSectionPerRoom(RoomShapeEnum shape, Vector2Int vector) {
        switch (shape) {
            case RoomShapeEnum.R1X1: {
                Vector2Int m = new Vector2Int(vector.x, vector.y);
                return new Vector2Int[] { m };
            }
            case RoomShapeEnum.R2X2: {
                Vector2Int bl = new Vector2Int(vector.x, vector.y);
                Vector2Int br = new Vector2Int(vector.x + 1, vector.y);
                Vector2Int tl = new Vector2Int(vector.x, vector.y + 1);
                Vector2Int tr = new Vector2Int(vector.x + 1, vector.y + 1 );
                return new Vector2Int[] { bl, br, tl, tr };
            }
            case RoomShapeEnum.R2X1: {
                Vector2Int l = new Vector2Int(vector.x, vector.y);
                Vector2Int r = new Vector2Int(vector.x + 1, vector.y);
                return new Vector2Int[] { l, r };
            }
            case RoomShapeEnum.R1X2: {
                Vector2Int b = new Vector2Int(vector.x, vector.y);
                Vector2Int t = new Vector2Int(vector.x, vector.y + 1);
                return new Vector2Int[] { b, t };
            }
        }
        return null;
    }
}
