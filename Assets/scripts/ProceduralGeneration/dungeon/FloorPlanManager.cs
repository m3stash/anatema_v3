using System.Collections.Generic;
using UnityEngine;

namespace DungeonNs {
    public class FloorPlanManager: IFloorPlanManager {

        private readonly int contFloorPlanSize = 12;
        private int[,] floorplan;
        private HashSet<(int, int)> occupiedCells = new HashSet<(int, int)>();

        public FloorPlanManager() {
            floorplan = new int[contFloorPlanSize, contFloorPlanSize];
        }

        public void SetFloorPlanValue(int x, int y, int value) {
            floorplan[x, y] = value;
            occupiedCells.Add((x, y));
        }

        public int GetFloorPlanBound() {
            return floorplan.GetLength(0) - 1;
        }

        public int GetFloorPlanValue(int x, int y) {
            return floorplan[x, y];
        }

        public HashSet<(int, int)> GetSections() {
            return occupiedCells;
        }

        public void ResetFloorPlan() {
            floorplan = new int[contFloorPlanSize, contFloorPlanSize];
            occupiedCells = new HashSet<(int, int)>();
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
