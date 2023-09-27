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

        public HashSet<(int, int)> GetOccupiedCells() {
            return occupiedCells;
        }

        public void ResetFloorPlan() {
            floorplan = new int[contFloorPlanSize, contFloorPlanSize];
            occupiedCells = new HashSet<(int, int)>();
        }

    }
}
