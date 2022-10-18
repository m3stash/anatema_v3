using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WorldUtils {
    public static Vector2Int[] GetNeighborsByShapes(RoomShapeEnum shape, Vector2Int vector, int bound) {
        switch (shape) {
            case RoomShapeEnum.ROOMSHAPE_1x1: {
                Vector2Int t = new Vector2Int(vector.x, vector.y + 1);
                Vector2Int b = new Vector2Int(vector.x, vector.y - 1);
                Vector2Int l = new Vector2Int(vector.x - 1, vector.y);
                Vector2Int r = new Vector2Int(vector.x + 1, vector.y);
                return new Vector2Int[] { t, b, l, r };
            }
            case RoomShapeEnum.ROOMSHAPE_2x2: {
                if (vector.y + 2 >= bound) {
                    return new Vector2Int[] { };
                }
                Vector2Int tl = new Vector2Int(vector.x, vector.y + 2);
                Vector2Int tr = new Vector2Int(vector.x + 1, vector.y + 2);
                Vector2Int lt = new Vector2Int(vector.x - 1, vector.y + 1);
                Vector2Int lb = new Vector2Int(vector.x - 1, vector.y);
                Vector2Int rt = new Vector2Int(vector.x + 1, vector.y + 1);
                Vector2Int rb = new Vector2Int(vector.x + 1, vector.y);
                Vector2Int bl = new Vector2Int(vector.x, vector.y - 1);
                Vector2Int br = new Vector2Int(vector.x + 1, vector.y - 1);
                return new Vector2Int[] { tl, tr, lt, lb, rt, rb, bl, br };
            }
            case RoomShapeEnum.ROOMSHAPE_2x1: {
                if (vector.x + 2 >= bound) {
                    return new Vector2Int[] { };
                }
                Vector2Int tl = new Vector2Int(vector.x, vector.y + 1);
                Vector2Int tr = new Vector2Int(vector.x + 1, vector.y + 1);
                Vector2Int bl = new Vector2Int(vector.x, vector.y - 1);
                Vector2Int br = new Vector2Int(vector.x + 1, vector.y - 1);
                Vector2Int l = new Vector2Int(vector.x - 1, vector.y);
                Vector2Int r = new Vector2Int(vector.x + 2, vector.y);
                return new Vector2Int[] { tl, tr, bl, br, l, r };
            }
            case RoomShapeEnum.ROOMSHAPE_1x2: {
                if (vector.y + 2 >= bound) {
                    return new Vector2Int[] { };
                }
                Vector2Int t = new Vector2Int(vector.x, vector.y + 2);
                Vector2Int b = new Vector2Int(vector.x, vector.y - 1);
                Vector2Int lt = new Vector2Int(vector.x - 1, vector.y + 1);
                Vector2Int lb = new Vector2Int(vector.x - 1, vector.y);
                Vector2Int rt = new Vector2Int(vector.x + 1, vector.y + 1);
                Vector2Int rb = new Vector2Int(vector.x + 1, vector.y);
                return new Vector2Int[] { t, b, lt, lb, rt, rb };
            }
        }
        return null;
    }
}
