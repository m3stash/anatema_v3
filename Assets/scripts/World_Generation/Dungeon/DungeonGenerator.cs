using System.Collections.Generic;
using UnityEngine;
using RoomNs;
using DoorNs;
using System.Linq;
using Debug = UnityEngine.Debug;
using System;
using UnityEditor.EditorTools;
#if UNITY_EDITOR
#endif

namespace DungeonNs {

    public class Generator : MonoBehaviour {

        private IDungeonFloorValues dungeonFloorValues;
        private IDungeonUtils dungeonUtils;
        private IDungeonFloorConfig floorConfig;
        private IRoomManager roomManager;
        private IDoorManager doorManager;
        private IBiomeManager biomeManager;
        private GameObject floorContainer;
        private int[,] floorplan;
        private HashSet<(int, int)> occupiedCells = new HashSet<(int, int)>();
        private int totalLoop = 0;

        public void GenerateDungeon(
            IDungeonFloorConfig floorConfig,
            GameObject floorContainer,
            IBiomeManager biomeManager,
            IDungeonFloorValues dungeonFloorValues,
            IDungeonUtils dungeonUtils,
            IRoomManager roomManager,
            IDoorManager doorManager
        ) {
            this.biomeManager = biomeManager;
            this.floorConfig = floorConfig;
            this.floorContainer = floorContainer;
            this.dungeonFloorValues = dungeonFloorValues;
            this.dungeonUtils = dungeonUtils; // TODO utiliser le dungeon manager pour faire proxy avec le dungeonUtils !!!
            this.roomManager = roomManager;
            this.doorManager = doorManager;
            GenerateAndPlaceRooms();
            ManageSpecialRooms();
            CreateRoomsGO();
        }

        private void TryGenerateRooms() {
            if (IsRoomGenerationUnsuccessful()) {
                AttemptReGeneration();
            } else {
                Debug.Log("Total Loop for current Generation " + totalLoop);
            }
        }

        private void TryGenerateRooms1000Times() {
            int count = 0;
            for (var i = 0; i < 1000; i++) {
                count++;
                TryGenerateRooms();
            }
            Debug.Log("Try Succesfull for " + count + " times");

        }

        private bool IsRoomGenerationUnsuccessful() {
            return roomManager.GetListOfRoom().Count < dungeonFloorValues.GetNumberOfRooms();
        }

        private void AttemptReGeneration() {
            totalLoop++;
            if (totalLoop <= 100) {
                GenerateAndPlaceRooms();
            } else {
                Debug.LogError("TRY GenerateRooms call == 100 tries");
            }
        }

        private void GenerateAndPlaceRooms() {
            floorplan = new int[12, 12];
            CreateRoomQueue();
            TryGenerateRooms();
            // TryGenerateRooms1000Times();
        }

