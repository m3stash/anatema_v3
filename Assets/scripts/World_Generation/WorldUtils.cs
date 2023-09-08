using UnityEngine;
using RoomNs;

public static class WorldUtils {

    public static Vector2Int[] Directions = {
        new Vector2Int(-1, 0),  // Up
        new Vector2Int(1, 0),   // Down
        new Vector2Int(0, -1),  // Left
        new Vector2Int(0, 1)    // Right
    };
}
