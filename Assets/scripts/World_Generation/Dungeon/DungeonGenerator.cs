using System.Collections.Generic;
using UnityEngine;
using RoomNs;
using DoorNs;
using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DungeonNs {

    public class Generator : MonoBehaviour {

        [SerializeField] private DungeonInitializer dungeonInitializer;

        private RoomManager roomManager;

        private int[,] floorplan;
        HashSet<(int, int)> occupiedCells = new HashSet<(int, int)>();
        private int totalLoop = 0;

        public void GenerateDungeon() {
            dungeonInitializer.InitValues();
            floorplan = dungeonInitializer.GetFloorPlan();
            roomManager = new RoomManager(dungeonInitializer);
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
            CreateRoomListAndSetFloorPlan();
            TryGenerateRooms();
            // TryGenerateRooms1000Times();
        }

        private void ManageSpecialRooms() {
            var (listOf2Neighbors, listOf3Neighbors) = CountNeighborsAndCreateHashset();
            AddRoomInListAndFloorplan(listOf2Neighbors, listOf3Neighbors);
        }

        private (HashSet<(int, int)>, HashSet<(int, int)>) CountNeighborsAndCreateHashset() {
            HashSet<(int, int)> emptyCellsWithMoreThan2Neighbors = new HashSet<(int, int)>();
            HashSet<(int, int)> emptyCellsWithMoreThan3Neighbors = new HashSet<(int, int)>();
            var directions = Utilities.GetDirection();

            foreach ((int row, int col) in occupiedCells) {

                foreach (var direction in directions) {
                    int newRow = row + direction[0];
                    int newCol = col + direction[1];

                    if (Utilities.CheckIsInBounds(newRow, newCol, dungeonInitializer.GetFloorPlanBound()) && floorplan[newRow, newCol] == 0) {
                        int occupiedNeighbors = CountOccupiedNeighbors(floorplan, newRow, newCol);
                        if (occupiedNeighbors == 2) {
                            emptyCellsWithMoreThan2Neighbors.Add((newRow, newCol));
                        }
                        if (occupiedNeighbors >= 3) {
                            emptyCellsWithMoreThan3Neighbors.Add((newRow, newCol));
                        }
                    }
                }
            }

            return (emptyCellsWithMoreThan2Neighbors, emptyCellsWithMoreThan3Neighbors);
        }

        private void AddRoomInListAndFloorplan(HashSet<(int, int)> listOf2Neighbors, HashSet<(int, int)> listOf3Neighbors) {
            // todo -> ajouter la gestion de si listOf3Neighbors == 0 alors le faire sur 2 ! et revoir le code du coup..
            foreach ((int x, int y) in listOf3Neighbors) {
                Room Room = new Room_R1X1(new Vector2Int(x, y));
                Room.SetRoomType(RoomTypeEnum.SECRET);
                roomManager.AddRoom(Room);
                floorplan[x, y] = 1;
                occupiedCells.Add((x, y));
            }
        }

        private int CountOccupiedNeighbors(int[,] grid, int row, int col) {

            int count = 0;

            foreach (var direction in Utilities.GetDirection()) {
                int newRow = row + direction[0];
                int newCol = col + direction[1];

                if (Utilities.CheckIsInBounds(newRow, newCol, dungeonInitializer.GetFloorPlanBound()) && grid[newRow, newCol] > 0) {
                    count++;
                }
            }

            return count;
        }

        private GameObject LoadRoomPrefab(DifficultyEnum diff, RoomShapeEnum shape, RoomTypeEnum type) {
            try {
                BiomeEnum biome = dungeonInitializer.GetBiome();
                List<string> rooms = dungeonInitializer.GetRoomConfigDictionary()[biome][diff][type][shape];
                if (rooms.Count == 0) {
                    throw new ArgumentNullException("CreateRooms : no room available for this configuration : " + biome + "/" + diff + "/" + type + "/" + shape);
                }
                int rnd = dungeonInitializer.GetNextRandomValue(rooms.Count); // Using System.Random with Seed
                return Resources.Load<GameObject>(GlobalConfig.Instance.PrefabRoomsVariantsPath + rooms[rnd]);
            } catch (ArgumentNullException ex) {
                Debug.LogError("Error loading room prefab: " + ex.Message);
                return null;
            }
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
            GameObject roomGo = LoadRoomPrefab(difficulty, shape, Room.GetRoomTypeEnum);
            Vector2Int worldPos = Room.GetWorldPosition();
            RoomGO roomGO = Instantiate(roomGo, new Vector3(worldPos.x, worldPos.y, 0), transform.rotation, dungeonInitializer.GetFloorGameObject().transform).GetComponent<RoomGO>();
            roomGO.Setup(worldPos, shape);

            CreateDoorsGo(Room, roomGO);
        }

        private void CreateDoorsGo(Room Room, RoomGO roomGO) {
            Room.SearchNeighborsAndCreateDoor(floorplan, dungeonInitializer.GetFloorPlanBound(), dungeonInitializer.GetBiome());
            if (Room.GetDoors().Count > 0) {
                foreach (Door door in Room.GetDoors()) {
                    GameObject doorGo = Instantiate(Resources.Load<GameObject>(GlobalConfig.Instance.PrefabDoorsPath + "Door"), Vector3.zero, transform.rotation);
                    doorGo.GetComponent<DoorGO>().SetDirection(door.GetDirection());
                    doorGo.transform.SetParent(roomGO.DoorsContainer.transform);
                    doorGo.transform.localPosition = door.LocalPosition;
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

        private void CreateRoomListAndSetFloorPlan() {
            List<RoomShapeEnum> roomShapes = GetListOfSpecialShapes();
            int currentShapeIndex = 0;
            Vector2Int vectorStart = dungeonInitializer.GetVectorStart();
            Room starterRoom = new Room_R1X1(vectorStart);
            starterRoom.SetRoomType(RoomTypeEnum.STARTER);
            roomManager.AddRoom(starterRoom);
            SetFloorPlan(starterRoom, vectorStart, 1);
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            queue.Enqueue(vectorStart);

            while (queue.Count > 0 && roomManager.GetListOfRoom().Count < dungeonInitializer.GetNumberOfRooms()) {
                Vector2Int roomQueue = DequeueRandomElement(queue);
                Room room = roomManager.GenerateRoom(roomShapes, ref currentShapeIndex);
                List<Vector2Int> listOfEmptySpaces = GetEmptySpaces(room, roomQueue);

                if (listOfEmptySpaces.Count > 0) {
                    int randomNeighbor = dungeonInitializer.GetNextRandomValue(listOfEmptySpaces.Count);
                    Vector2Int randomCell = listOfEmptySpaces[randomNeighbor];
                    queue.Enqueue(randomCell);
                    room.SetPosition(randomCell);
                    room.SetRoomType(RoomTypeEnum.STANDARD);
                    roomManager.AddRoom(room);
                    SetFloorPlan(room, randomCell, roomManager.GetListOfRoom().Count + 1);
                }

            }
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
                if (!Utilities.CheckIsOutOfBound(checkNewPlace, dungeonInitializer.GetFloorPlanBound())) {
                    int neighbour = floorplan[checkNewPlace.x, checkNewPlace.y] > 0 ? 1 : 0;
                    count += neighbour;
                }
            }
            return count;
        }

        private bool CheckIsEmptySpace(Vector2Int vector, Room room) {
            Vector2Int[] cells = room.GetOccupiedCells(vector);
            int usedCells = cells.Sum(cell => Utilities.CheckIsOutOfBound(cell, dungeonInitializer.GetFloorPlanBound()) ? 1 : floorplan[cell.x, cell.y]);
            return usedCells == 0;
        }

    }
}