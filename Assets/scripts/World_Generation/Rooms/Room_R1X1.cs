using RoomNs;
using UnityEngine;

public class Room_R1X1 : PseudoRoom {

    public override Vector2Int GetSizeOfRoom() {
        return new Vector2Int(1, 1);
    }

    public Room_R1X1() : base() {
        roomShape = RoomShapeEnum.R1X1;
    }

    public Room_R1X1(Vector2Int pos) : base(pos) {
        position = pos;
        roomShape = RoomShapeEnum.R1X1;
    }

    public override Vector2Int[] GetDirections(Vector2Int vector) {
        Vector2Int[] dir = {
            new Vector2Int(vector.x - 1, vector.y),
            new Vector2Int(vector.x + 1, vector.y),
            new Vector2Int(vector.x, vector.y + 1),
            new Vector2Int(vector.x, vector.y - 1)
        };
        return dir;
    }

    public override Vector2Int[] GetOccupiedCells(Vector2Int vector) {
        Vector2Int[] cells = {
            new Vector2Int(vector.x, vector.y)
        };
        return cells;
    }

    public override Vector2Int[] GetNeighborsCells(Vector2Int vector) {
        Vector2Int t = new Vector2Int(vector.x, vector.y + 1);
        Vector2Int b = new Vector2Int(vector.x, vector.y - 1);
        Vector2Int l = new Vector2Int(vector.x - 1, vector.y);
        Vector2Int r = new Vector2Int(vector.x + 1, vector.y);
        return new Vector2Int[] { t, b, l, r };
    }
}
