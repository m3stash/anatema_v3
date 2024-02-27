/*using RoomNs;
using UnityEngine;

public class Room_L : Room {

    public override Vector2Int GetSizeOfRoom() {
        return new Vector2Int(2, 2);
    }

    public Room_L() : base() {
        roomShape = RoomShapeEnum.L;
    }
    
    public override Vector2Int[] GetDirections(Vector2Int vector) {
        Vector2Int[] dir = {
            new Vector2Int(vector.x - 2, vector.y),
            new Vector2Int(vector.x + 1, vector.y),
            new Vector2Int(vector.x, vector.y + 1),
            new Vector2Int(vector.x, vector.y - 2)
        };
        return dir;
    }

    // toDo voir à renommer cette méthode elle sert aussi pour les sections !
    public override Vector2Int[] GetSections(Vector2Int vector) {
        Vector2Int[] cells = {
            new Vector2Int(vector.x, vector.y),
            new Vector2Int(vector.x + 1, vector.y),
            new Vector2Int(vector.x, vector.y + 1),
        };
        return cells;
    }

    public override Vector2Int[] GetNeighborsCells(Vector2Int vector) {
        Vector2Int tl = new Vector2Int(vector.x, vector.y + 2);
        Vector2Int lt = new Vector2Int(vector.x - 1, vector.y + 1);
        Vector2Int lb = new Vector2Int(vector.x - 1, vector.y);
        Vector2Int rb = new Vector2Int(vector.x + 2, vector.y);
        Vector2Int bl = new Vector2Int(vector.x, vector.y - 1);
        Vector2Int br = new Vector2Int(vector.x + 1, vector.y - 1);
        Vector2Int mtr = new Vector2Int(vector.x + 1, vector.y + 1);
        return new Vector2Int[] { tl, lt, lb, rb, bl, br, mtr };
    }

}*/
