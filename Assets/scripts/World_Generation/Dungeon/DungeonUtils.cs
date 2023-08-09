using UnityEngine;

namespace DungeonNs {
    public static class Utilities {
        public static bool CheckIsOutOfBound(Vector2Int vector, int floorplanBound) {
            return vector.x < 0 || vector.x > floorplanBound || vector.y > floorplanBound || vector.y < 0;
        }
    }
}