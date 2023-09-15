using System.Collections.Generic;
using UnityEngine;
using RoomNs;
using DoorNs;
using System.Linq;
using Debug = UnityEngine.Debug;
using System;
#if UNITY_EDITOR
#endif

namespace DungeonNs {

    public class Generator : MonoBehaviour {

        private IDungeonSeedGenerator dungeonSeedGenerator;
        private IDungeonInitializer dungeonInitializer;
        private IRoomFactory roomFactory;
        private IDungeonUtils dungeonUtils;
        private GameObject floorContainer;
        private CurrentFloorConfig currentFloorConfig;
        private RoomManager roomManager;
        private int[,] floorplan;
        private HashSet<(int, int)> occupiedCells = new HashSet<(int, int)>();
        private int totalLoop = 0;
        private readonly int seedLengh = 8;

        private void Awake() {
            dungeonSeedGenerator = DungeonSeedGenerator.GetInstance();
            dungeonInitializer = DungeonInitializer.GetInstance();
            dungeonUtils = DungeonUtils.GetInstance();
            roomFactory = new RoomFactory();
        }

        public void GenerateDungeon(CurrentFloorConfig currentFloorConfig, GameObject floorContainer) {
            this.currentFloorConfig = currentFloorConfig;
            this.floorContainer = floorContainer;
            string seed = dungeonSeedGenerator.GetNewSeed(seedLengh);
            seed = "TOTOTOTO";
            dungeonInitializer.InitValues(currentFloorConfig, seed, dungeonSeedGenerator);
            roomManager = new RoomManager(dungeonInitializer, roomFactory);
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
            for(var i = 0; i < 1000; i++) {
                count++;
                TryGenerateRooms();
            }
            Debug.Log("Try Succesfull for " + count + " times");
            
        }

