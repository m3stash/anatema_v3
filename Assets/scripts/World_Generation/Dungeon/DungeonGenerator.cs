using System.Collections.Generic;
using UnityEngine;
using RoomNs;
using DoorNs;
using System;
using Random = UnityEngine.Random;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DungeonNs {

    public class Generator : MonoBehaviour {

        // InitValues
        private int[,] floorplan;
        HashSet<(int, int)> occupiedCells = new HashSet<(int, int)>();
        private int floorplanBound;
        private int totalLoop = 0;
        private Vector2Int vectorStart;
        private List<Room> listOfRoom;
        private RoomConfigDictionary roomDico = RoomsJsonConfig.LoadRoomDictionary();
        // private Pool<Room> roomPool;
        private Dictionary<DifficultyEnum, float> roomRepartition = new Dictionary<DifficultyEnum, float>();
        private BiomeEnum biome;
        private DungeonValues dungeonValues;
        private System.Random random;
        private GameObject floorGO;

        public void StartGeneration() {
            InitValues();
            // CreatePool();
            GenerateAndPlaceRooms();
            ManageSpecialRooms();
            CreateRoomsGO();
        }

        private void InitValues() {
            floorGO = GameManager.GetFloorContainer();
            Config config = GameManager.GetCurrentDungeonConfig();
            biome = config.GetBiomeType();
            dungeonValues = DungeonValueGeneration.CreateRandomValues(GameManager.GetSeed, config.GetCurrentFloorNumber());
            RoomRepartition.SetRoomRepartition(config.GetDifficulty(), dungeonValues.GetNumberOfRooms(), roomRepartition);
            floorplan = new int[12, 12];
            int bound = floorplan.GetLength(0);
            floorplanBound = bound - 1;
            vectorStart = new Vector2Int((bound / 2) - 1, (bound / 2) - 1);
            totalLoop = 0;

            string seed = GameManager.GetSeed;
            int seedHash = seed.GetHashCode();
            random = new System.Random(seedHash);
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
            return listOfRoom.Count < dungeonValues.GetNumberOfRooms();
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
            InitGenerateValues();
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

                    if (Utilities.CheckIsInBounds(newRow, newCol, floorplanBound) && floorplan[newRow, newCol] == 0) {
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
                listOfRoom.Add(Room);
                floorplan[x, y] = 1;
                occupiedCells.Add((x, y));
            }
        }

        private int CountOccupiedNeighbors(int[,] grid, int row, int col) {

            int count = 0;

            foreach (var direction in Utilities.GetDirection()) {
                int newRow = row + direction[0];
                int newCol = col + direction[1];

                if (Utilities.CheckIsInBounds(newRow, newCol, floorplanBound) && grid[newRow, newCol] > 0) {
                    count++;
                }
            }

            return count;
        }

        private GameObject LoadRoomPrefab(DifficultyEnum diff, RoomShapeEnum shape, RoomTypeEnum type) {
            try {
                List<string> rooms = roomDico[biome][diff][type][shape];
                if (rooms.Count == 0) {
                    throw new ArgumentNullException("CreateRooms : no room available for this configuration : " + biome + "/" + diff + "/" + type + "/" + shape);
                }
                int rnd = Random.Range(0, rooms.Count - 1); // toDo Gérer ça avec la SEED !!!
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
            foreach (KeyValuePair<DifficultyEnum, float> values in roomRepartition) {

                DifficultyEnum diff = values.Key;
                // for each difficulties
                for (var i = 0; i < values.Value; i++) {

                    Room Room = GetNextRoom();
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
            foreach (Room Room in listOfRoom) {
                InstanciateGoRoomAndDoors(Room, DifficultyEnum.DEFAULT);
            }
        }


        private void InstanciateGoRoomAndDoors(Room Room, DifficultyEnum difficulty) {
            RoomShapeEnum shape = Room.GetShape();
            GameObject roomGo = LoadRoomPrefab(difficulty, shape, Room.GetRoomTypeEnum);
            Vector2Int worldPos = Room.GetWorldPosition();
            RoomGO roomGO = Instantiate(roomGo, new Vector3(worldPos.x, worldPos.y, 0), transform.rotation, floorGO.transform).GetComponent<RoomGO>();
            roomGO.Setup(worldPos, shape);

            CreateDoorsGo(Room, roomGO);
        }

        private void CreateDoorsGo(Room Room, RoomGO roomGO) {
            Room.SearchNeighborsAndCreateDoor(floorplan, floorplanBound, biome);
            if (Room.GetDoors().Count > 0) {
                foreach (Door door in Room.GetDoors()) {
                    GameObject doorGo = Instantiate(Resources.Load<GameObject>(GlobalConfig.Instance.PrefabDoorsPath + "Door"), Vector3.zero, transform.rotation);
                    doorGo.GetComponent<DoorGO>().SetDirection(door.GetDirection());
                    doorGo.transform.SetParent(roomGO.DoorsContainer.transform);
                    doorGo.transform.localPosition = door.LocalPosition;
                }
            }
        }

        private Room GetNextRoom() {
            if (listOfRoom.Count > 0) {
                Room room = listOfRoom[0];
                listOfRoom.RemoveAt(0);
                return room;
            }
            return null;
        }

        private void InitGenerateValues() {
            floorplan = new int[12, 12];
            listOfRoom = new List<Room>();
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

        // use Knuth Algorithm to random shuffle to ensure that room shapes are evenly distributed throughout the level.
        private void ShuffleShapes<T>(List<T> list, System.Random random) {
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = random.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]); // tuples desconstruction
            }
        }

        private bool CheckProportionalShapeDistribution(List<Room> rooms) {
            int specials = rooms.Count(r => r.GetShape() != RoomShapeEnum.R1X1);
            double currentRatio = (double)specials / rooms.Count;
            return currentRatio <= 0.25;
        }

        private void CreateRoomListAndSetFloorPlan() {
            List<RoomShapeEnum> roomShapes = GetListOfSpecialShapes();
            int currentShapeIndex = 0;
            Room starterRoom = new Room_R1X1(vectorStart);
            starterRoom.SetRoomType(RoomTypeEnum.STARTER);
            listOfRoom.Add(starterRoom);
            SetFloorPlan(starterRoom, vectorStart, 1);
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            queue.Enqueue(vectorStart);

            while (queue.Count > 0 && listOfRoom.Count < dungeonValues.GetNumberOfRooms()) {
                Vector2Int roomQueue = DequeueRandomElement(queue);
                Room room = GenerateRoom(roomShapes, random, ref currentShapeIndex);
                List<Vector2Int> listOfEmptySpaces = GetEmptySpaces(room, roomQueue);

                if (listOfEmptySpaces.Count > 0) {
                    int randomNeighbor = Random.Range(0, listOfEmptySpaces.Count);
                    Vector2Int randomCell = listOfEmptySpaces[randomNeighbor];
                    queue.Enqueue(randomCell);
                    room.SetPosition(randomCell);
                    room.SetRoomType(RoomTypeEnum.STANDARD);
                    listOfRoom.Add(room);
                    SetFloorPlan(room, randomCell, listOfRoom.Count + 1);
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
            int randomIndex = UnityEngine.Random.Range(0, queue.Count);
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

        private Room GenerateRoom(List<RoomShapeEnum> roomShapes, System.Random random, ref int currentShapeIndex) {
            try {
                RoomShapeEnum newRoomShape = RoomShapeEnum.R1X1;
                if (CheckProportionalShapeDistribution(listOfRoom)) {
                    ShuffleShapes(roomShapes, random);
                    newRoomShape = roomShapes[currentShapeIndex];
                    currentShapeIndex = (currentShapeIndex + 1) % roomShapes.Count;
                }
                Type classType = Type.GetType("Room_" + newRoomShape.ToString());
                if (classType != null && typeof(Room).IsAssignableFrom(classType)) {
                    return (Room)Activator.CreateInstance(classType);
                } else {
                    throw new TypeLoadException("TryToGenerateAllRoomsFloor: Shape does not exist");
                }
            } catch (TypeLoadException ex) {
                Debug.LogError("Error generating Room: " + ex.Message);
                return null;
            }
        }

        private int NeighborCount(Vector2Int vector, Room room) {
            int count = 0;
            Vector2Int[] shapesToCheck = room.GetNeighborsCells(vector);

            if (shapesToCheck.Length == 0) {
                return -1;
            }
            foreach (var checkNewPlace in shapesToCheck) {
                if (!Utilities.CheckIsOutOfBound(checkNewPlace, floorplanBound)) {
                    int neighbour = floorplan[checkNewPlace.x, checkNewPlace.y] > 0 ? 1 : 0;
                    count += neighbour;
                }
            }
            return count;
        }

        private bool CheckIsEmptySpace(Vector2Int vector, Room room) {
            Vector2Int[] cells = room.GetOccupiedCells(vector);
            int usedCells = cells.Sum(cell => Utilities.CheckIsOutOfBound(cell, floorplanBound) ? 1 : floorplan[cell.x, cell.y]);
            return usedCells == 0;
        }

    }
}