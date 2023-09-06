using System.Collections.Generic;
using UnityEngine;
using RoomNs;
using DoorNs;
using System;
using Random = UnityEngine.Random;
using System.Linq;
using static UnityEngine.Rendering.DebugUI.Table;
using System.Collections;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering.Universal;
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
        private List<PseudoRoom> listOfPseudoRoom;
        private Dictionary<BiomeEnum, Dictionary<DifficultyEnum, Dictionary<RoomTypeEnum, Dictionary<RoomShapeEnum, List<string>>>>> roomDico = RoomsJsonConfig.GetRoomDictionary();
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
            SetSpecialRooms();
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

        private bool IsRoomGenerationUnsuccessful() {
            return listOfPseudoRoom.Count < dungeonValues.GetNumberOfRooms();
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
            CreatePseudoRoomListAndSetFloorPlan();
            TryGenerateRooms();
        }

        private void SetSpecialRooms() {
            HashSet<(int, int)> emptyCellsWithMoreThan2Neighbors = new HashSet<(int, int)>();
            HashSet<(int, int)> emptyCellsWithMoreThan3Neighbors = new HashSet<(int, int)>();

            foreach ((int row, int col) in occupiedCells) {

                int[][] directions = new int[][] {
                    new int[] { -1, 0 }, // Up
                    new int[] { 1, 0 },  // Down
                    new int[] { 0, -1 }, // Left
                    new int[] { 0, 1 }   // Right
                };
                foreach (var direction in directions) {
                    int newRow = row + direction[0];
                    int newCol = col + direction[1];

                    if (Utilities.CheckIsOnOfBound(newRow, newCol, floorplanBound) && floorplan[newRow, newCol] == 0) {
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

            foreach ((int x, int y) in emptyCellsWithMoreThan3Neighbors) {
                Debug.Log($"X: {x}, Y: {y}");
                PseudoRoom pseudoRoom = new Room_R1X1(new Vector2Int(x, y));
                pseudoRoom.SetRoomType(RoomTypeEnum.SECRET);
                listOfPseudoRoom.Add(pseudoRoom);
                floorplan[x, y] = 1;
                occupiedCells.Add((x, y));
            }
            

            Debug.Log("Empty cells with more than 3occupied neighbors: " + emptyCellsWithMoreThan3Neighbors.Count);
        }

        private int CountOccupiedNeighbors(int[,] grid, int row, int col) {

            int count = 0;

            int[][] directions = new int[][] {
                new int[] { -1, 0 }, // Up
                new int[] { 1, 0 },  // Down
                new int[] { 0, -1 }, // Left
                new int[] { 0, 1 }   // Right
            };

            foreach (var direction in directions) {
                int newRow = row + direction[0];
                int newCol = col + direction[1];

                if (Utilities.CheckIsOnOfBound(newRow, newCol, floorplanBound) && grid[newRow, newCol] > 0) {
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
                return Resources.Load<GameObject>(GlobalConfig.prefabRoomsVariantsPath + rooms[rnd]);
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

                    PseudoRoom pseudoRoom = GetNextPseudoRoom();
                    if (pseudoRoom == null) {
                        Debug.LogError("CreateRooms : No more pseudo rooms available");
                        return;
                    }

                    DifficultyEnum difficulty = pseudoRoom.GetRoomTypeEnum == RoomTypeEnum.STANDARD ? diff : DifficultyEnum.DEFAULT;
                    InstanciateGoRoomAndDoors(pseudoRoom, difficulty);
                }
            }
            
        }

        private void CreateSecretRoomGo() {
            foreach (PseudoRoom pseudoRoom in listOfPseudoRoom) {
                InstanciateGoRoomAndDoors(pseudoRoom, DifficultyEnum.DEFAULT);
            }
        }


        private void InstanciateGoRoomAndDoors(PseudoRoom pseudoRoom, DifficultyEnum difficulty) {
            RoomShapeEnum shape = pseudoRoom.GetShape();
            GameObject roomGo = LoadRoomPrefab(difficulty, shape, pseudoRoom.GetRoomTypeEnum);
            Vector2Int worldPos = pseudoRoom.GetWorldPosition();
            Room room = Instantiate(roomGo, new Vector3(worldPos.x, worldPos.y, 0), transform.rotation, floorGO.transform).GetComponent<Room>();
            room.Setup(worldPos, shape);

            CreateDoorsGo(pseudoRoom, room);
        }

        private void CreateDoorsGo(PseudoRoom pseudoRoom, Room room) {
            pseudoRoom.SeachNeighborsAndCreatePseudoDoor(floorplan, floorplanBound, biome);
            if (pseudoRoom.GetDoors().Count > 0) {
                foreach (PseudoDoor door in pseudoRoom.GetDoors()) {
                    GameObject doorGo = Instantiate(Resources.Load<GameObject>(GlobalConfig.prefabDoorsPath + "Door"), Vector3.zero, transform.rotation);
                    doorGo.GetComponent<Door>().SetDirection(door.GetDirection());
                    doorGo.transform.SetParent(room.DoorsContainer.transform);
                    doorGo.transform.localPosition = door.LocalPosition;
                }
            }
        }

        private PseudoRoom GetNextPseudoRoom() {
            if (listOfPseudoRoom.Count > 0) {
                PseudoRoom room = listOfPseudoRoom[0];
                listOfPseudoRoom.RemoveAt(0);
                return room;
            }
            return null;
        }

        private void InitGenerateValues() {
            floorplan = new int[12, 12];
            listOfPseudoRoom = new List<PseudoRoom>();
        }

        private void SetFloorPlan(PseudoRoom room, Vector2Int vector, int index) {
            foreach (var cell in room.GetOccupiedCells(vector)) {
                floorplan[cell.x, cell.y] = index;
                occupiedCells.Add((cell.x, cell.y));
            }
        }

        private bool CanAddShape(Vector2Int vector, PseudoRoom room) {
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

        private bool CheckProportionalShapeDistribution(List<PseudoRoom> rooms) {
            int specials = rooms.Count(r => r.GetShape() != RoomShapeEnum.R1X1);
            double currentRatio = (double)specials / rooms.Count;
            return currentRatio <= 0.25;
        }

        private void CreatePseudoRoomListAndSetFloorPlan() {
            // init values
            List<RoomShapeEnum> roomShapes = GetListOfSpecialShapes();
            int currentShapeIndex = 0;
            PseudoRoom starterRoom = new Room_R1X1(vectorStart);
            starterRoom.SetRoomType(RoomTypeEnum.STARTER);
            listOfPseudoRoom.Add(starterRoom);
            SetFloorPlan(starterRoom, vectorStart, 1);
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            queue.Enqueue(vectorStart);

            while (queue.Count > 0 && listOfPseudoRoom.Count < dungeonValues.GetNumberOfRooms()) {
                Vector2Int roomQueue = DequeueRandomElement(queue);
                PseudoRoom pRoom = GeneratePseudoRoom(roomShapes, random, ref currentShapeIndex);
                List<Vector2Int> listOfEmptySpaces = GetEmptySpaces(pRoom, roomQueue);

                if (listOfEmptySpaces.Count > 0) {
                    int randomNeighbor = Random.Range(0, listOfEmptySpaces.Count);
                    Vector2Int randomCell = listOfEmptySpaces[randomNeighbor];
                    queue.Enqueue(randomCell);
                    pRoom.SetPosition(randomCell);
                    pRoom.SetRoomType(RoomTypeEnum.STANDARD);
                    listOfPseudoRoom.Add(pRoom);
                    SetFloorPlan(pRoom, randomCell, listOfPseudoRoom.Count + 1);
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

        private List<Vector2Int> GetEmptySpaces(PseudoRoom room, Vector2Int position) {
            return room.GetDirections(position)
                .Where(Vector2Int => CanAddShape(Vector2Int, room))
                .ToList();
        }

        private PseudoRoom GeneratePseudoRoom(List<RoomShapeEnum> roomShapes, System.Random random, ref int currentShapeIndex) {
            try {
                RoomShapeEnum newRoomShape = RoomShapeEnum.R1X1;
                if (CheckProportionalShapeDistribution(listOfPseudoRoom)) {
                    ShuffleShapes(roomShapes, random);
                    newRoomShape = roomShapes[currentShapeIndex];
                    currentShapeIndex = (currentShapeIndex + 1) % roomShapes.Count;
                }
                Type classType = Type.GetType("Room_" + newRoomShape.ToString());
                if (classType != null && typeof(PseudoRoom).IsAssignableFrom(classType)) {
                    return (PseudoRoom)Activator.CreateInstance(classType);
                } else {
                    throw new TypeLoadException("TryToGenerateAllRoomsFloor: Shape does not exist");
                }
            } catch (TypeLoadException ex) {
                Debug.LogError("Error generating PseudoRoom: " + ex.Message);
                return null;
            }
        }

        private int NeighborCount(Vector2Int vector, PseudoRoom room) {
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

        private bool CheckIsEmptySpace(Vector2Int vector, PseudoRoom room) {
            Vector2Int[] cells = room.GetOccupiedCells(vector);
            int usedCells = cells.Sum(cell => Utilities.CheckIsOutOfBound(cell, floorplanBound) ? 1 : floorplan[cell.x, cell.y]);
            return usedCells == 0;
        }

    }
}