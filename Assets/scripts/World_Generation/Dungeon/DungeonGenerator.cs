using System.Collections.Generic;
using UnityEngine;
using RoomNs;
using DoorNs;
using System;
using Random = UnityEngine.Random;
using Unity.Mathematics;
using System.Net;
using UnityEngine.TextCore.Text;
using static UnityEditor.VersionControl.Asset;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DungeonNs {

    public class Generator : MonoBehaviour {

        private Vector2Int roomSize = new Vector2Int(61, 31);

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

        public void StartGeneration() {
            InitValues();
            // CreatePool();
            GenerateRooms();
            SetSpecialRooms();
            ManageRoomsDoors();
            CreateRooms();
        }

        private void InitValues() {
            Config config = GameManager.GetCurrentDungeonConfig();
            biome = config.GetBiomeType();
            dungeonValues = DungeonValueGeneration.CreateRandomValues(GameManager.GetSeed, config.GetCurrentFloorNumber());
            RoomRepartition.SetRoomRepartition(config.GetDifficulty(), dungeonValues.GetNumberOfRooms(), roomRepartition);
            floorplan = new int[12, 12];
            int bound = floorplan.GetLength(0);
            floorplanBound = bound - 1;
            vectorStart = new Vector2Int((bound / 2) - 1, (bound / 2) - 1);
            totalLoop = 0;
        }

        private void GenerateRooms() {
            InitGenerateValues();
            int totalRoomPlaced = TryToGenerateAllRoomsFloor();
            // if all the pieces have not been successfully placed then we start again
            if (totalRoomPlaced < dungeonValues.GetNumberOfRooms()) {
                totalLoop++;
                if (totalLoop < 50) {
                    GenerateRooms();
                } else {
                    Debug.LogError("NOMBRE ESSAI > 500");
                }
            } else {
                Debug.Log("END " + totalLoop);
            }
        }

        private void SetSpecialRooms() {
            /*foreach (Vector2Int room in endRooms) {
                Debug.Log("ICI" + room);
            }*/
        }

        private void ManageRoomsDoors() {
            foreach (PseudoRoom room in listOfPseudoRoom) {
                room.SeachNeighborsAndCreateDoor(listOfPseudoRoom);
            }
        }

        private GameObject RoomInstanciation(DifficultyEnum diff, RoomShapeEnum shape, RoomTypeEnum type) {

            List<string> rooms = roomDico[biome][diff][type][shape];
            if (rooms.Count == 0) {
                Debug.LogError("CreateRooms : no room available for this configuration : " + biome + "/" + diff + "/" + type + "/" + shape);
                return null;
            }

            int rnd = Random.Range(0, rooms.Count - 1); // toDo ne pas gérer ça ici !!!!

            return Resources.Load<GameObject>(GlobalConfig.prefabRoomsVariantsPath + rooms[rnd]);
        }

        private void CreateRooms() {

            GameObject floorGO = GameManager.GetFloorContainer();

            foreach (KeyValuePair<DifficultyEnum, float> values in roomRepartition) {

                DifficultyEnum diff = values.Key;

                for (var i = 0; i < values.Value; i++) {

                    PseudoRoom room = null;
                    GameObject roomGo = null;

                    room = listOfPseudoRoom[0];
                    listOfPseudoRoom.RemoveAt(0);

                    RoomShapeEnum shape = room.GetShape();

                    roomGo = RoomInstanciation(diff, shape, room.GetRoomTypeEnum);

                    if (roomGo == null) {
                        Debug.LogError("CreateRooms : Room GameObject is null");
                        return;
                    }

                    Vector2Int roomPos = room.GetPosition();
                    Vector2Int pos = new Vector2Int(roomPos.x * roomSize.x, roomPos.y * roomSize.y);

                    Room obj = Instantiate(roomGo, new Vector3(pos.x, pos.y, 0), transform.rotation).GetComponent<Room>();
                    obj.transform.parent = floorGO.transform;
                    obj.Setup(roomPos, room.GetShape());
                    if (room.GetDoors().Count > 0) {
                        foreach (PseudoDoor door in room.GetDoors()) {
                            GameObject doorGo = Instantiate(Resources.Load<GameObject>(GlobalConfig.prefabDoorsPath + "Door"), Vector3.zero, transform.rotation);
                            doorGo.GetComponent<Door>().SetDirection(door.GetDirection());
                            doorGo.transform.SetParent(obj.DoorsContainer.transform);
                            doorGo.transform.localPosition = door.GetLocalPosition();
                        }
                    }

                }
            }
        }

        private void InitGenerateValues() {
            floorplan = new int[12, 12];
            listOfPseudoRoom = new List<PseudoRoom>();
        }

        private void SetFloorPlan(PseudoRoom room, Vector2Int vector) {

            Vector2Int[] cells = room.GetCellToVerify(vector);
            for (int i = 0; i < cells.Length; i++) {
                floorplan[cells[i].x, cells[i].y] = 1;
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
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private bool IsRatioSpecialShape(List<PseudoRoom> rooms) {
            int specials = 0;
            int standard = 0;
            int totalShapes = rooms.Count;
            rooms.ForEach(r => {
                if(r.GetShape() == RoomShapeEnum.R1X1) {
                    standard++;
                } else {
                    specials++;
                }
            });
            double currentRatio = (double)specials / totalShapes;
            return currentRatio  <= 0.25;
        }

        private int TryToGenerateAllRoomsFloor() {

            //// toDO voir à bouger ce qui ne doit être calculer qu'une fois par level et non à chaque essaie de génération !!!
            string seed = GameManager.GetSeed;
            int seedHash = seed.GetHashCode();
            System.Random random = new System.Random(seedHash);
            int currentShapeIndex = 0;

            // toDo dynamiser avec une boucle
            List<RoomShapeEnum> roomShapes = new List<RoomShapeEnum> {
                //RoomShapeEnum.R1X1,
                RoomShapeEnum.R2X2,
                RoomShapeEnum.R2X1,
                RoomShapeEnum.R1X2,
            };

            PseudoRoom starterRoom = new Room_R1X1(vectorStart);
            listOfPseudoRoom.Add(starterRoom);
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            queue.Enqueue(vectorStart);
            SetFloorPlan(starterRoom, vectorStart);

            // Start BFS Search Pattern
            while (queue.Count > 0 && listOfPseudoRoom.Count < dungeonValues.GetNumberOfRooms()) {

                Vector2Int roomQueue = queue.Dequeue();

                PseudoRoom room;
                
                RoomShapeEnum newRoomShape = RoomShapeEnum.R1X1;

                if (IsRatioSpecialShape(listOfPseudoRoom)) {
                    ShuffleShapes(roomShapes, random);
                    newRoomShape = roomShapes[currentShapeIndex];
                    currentShapeIndex = (currentShapeIndex + 1) % roomShapes.Count;
                }

                // use reflexion to create instance dynamically
                Type classType = Type.GetType("Room_" + newRoomShape.ToString());
                if (classType != null && typeof(PseudoRoom).IsAssignableFrom(classType)) {
                    room = (PseudoRoom)Activator.CreateInstance(classType);
                } else {
                    Debug.LogError("TryToGenerateAllRoomsFloor: Shape does not exist");
                    room = null;
                }

                List<Vector2Int> listOfEmptySpaces = new List<Vector2Int>();
                Vector2Int[] directions = room.GetDirections(roomQueue);

                for (int i = 0; i < directions.Length; i++) {
                    if (CanAddShape(directions[i], room)) {
                        listOfEmptySpaces.Add(directions[i]);
                    }
                }

                switch (newRoomShape) {
                    case RoomShapeEnum.R1X1: {

                        if (listOfEmptySpaces.Count > 0) {
                            for (int i = 0; i < listOfEmptySpaces.Count; i++) {
                                queue.Enqueue(listOfEmptySpaces[i]);
                                listOfPseudoRoom.Add(new Room_R1X1(listOfEmptySpaces[i]));
                                SetFloorPlan(room, listOfEmptySpaces[i]);
                            }
                        }
                        break;
                    }
                    default: {
                        if (listOfEmptySpaces.Count > 0) {
                            int randomNeightbor = Random.Range(0, listOfEmptySpaces.Count);
                            queue.Enqueue(listOfEmptySpaces[randomNeightbor]);
                            room.SetPosition(listOfEmptySpaces[randomNeightbor]);
                            listOfPseudoRoom.Add(room);
                            SetFloorPlan(room, listOfEmptySpaces[randomNeightbor]);
                            /*bool isPlaced = false;
                            for (int i = 0; i < listOfEmptySpaces.Count; i++) {
                                int random = Random.Range(0, 2);
                                if (listOfEmptySpaces.Count > 1 && random == 1)
                                    continue;
                                if (!isPlaced) {
                                    isPlaced = true;
                                    queue.Enqueue(listOfEmptySpaces[i]);
                                    listOfPseudoRoom.Add(new PseudoRoom(listOfEmptySpaces[i], RoomTypeEnum.STANDARD, newRoomShape));
                                    SetFloorPlan(shape, listOfEmptySpaces[i]);
                                } else {
                                    queue.Enqueue(listOfEmptySpaces[i]);
                                    listOfPseudoRoom.Add(new PseudoRoom(listOfEmptySpaces[i], RoomTypeEnum.STANDARD, RoomShapeEnum.R1X1));
                                    SetFloorPlan(new Room_R1X1(), listOfEmptySpaces[i]);
                                }
                            }*/
                        }
                        break;
                    }
                }
                /*if (createdCount == 0) {
                    endRooms.Add(vector);
                    listOfPseudoRoom.Find(room => room.GetPosition() == vector).SetIsEndRoom(true);
                }*/
            }
            return listOfPseudoRoom.Count;

        }

        private int NeighbourCount(Vector2Int vector, PseudoRoom room) {

            int count = 0;
            Vector2Int[] shapesToCheck = room.GetNeighborsCells(vector);
            /* case
            * if x or y == 1 
            * || x or y == bound -1 
            * if search to check neighbor of big room like 2x2 so -1 + -2 is out of bound!
            */
            if (shapesToCheck.Length == 0) {
                return 99;
            }
            foreach (var checkNewPlace in shapesToCheck) {
                if (CheckIsOutOfBound(checkNewPlace, floorplanBound))
                    continue;
                count += floorplan[checkNewPlace.x, checkNewPlace.y];
            }
            return count;
        }

        private bool CheckIsOutOfBound(Vector2Int vector, int bound) {
            return vector.x < 0 || vector.x > bound || vector.y > bound || vector.y < 0;
        }

        private bool CheckIsEmptySpace(Vector2Int vector, PseudoRoom room) {

            Vector2Int[] cells = room.GetCellToVerify(vector);
            int usedCells = 0;
            for (int i = 0; i < cells.Length; i++) {
                if (CheckIsOutOfBound(cells[i], floorplanBound)) {
                    usedCells += 1;
                } else {
                    usedCells += floorplan[cells[i].x, cells[i].y];
                }
            }
            return usedCells == 0;

        }

    }
}