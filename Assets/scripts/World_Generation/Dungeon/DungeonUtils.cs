using UnityEngine;

namespace DungeonNs {
    public class DungeonUtils: IDungeonUtils {

        private static DungeonUtils instance;

        public static DungeonUtils GetInstance() {
            instance ??= new DungeonUtils();
            return instance;
        }

        public bool CheckIsOutOfBound(Vector2Int vector, int floorplanBound) {
            return vector.x < 0 || vector.x > floorplanBound || vector.y > floorplanBound || vector.y < 0;
        }

        public bool CheckIsInBounds(int x, int y, int bound) {
            return x >= 0 && x <= bound && y >= 0 && y <= bound;
        }

        public int[][] GetDirection() {
            return new int[][] {
                new int[] { -1, 0 }, // Up
                new int[] { 1, 0 },  // Down
                new int[] { 0, -1 }, // Left
                new int[] { 0, 1 }   // Right
            };
        }
    }

}