        private bool IsRoomGenerationUnsuccessful() {
            return roomManager.GetListOfRoom().Count < dungeonInitializer.GetNumberOfRooms();
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

        private void ManageSpecialRooms() {
            var (listOf1Neighbors, listOf2Neighbors, listOf3Neighbors) = CountNeighborsAndCreateHashset();
            AddRoomInListAndFloorplan(listOf1Neighbors, listOf2Neighbors, listOf3Neighbors);
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

                    if (dungeonUtils.CheckIsInBounds(newRow, newCol, dungeonInitializer.GetFloorPlanBound()) && floorplan[newRow, newCol] == 0) {
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

        private void AddRoomInListAndFloorplan(HashSet<(int, int)> oneNeighbors, HashSet<(int, int)> listOf2Neighbors, HashSet<(int, int)> listOf3Neighbors) {
            AddSpecialRoom(oneNeighbors, RoomTypeEnum.BOSS);
            HashSet<(int, int)> listOfNeighbors = listOf3Neighbors.Count == 0 ? listOf2Neighbors : listOf3Neighbors;
            AddSpecialRoom(listOfNeighbors, RoomTypeEnum.SECRET);
        }

        private (int, int)? GetMaxDistanceRoomFromStarter(HashSet<(int, int)> listOfNeighbors) {
            double maxDistanceSquared = 0;
            (int, int)? farthestPosition = null;
            Vector2Int starter = dungeonInitializer.GetVectorStart();

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
            /*int randomRoom = dungeonInitializer.GetNextRandomValue(listOfNeighbors.Count);
            (int x, int y) selectedRoom = listOfNeighbors.ElementAt(randomRoom);*/
            if (GetMaxDistanceRoomFromStarter(listOfNeighbors) is (int x, int y)) {
                Room room = new Room_R1X1(new Vector2Int(x, y));
                room.SetRoomType(type);
                roomManager.AddRoom(room);
                floorplan[x, y] = 1;
                occupiedCells.Add((x, y));
            } else {
                Debug.Log("ERROR AddSpecialRoom: GetMaxDistanceRoomFromStarter");
            }
        }

        private int CountOccupiedNeighbors(int[,] grid, int row, int col) {
            int count = 0;

            foreach (var direction in dungeonUtils.GetDirection()) {
                int newRow = row + direction[0];
                int newCol = col + direction[1];

                if (dungeonUtils.CheckIsInBounds(newRow, newCol, dungeonInitializer.GetFloorPlanBound()) && grid[newRow, newCol] > 0) {
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
            foreach (KeyValuePair<DifficultyEnum, float> values in dungeonInitializer.GetRoomRepartition()) {

                DifficultyEnum diff = values.Key;
                // for each difficulties
                for (var i = 0; i < values.Value; i++) {

                    Room Room = roomManager.GetNextRoom();
                    if (Room == null) {
                        Debug.LogError("CreateRooms : No more rooms available");
                        return;
                    }

                    DifficultyEnum difficulty = Room.GetRoomTypeEnum == RoomTypeEnum.STANDARD ? diff : DifficultyEnum.DEFAULT;
                    InstanciateGoRoomAndDoors(Room, difficulty);
                }
            }
            
        }

        private void CreateSecretRoomGo() {
            foreach (Room Room in roomManager.GetListOfRoom()) {
                InstanciateGoRoomAndDoors(Room, DifficultyEnum.DEFAULT);
            }
        }

        private void InstanciateGoRoomAndDoors(Room Room, DifficultyEnum difficulty) {
            RoomShapeEnum shape = Room.GetShape();
            GameObject roomGo = roomFactory.CreateRoomGO(difficulty, shape, Room.GetRoomTypeEnum, dungeonInitializer, currentFloorConfig.GetBiomeType());
            Vector2Int worldPos = Room.GetWorldPosition();
            RoomGO roomGO = Instantiate(roomGo, new Vector3(worldPos.x, worldPos.y, 0), transform.rotation, floorContainer.transform).GetComponent<RoomGO>();
            roomGO.Setup(worldPos, shape);

            CreateDoorsGo(Room, roomGO);
        }

        private void CreateDoorsGo(Room Room, RoomGO roomGO) {
            Room.SearchNeighborsAndCreateDoor(floorplan, dungeonInitializer.GetFloorPlanBound(), currentFloorConfig.GetBiomeType(), dungeonUtils);
            List<Door> doorList = Room.GetDoors();
            if (doorList.Count > 0) {
                foreach (Door door in doorList) {
                    try {
                        GameObject doorGo = Instantiate(Resources.Load<GameObject>(GlobalConfig.Instance.PrefabDoorsPath + "Door"), Vector3.zero, transform.rotation);
                        doorGo.GetComponent<DoorGO>().SetDirection(door.GetDirection());
                        doorGo.transform.SetParent(roomGO.DoorsContainer.transform);
                        doorGo.transform.localPosition = door.LocalPosition;
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

            Vector2Int vectorStart = dungeonInitializer.GetVectorStart();
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

            int randomNeighbor = dungeonInitializer.GetNextRandomValue(listOfEmptySpaces.Count);
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
            return roomPositions.Count > 0 && roomManager.GetListOfRoom().Count < dungeonInitializer.GetNumberOfRooms();
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
            int randomIndex = dungeonInitializer.GetNextRandomValue(queue.Count);
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
                if (!dungeonUtils.CheckIsOutOfBound(checkNewPlace, dungeonInitializer.GetFloorPlanBound())) {
                    int neighbour = floorplan[checkNewPlace.x, checkNewPlace.y] > 0 ? 1 : 0;
                    count += neighbour;
                }
            }
            return count;
        }

        private bool CheckIsEmptySpace(Vector2Int vector, Room room) {
            Vector2Int[] cells = room.GetOccupiedCells(vector);
            int usedCells = cells.Sum(cell => dungeonUtils.CheckIsOutOfBound(cell, dungeonInitializer.GetFloorPlanBound()) ? 1 : floorplan[cell.x, cell.y]);
            return usedCells == 0;
        }

    }
}