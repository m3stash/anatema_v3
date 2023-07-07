using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using RoomNs;


/* TODO ====> trouver le moyen de créer un objet plus complexe qui aura : 
    - une liste Vector2
    - le nom de la porte ex: "T", "B" etc..
    - le vector 2 de la position de la porte (ex: new Vector2Int(0, 16))
*/
public static class WorldUtils {
    public static Vector2Int[] GetNeighborsByShapes(RoomShapeEnum shape, Vector2Int vector) {
        switch (shape) {
            case RoomShapeEnum.R1X1: {
                Vector2Int t = new Vector2Int(vector.x, vector.y + 1);
                Vector2Int b = new Vector2Int(vector.x, vector.y - 1);
                Vector2Int l = new Vector2Int(vector.x - 1, vector.y);
                Vector2Int r = new Vector2Int(vector.x + 1, vector.y);
                return new Vector2Int[] { t, b, l, r };
            }
            case RoomShapeEnum.R2X2: {
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
            case RoomShapeEnum.R2X1: {
                Vector2Int tl = new Vector2Int(vector.x, vector.y + 1);
                Vector2Int tr = new Vector2Int(vector.x + 1, vector.y + 1);
                Vector2Int bl = new Vector2Int(vector.x, vector.y - 1);
                Vector2Int br = new Vector2Int(vector.x + 1, vector.y - 1);
                Vector2Int l = new Vector2Int(vector.x - 1, vector.y);
                Vector2Int r = new Vector2Int(vector.x + 2, vector.y);
                return new Vector2Int[] { tl, tr, bl, br, l, r };
            }
            case RoomShapeEnum.R1X2: {
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
