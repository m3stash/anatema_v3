using RoomNs;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonNs {
    public class SpecialRoomManager {

        private IDungeonFloorValues dungeonFloorValues;
        private IRoomManager roomManager;
        private IDungeonUtils dungeonUtils;
        private IFloorPlanManager floorPlanManager;

        public SpecialRoomManager(IDungeonFloorValues dungeonFloorValues, IRoomManager roomManager, IDungeonUtils dungeonUtils, IFloorPlanManager floorPlanManager) {
            this.dungeonFloorValues = dungeonFloorValues;
            this.roomManager = roomManager;
            this.dungeonUtils = dungeonUtils;
            this.floorPlanManager = floorPlanManager;
        }

        public void PlaceSpecialRooms() {
            var (oneNeighbors, listOf2Neighbors, listOf3Neighbors) = CountNeighborsAndCreateHashset();
            AddSpecialRoom(oneNeighbors, RoomTypeEnum.BOSS);
            HashSet<(int, int)> listOfNeighbors = listOf3Neighbors.Count == 0 ? listOf2Neighbors : listOf3Neighbors;
            AddSpecialRoom(listOfNeighbors, RoomTypeEnum.SECRET);
        }

        private (int, int)? GetMaxDistanceRoomFromStarter(HashSet<(int, int)> listOfNeighbors) {
            double maxDistanceSquared = 0;
            (int, int)? farthestPosition = null;
            Vector2Int starter = dungeonFloorValues.GetVectorStart();

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
                Room room = roomManager.InstantiateRoomImplWithProperties(RoomShapeEnum.R1X1, new Vector2Int(x, y), type);
                roomManager.AddRoom(room);
                floorPlanManager.SetFloorPlanValue(x, y, 1);
            } else {
                Debug.LogError("ERROR AddSpecialRoom");
            }
        }

        private int CountOccupiedNeighbors(int row, int col) {
            int count = 0;

            foreach (var direction in dungeonUtils.GetDirection()) {
                int newRow = row + direction[0];
                int newCol = col + direction[1];

                if (dungeonUtils.CheckIsInBounds(newRow, newCol, floorPlanManager.GetFloorPlanBound()) && floorPlanManager.GetFloorPlanValue(newRow, newCol) > 0) {
                    count++;
                }
            }
            return count;
        }

        private (HashSet<(int, int)>, HashSet<(int, int)>, HashSet<(int, int)>) CountNeighborsAndCreateHashset() {
            HashSet<(int, int)> emptyCellsWithOneNeighbor = new HashSet<(int, int)>();
            HashSet<(int, int)> emptyCellsWithTwoNeighbors = new HashSet<(int, int)>();
            HashSet<(int, int)> emptyCellsWithMoreThan3Neighbors = new HashSet<(int, int)>();
            var directions = dungeonUtils.GetDirection();

            foreach ((int row, int col) in floorPlanManager.GetOccupiedCells()) {

                foreach (var direction in directions) {
                    int newRow = row + direction[0];
                    int newCol = col + direction[1];

                    if (dungeonUtils.CheckIsInBounds(newRow, newCol, floorPlanManager.GetFloorPlanBound()) && floorPlanManager.GetFloorPlanValue(newRow, newCol) == 0) {
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