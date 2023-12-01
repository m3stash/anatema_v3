using RoomNs;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonNs {
    public class SpecialRoomManager {

        private IFloorPlanManager floorPlanManager;
        private Vector2Int vectorStart;
        private List<SpecialRoom> specialRooms;

        public class SpecialRoom {

            private RoomShapeEnum shape;
            private Vector2Int vector;
            private RoomTypeEnum type;

            public SpecialRoom(RoomShapeEnum shape, Vector2Int vector, RoomTypeEnum type) {
                this.shape = shape;
                this.vector = vector;
                this.type = type;
            }

            public RoomShapeEnum GetShape() {
                return shape;
            }

            public Vector2Int GetVector() {
                return vector;
            }

            public RoomTypeEnum GetTypeEnum() {
                return type;
            }
        }

        public SpecialRoomManager(Vector2Int vectorStart, IFloorPlanManager floorPlanManager) {
            this.floorPlanManager = floorPlanManager;
            this.vectorStart = vectorStart;
            specialRooms = new List<SpecialRoom>();
        }

        public List<SpecialRoom> CreateSpecialRooms() {
            var (oneNeighbors, listOf2Neighbors, listOf3Neighbors) = CountNeighborsAndCreateHashset();
            AddSpecialRoom(oneNeighbors, RoomTypeEnum.BOSS);
            HashSet<(int, int)> listOfNeighbors = listOf3Neighbors.Count == 0 ? listOf2Neighbors : listOf3Neighbors;
            AddSpecialRoom(listOfNeighbors, RoomTypeEnum.SECRET);
            return specialRooms;
        }

        private (int, int)? GetMaxDistanceRoomFromStarter(HashSet<(int, int)> listOfNeighbors) {
            double maxDistanceSquared = 0;
            (int, int)? farthestPosition = null;
            Vector2Int starter = vectorStart;

            foreach ((int x, int y) in listOfNeighbors) {
                double distanceSquared = (starter.x - x) * (starter.x - x) + (starter.y - y) * (starter.y - y);
                if (distanceSquared > maxDistanceSquared) {
                    maxDistanceSquared = distanceSquared;
                    farthestPosition = (x, y);
                }
            }

            if (!farthestPosition.HasValue) {
                Debug.LogError("NO position found for GetMaxDistanceRoomFromStarter");
            }

            return farthestPosition;
        }

        private void AddSpecialRoom(HashSet<(int, int)> listOfNeighbors, RoomTypeEnum type) {
            if (GetMaxDistanceRoomFromStarter(listOfNeighbors) is (int x, int y)) {
                specialRooms.Add(new SpecialRoom(RoomShapeEnum.R1X1, new Vector2Int(x, y), type));
            } else {
                Debug.LogError("ERROR AddSpecialRoom");
            }
        }

        private int CountOccupiedNeighbors(int row, int col) {
            int count = 0;

            foreach (var direction in floorPlanManager.GetDirection()) {
                int newRow = row + direction[0];
                int newCol = col + direction[1];

                if (floorPlanManager.CheckIsInBounds(newRow, newCol, floorPlanManager.GetFloorPlanBound()) && floorPlanManager.GetFloorPlanValue(newRow, newCol) > 0) {
                    count++;
                }
            }
            return count;
        }

        private (HashSet<(int, int)>, HashSet<(int, int)>, HashSet<(int, int)>) CountNeighborsAndCreateHashset() {
            HashSet<(int, int)> emptyCellsWithOneNeighbor = new HashSet<(int, int)>();
            HashSet<(int, int)> emptyCellsWithTwoNeighbors = new HashSet<(int, int)>();
            HashSet<(int, int)> emptyCellsWithMoreThan3Neighbors = new HashSet<(int, int)>();
            var directions = floorPlanManager.GetDirection();

            foreach ((int row, int col) in floorPlanManager.GetOccupiedCells()) {

                foreach (var direction in directions) {
                    int newRow = row + direction[0];
                    int newCol = col + direction[1];

                    if (floorPlanManager.CheckIsInBounds(newRow, newCol, floorPlanManager.GetFloorPlanBound()) && floorPlanManager.GetFloorPlanValue(newRow, newCol) == 0) {
                        int occupiedNeighbors = CountOccupiedNeighbors(newRow, newCol);
                        if (occupiedNeighbors == 1) {
                            emptyCellsWithOneNeighbor.Add((newRow, newCol));
                        }
                        if (occupiedNeighbors == 2) {
                            emptyCellsWithTwoNeighbors.Add((newRow, newCol));
                        }
                        if (occupiedNeighbors >= 3) {
                            emptyCellsWithMoreThan3Neighbors.Add((newRow, newCol));
                        }
                    }
                }
            }

            return (emptyCellsWithOneNeighbor, emptyCellsWithTwoNeighbors, emptyCellsWithMoreThan3Neighbors);
        }
    }
}