        private (HashSet<(int, int)>, HashSet<(int, int)>, HashSet<(int, int)>) CountNeighborsAndCreateHashset() {
            HashSet<(int, int)> emptyCellsWithOneNeighbor = new HashSet<(int, int)>();
            HashSet<(int, int)> emptyCellsWithTwoNeighbors = new HashSet<(int, int)>();
            HashSet<(int, int)> emptyCellsWithMoreThan3Neighbors = new HashSet<(int, int)>();
            var directions = dungeonUtils.GetDirection();

            foreach ((int row, int col) in occupiedCells) {

                foreach (var direction in directions) {
                    int newRow = row + direction[0];
                    int newCol = col + direction[1];

                    if (dungeonUtils.CheckIsInBounds(newRow, newCol, dungeonFloorValues.GetFloorPlanBound()) && floorplan[newRow, newCol] == 0) {
                        int occupiedNeighbors = CountOccupiedNeighbors(floorplan, newRow, newCol);
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

        private void ManageSpecialRooms() {
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
                Debug.Log("NO position found for GetMaxDistanceRoomFromStarter");
            }

            return farthestPosition;
        }

        private void AddSpecialRoom(HashSet<(int, int)> listOfNeighbors, RoomTypeEnum type) {
            if (GetMaxDistanceRoomFromStarter(listOfNeighbors) is (int x, int y)) {
                Room room = roomManager.InstantiateRoomImplWithProperties(RoomShapeEnum.R1X1, new Vector2Int(x, y), type);
                roomManager.AddRoom(room);
                floorplan[x, y] = 1;
                occupiedCells.Add((x, y));
            } else {
                Debug.Log("ERROR AddSpecialRoom");
            }
        }

        private int CountOccupiedNeighbors(int[,] grid, int row, int col) {
            int count = 0;

            foreach (var direction in dungeonUtils.GetDirection()) {
                int newRow = row + direction[0];
                int newCol = col + direction[1];

                if (dungeonUtils.CheckIsInBounds(newRow, newCol, dungeonFloorValues.GetFloorPlanBound()) && grid[newRow, newCol] > 0) {
                    count++;
                }
            }
            return count;
        }

        private void CreateRoomsGO() {
            CreateRoomGO();
            CreateSecretRoomGo();
        }

        private void CreateRoomGO() {
            foreach (KeyValuePair<DifficultyEnum, float> values in dungeonFloorValues.GetRoomRepartition()) {
                DifficultyEnum diff = values.Key;
                for (var i = 0; i < values.Value; i++) {

                    Room Room = roomManager.GetNextRoom();
                    if (Room == null) {
                        Debug.LogError("CreateRooms : No more rooms available");
                        return;
                    }

                    DifficultyEnum difficulty = Room.GetRoomTypeEnum == RoomTypeEnum.STANDARD ? diff : DifficultyEnum.DEFAULT;
                    GameObject roomGO = InstanciateRoomGo(Room, difficulty);
                    CreateDoorsGo(Room, roomGO);
                }
            }

        }

        private void CreateSecretRoomGo() {
            foreach (Room Room in roomManager.GetListOfRoom()) {
                GameObject roomGO = InstanciateRoomGo(Room, DifficultyEnum.DEFAULT);
                CreateDoorsGo(Room, roomGO);
            }
        }

        private GameObject InstanciateRoomGo(Room Room, DifficultyEnum difficulty) {
            RoomShapeEnum shape = Room.GetShape();
            GameObject roomPrefab = roomManager.InstantiateRoomPrefab(difficulty, shape, Room.GetRoomTypeEnum, dungeonFloorValues, floorConfig.GetBiomeType());
            Vector2Int worldPos = Room.GetWorldPosition();
            return roomManager.InstantiateRoomGO(roomPrefab, new Vector3(worldPos.x, worldPos.y, 0), transform, floorContainer);
        }

        private void CreateDoorsGo(Room Room, GameObject roomGo) {
            Room.SearchNeighborsAndCreateDoor(floorplan, dungeonFloorValues.GetFloorPlanBound(), floorConfig.GetBiomeType(), dungeonUtils);
            List<Door> doorList = Room.GetDoors();
            if (doorList.Count > 0) {
                foreach (Door door in doorList) {
                    try {
                        // toDO gérer un POOOOOOOL !
                        GameObject doorPrefab = doorManager.InstantiateRoomPrefab(GlobalConfig.Instance.PrefabDoorsPath + "Door");
                        GameObject doorGo = doorManager.InstantiateDoorGO(doorPrefab, Vector3.zero, transform, roomGo.transform);
                        doorManager.SetProperties(doorGo, door, biomeManager.GetBiomeConfiguration(floorConfig.GetBiomeType()));
                    } catch (Exception ex) {
                        Debug.LogError($"Error creating door game object: {ex.Message}");
                    }
                }
            }
        }

        private void SetFloorPlan(Room room, Vector2Int vector, int index) {
            foreach (var cell in room.GetOccupiedCells(vector)) {
                floorplan[cell.x, cell.y] = index;
                occupiedCells.Add((cell.x, cell.y));
            }
        }

        private bool CanAddShape(Vector2Int vector, Room room) {
            if (!CheckIsEmptySpace(vector, room)) {
                return false;
            }
            if (NeighborCount(vector, room) > 1) {
                return false;
            }
            return true;
        }

        private void CreateRoomQueue() {
            List<RoomShapeEnum> roomShapes = GetListOfSpecialShapes();
            int currentShapeIndex = 0;

            Vector2Int vectorStart = dungeonFloorValues.GetVectorStart();
            InitializeStarterRoom(vectorStart);

            Queue<Vector2Int> roomPositions = new Queue<Vector2Int>();
            roomPositions.Enqueue(vectorStart);

            while (ShouldContinueCreatingRooms(roomPositions)) {
                Vector2Int currentRoomPosition = DequeueRandomElement(roomPositions);
                Room room = roomManager.GenerateRoom(roomShapes, ref currentShapeIndex);

                if (TryPlaceRoom(room, currentRoomPosition, out Vector2Int newRoomPosition)) {
                    roomPositions.Enqueue(newRoomPosition);
                    AddRoomToManager(room, newRoomPosition);
                }
            }
        }

        private bool TryPlaceRoom(Room room, Vector2Int roomPosition, out Vector2Int newRoomPosition) {
            List<Vector2Int> listOfEmptySpaces = GetEmptySpaces(room, roomPosition);

            if (listOfEmptySpaces.Count == 0) {
                newRoomPosition = default;
                return false;
            }

            int randomNeighbor = dungeonFloorValues.GetNextRandomValue(listOfEmptySpaces.Count);
            newRoomPosition = listOfEmptySpaces[randomNeighbor];
            room.SetPosition(newRoomPosition);
            room.SetRoomType(RoomTypeEnum.STANDARD);
            return true;
        }

        private void AddRoomToManager(Room room, Vector2Int position) {
            roomManager.AddRoom(room);
            SetFloorPlan(room, position, roomManager.GetListOfRoom().Count + 1);
        }

        private bool ShouldContinueCreatingRooms(Queue<Vector2Int> roomPositions) {
            return roomPositions.Count > 0 && roomManager.GetListOfRoom().Count < dungeonFloorValues.GetNumberOfRooms();
        }

        private void InitializeStarterRoom(Vector2Int vectorStart) {
            Room starterRoom = new Room_R1X1(vectorStart);
            starterRoom.SetRoomType(RoomTypeEnum.STARTER);
            roomManager.AddRoom(starterRoom);
            SetFloorPlan(starterRoom, vectorStart, 1);
        }

        private List<RoomShapeEnum> GetListOfSpecialShapes() {
            return Enum.GetValues(typeof(RoomShapeEnum))
               .Cast<RoomShapeEnum>()
               .Where(shape => shape != RoomShapeEnum.R1X1)
               .ToList();
        }

        private Vector2Int DequeueRandomElement(Queue<Vector2Int> queue) {
            int randomIndex = dungeonFloorValues.GetNextRandomValue(queue.Count);
            Vector2Int[] array = queue.ToArray();
            Vector2Int randomElement = array[randomIndex];
            queue = new Queue<Vector2Int>(array.Where(element => element != randomElement));
            return randomElement;
        }

        private List<Vector2Int> GetEmptySpaces(Room room, Vector2Int position) {
            return room.GetDirections(position)
                .Where(Vector2Int => CanAddShape(Vector2Int, room))
                .ToList();
        }

        private int NeighborCount(Vector2Int vector, Room room) {
            int count = 0;
            Vector2Int[] shapesToCheck = room.GetNeighborsCells(vector);

            if (shapesToCheck.Length == 0) {
                return -1;
            }
            foreach (var checkNewPlace in shapesToCheck) {
                if (!dungeonUtils.CheckIsOutOfBound(checkNewPlace, dungeonFloorValues.GetFloorPlanBound())) {
                    int neighbour = floorplan[checkNewPlace.x, checkNewPlace.y] > 0 ? 1 : 0;
                    count += neighbour;
                }
            }
            return count;
        }

        private bool CheckIsEmptySpace(Vector2Int vector, Room room) {
            Vector2Int[] cells = room.GetOccupiedCells(vector);
            int usedCells = cells.Sum(cell => dungeonUtils.CheckIsOutOfBound(cell, dungeonFloorValues.GetFloorPlanBound()) ? 1 : floorplan[cell.x, cell.y]);
            return usedCells == 0;
        }

    }
}