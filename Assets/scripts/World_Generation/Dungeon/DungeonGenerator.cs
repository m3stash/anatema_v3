using System.Collections.Generic;
using UnityEngine;
using RoomNs;
using DoorNs;
using System;
using Random = UnityEngine.Random;
using System.Linq;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DungeonNs {

    public class Generator : MonoBehaviour {

        // InitValues
        private int[,] floorplan;
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
            GenerateRooms();
            SetSpecialRooms();
            CreateRoomGO();
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

        private void GenerateRooms() {
            InitGenerateValues();
            CreatePseudoRoomListAndSetFloorPlan();
            // if all the pieces have not been successfully placed then we start again
            if (listOfPseudoRoom.Count < dungeonValues.GetNumberOfRooms()) {
                totalLoop++;
                if (totalLoop <= 100) {
                    GenerateRooms();
                } else {
                    Debug.LogError("TRY GenerateRooms call == 100 tries");
                }
            } else {
                Debug.Log("Total Loop for current Generation " + totalLoop);
            }
        }

        private void SetSpecialRooms() {
            /*foreach (Vector2Int room in endRooms) {
                Debug.Log("ICI" + room);
            }*/
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

        private void CreateRoomGO() {
            foreach (KeyValuePair<DifficultyEnum, float> values in roomRepartition) {

                DifficultyEnum diff = values.Key;
                // for each difficulties
                for (var i = 0; i < values.Value; i++) {

                    PseudoRoom pRoom = GetNextPseudoRoom();
                    if (pRoom == null) {
                        Debug.LogError("CreateRooms : No more pseudo rooms available");
                        return;
                    }

                    RoomShapeEnum shape = pRoom.GetShape();
                    DifficultyEnum difficulty = pRoom.GetRoomTypeEnum == RoomTypeEnum.STANDARD ? diff : DifficultyEnum.DEFAULT;
                    GameObject roomGo = LoadRoomPrefab(difficulty, shape, pRoom.GetRoomTypeEnum);
                    Vector2Int worldPos = pRoom.GetWorldPosition();
                    Room room = Instantiate(roomGo, new Vector3(worldPos.x, worldPos.y, 0), transform.rotation).GetComponent<Room>();
                    room.transform.parent = floorGO.transform;
                    // Room room = Instantiate(roomGo, new Vector3(worldPos.x, worldPos.y, 0), transform.rotation, floorGO.transform).GetComponent<Room>(); // TODO : try to find why prefab is hidden...
                    room.transform.parent = floorGO.transform;
                    room.Setup(worldPos, shape);

                    CreateDoorsGo(pRoom, room);
                }
            }
        }

        private void CreateDoorsGo(PseudoRoom pRoom, Room room) {
            pRoom.SeachNeighborsAndCreateDoor(listOfPseudoRoom);
            if (pRoom.GetDoors().Count > 0) {
                foreach (PseudoDoor door in pRoom.GetDoors()) {
                    GameObject doorGo = Instantiate(Resources.Load<GameObject>(GlobalConfig.prefabDoorsPath + "Door"), Vector3.zero, transform.rotation);
                    doorGo.GetComponent<Door>().SetDirection(door.GetDirection());
                    doorGo.transform.SetParent(room.DoorsContainer.transform);
                    doorGo.transform.localPosition = door.GetLocalPosition();
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

        private void SetFloorPlan(PseudoRoom room, Vector2Int vector) {
            foreach (var cell in room.GetCellToVerify(vector)) {
                floorplan[cell.x, cell.y] = 1;
            }
        }

        private bool CanAddShape(Vector2Int vector, PseudoRoom room) {
            if (!CheckIsEmptySpace(vector, room)) {
                return false;
            }
            if (NeighbourCount(vector, room) > 1) {
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
            SetFloorPlan(starterRoom, vectorStart);
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            queue.Enqueue(vectorStart);

            while (queue.Count > 0 && listOfPseudoRoom.Count < dungeonValues.GetNumberOfRooms()) {
                Vector2Int roomQueue = DequeueRandomElement(queue);
                PseudoRoom pRoom = GeneratePseudoRoom(roomShapes, random, ref currentShapeIndex);
                List<Vector2Int> listOfEmptySpaces = GetEmptySpaces(pRoom, roomQueue);

                if (listOfEmptySpaces.Count > 0) {
                    int randomNeighbor = Random.Range(0, listOfEmptySpaces.Count);
                    queue.Enqueue(listOfEmptySpaces[randomNeighbor]);
                    pRoom.SetPosition(listOfEmptySpaces[randomNeighbor]);
                    pRoom.SetRoomType(RoomTypeEnum.STANDARD);
                    listOfPseudoRoom.Add(pRoom);
                    SetFloorPlan(pRoom, listOfEmptySpaces[randomNeighbor]);
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

        private List<Vector2Int> GetEmptySpaces(PseudoRoom room, Vector2Int roomQueue) {
            return room.GetDirections(roomQueue)
                .Where(direction => CanAddShape(direction, room))
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

        private int NeighbourCount(Vector2Int vector, PseudoRoom room) {
            int count = 0;
            Vector2Int[] shapesToCheck = room.GetNeighborsCells(vector);

            if (shapesToCheck.Length == 0) {
                return 99;
            }
            foreach (var checkNewPlace in shapesToCheck) {
                if (!CheckIsOutOfBound(checkNewPlace, floorplanBound)) {
                    count += floorplan[checkNewPlace.x, checkNewPlace.y];
                }
            }
            return count;
        }

        private bool CheckIsOutOfBound(Vector2Int vector, int bound) {
            return vector.x < 0 || vector.x > bound || vector.y > bound || vector.y < 0;
        }

        private bool CheckIsEmptySpace(Vector2Int vector, PseudoRoom room) {
            Vector2Int[] cells = room.GetCellToVerify(vector);
            int usedCells = cells.Sum(cell => CheckIsOutOfBound(cell, floorplanBound) ? 1 : floorplan[cell.x, cell.y]);
            return usedCells == 0;
        }

    }
}