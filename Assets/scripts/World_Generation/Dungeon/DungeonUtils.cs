using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

namespace DungeonNs {
    public static class Utilities {
        public static bool CheckIsOutOfBound(Vector2Int vector, int floorplanBound) {
            return vector.x < 0 || vector.x > floorplanBound || vector.y > floorplanBound || vector.y < 0;
        }

        public static bool CheckIsOnOfBound(int x, int y, int bound) {
            return x > 0 && x < bound && y > 0 && y < bound;
        }

        public static Vector2Int GetOffsetForDirection(DirectionalEnum direction) {
            switch (direction) {
                case DirectionalEnum.T:
                return Vector2Int.up;
                case DirectionalEnum.B:
                return Vector2Int.down;
                case DirectionalEnum.L:
                return Vector2Int.left;
                case DirectionalEnum.R:
                return Vector2Int.right;
                default:
                return Vector2Int.zero;
            }
        }

    }


}