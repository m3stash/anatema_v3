using System.Collections.Generic;
using UnityEngine;
using RoomNs;
using DoorNs;

namespace DungeonNs {

    public class Generator : MonoBehaviour {

        private Vector2Int roomSize = new Vector2Int(61, 31);
        private int current_R2X2, current_R1X2, current_R2X1 = 0;

        // InitValues
        private int[,] floorplan;
        private int bound;
        private int roomMaxBound;
        private int totalLoop = 0;
        private int floorplanCount;
        private Vector2Int vectorStart;
        private List<Vector2Int> cellQueue;
        private List<Vector2Int> endRooms;
        private List<PseudoRoom> listOfPseudoRoom;
        private Dictionary<BiomeEnum, Dictionary<DifficultyEnum, Dictionary<RoomTypeEnum, Dictionary<RoomShapeEnum, List<string>>>>> roomDico = RoomsJsonConfig.GetRoomDictionary();
        // private Pool<Room> roomPool;
        private Dictionary<DifficultyEnum, float> roomRepartition = new Dictionary<DifficultyEnum, float>();
        private BiomeEnum biome;
        private DungeonValues dungeonValues;
        private RoomShapeEnum[] enumShapes;




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
            bound = floorplan.GetLength(0);
            roomMaxBound = floorplan.GetLength(0) - 2;
            vectorStart = new Vector2Int(bound / 2, bound / 2);
            enumShapes = DungeonValueGeneration.GenerateRandomShapeByBiome(dungeonValues.GetNumberOfRooms(), GameManager.GetSeed, GameManager.GetCurrentDungeonConfig().GetCurrentFloorNumber());
            totalLoop = 0;
        }

        private void GenerateRooms() {
            InitGenerateValues();
            InitSpawnRoom();
            int totalRoomPlaced = TryToGenerateAllRoomsFloor();
            // if all the pieces have not been successfully placed then we start again
            if (totalRoomPlaced < dungeonValues.GetNumberOfRooms()) {
                totalLoop++;
                GenerateRooms();
            } else {
                Debug.Log("END " + totalLoop);
                Debug.Log("max_R1X2 " + DungeonConsts.max_R1X2);
                Debug.Log("max_R2X1 " + DungeonConsts.max_R2X1);
                Debug.Log("max_R1X2 " + DungeonConsts.max_R1X2);
            }
        }

        private void SetSpecialRooms() {
            foreach (Vector2Int room in endRooms) {
                Debug.Log("ICI" + room);
            }
        }

        private void ManageRoomsDoors() {
            foreach (PseudoRoom room in listOfPseudoRoom) {
                room.SeachNeighborsAndCreateDoor(listOfPseudoRoom);
            }
        }

        private GameObject RoomInstanciation(GameObject roomGo, DifficultyEnum diff, RoomShapeEnum shape, RoomTypeEnum type) {

            List<string> rooms = roomDico[biome][diff][type][shape];
            if (rooms.Count == 0) {
                Debug.LogError("CreateRooms : no room available for this configuration : " + biome + "/" + diff + "/" + type + "/" + shape);
                return null;
            }

            int rnd = UnityEngine.Random.Range(0, rooms.Count - 1);

            return Resources.Load<GameObject>(GlobalConfig.prefabRoomsVariantsPath + rooms[rnd]);
        }

        private void CreateRooms() {

            GameObject floorGO = GameManager.GetFloorContainer();

            foreach (KeyValuePair<DifficultyEnum, float> values in roomRepartition) {

                DifficultyEnum diff = values.Key;

                for(var i = 0; i < values.Value; i++) {

                    PseudoRoom room = null;
                    GameObject roomGo = null;
                    
                    if (listOfPseudoRoom.Count > 0) {
                        room = listOfPseudoRoom[0];
                        listOfPseudoRoom.RemoveAt(0);
                    } else {
                        Debug.LogError("CreateRooms : error the number of parts to be added is greater than the number of parts distributed");
                        return;
                    }
                    
                    RoomShapeEnum shape = room.GetShape();

                    switch (room.GetRoomTypeEnum) {
                        case RoomTypeEnum.BOSS:
                            roomGo = RoomInstanciation(roomGo, diff, shape, RoomTypeEnum.BOSS);
                        break;
                        case RoomTypeEnum.STARTER:
                            roomGo = RoomInstanciation(roomGo, diff, shape, RoomTypeEnum.STARTER);
                        break;
                        case RoomTypeEnum.ITEMS:
                            roomGo = RoomInstanciation(roomGo, diff, shape, RoomTypeEnum.ITEMS);
                        break;
                        case RoomTypeEnum.STANDARD:
                            roomGo = RoomInstanciation(roomGo, diff, shape, RoomTypeEnum.STANDARD);
                        break;
                        default:
                            Debug.LogError("CreateRooms : No RoomTypeEnum find for : " + room.GetRoomTypeEnum);
                            return;
                    }

                    if(roomGo == null) {
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
            current_R2X2 = 0;
            current_R1X2 = 0;
            current_R2X1 = 0;
            cellQueue = new List<Vector2Int>();
            endRooms = new List<Vector2Int>();
            listOfPseudoRoom = new List<PseudoRoom>();
            floorplanCount = 0;
        }
        private void InitSpawnRoom() {
            Visit(vectorStart, RoomShapeEnum.R1X1);
            cellQueue.Add(vectorStart);
            listOfPseudoRoom.Add(new PseudoRoom(vectorStart, RoomTypeEnum.STARTER, RoomShapeEnum.R1X1));
        }

        private int TryToGenerateAllRoomsFloor() {

            floorplanCount = 1; // the first room is already create so we start at 1
            // Start BFS pattern
            while (cellQueue.Count > 0 && floorplanCount < dungeonValues.GetNumberOfRooms()) {

                Vector2Int vector = cellQueue[0];
                cellQueue.RemoveAt(0); // get the first of list
                int createdCount = 0;

                if (vector.x > 1) {
                    createdCount += CheckIsEmtyPlaceAndAddRoomToQueue(new Vector2Int(vector.x - 1, vector.y)) ? 1 : 0;
                }
                if (vector.x < roomMaxBound) {
                    createdCount += CheckIsEmtyPlaceAndAddRoomToQueue(new Vector2Int(vector.x + 1, vector.y)) ? 1 : 0;
                }
                if (vector.y > 1) {
                    createdCount += CheckIsEmtyPlaceAndAddRoomToQueue(new Vector2Int(vector.x, vector.y - 1)) ? 1 : 0;
                }
                if (vector.y < roomMaxBound) {
                    createdCount += CheckIsEmtyPlaceAndAddRoomToQueue(new Vector2Int(vector.x, vector.y + 1)) ? 1 : 0;
                }
                if (createdCount == 0) {
                    endRooms.Add(vector);
                    listOfPseudoRoom.Find(room => room.GetPosition() == vector).SetIsEndRoom(true);
                }
            }

            return floorplanCount;

        }

        private bool CheckIsEmtyPlaceAndAddRoomToQueue(Vector2Int vector) {
            if (floorplanCount >= dungeonValues.GetNumberOfRooms())
                return false;
            bool isEmptyPlace = Visit(vector, RoomShapeEnum.R1X1);
            if (isEmptyPlace) {
                cellQueue.Add(vector);
                // TODO attention avec la position du tableau à ne pas écraser la shape du starter (c'est le cas !!!!!!!!!!!!)
                listOfPseudoRoom.Add(new PseudoRoom(vector, RoomTypeEnum.STANDARD, enumShapes[listOfPseudoRoom.Count - 1]));
                floorplanCount += 1;
            }
            return isEmptyPlace;
        }

        private bool Visit(Vector2Int vector, RoomShapeEnum shape) {
            // bool chanceToCancel = DungeonValueGeneration.TossUp(floorplan.Length, GameManager.GetSeed);
            bool chanceToCancel = false; // TODO trouver un moyen de faire du une chance sur deux mais maitrisée
            if (chanceToCancel && vector != vectorStart || NeighbourCount(vector, shape) > 1) {
                return false;
            }

            // toDO rajouter le sens T L R B du visite afin de prendre les bonnes distances ex : si 2x2 et sens = L alors on enlève -1 en X 
            // si 2x2 && sens = T alors on ajoute +1 en y
            if (CheckEmptySpace(vector, shape) > 0) {
                return false;
            }

            switch (shape) {
                case RoomShapeEnum.R1X1: {
                    floorplan[vector.x, vector.y] = 1;
                    break;
                }
                case RoomShapeEnum.R2X2: {
                    floorplan[vector.x, vector.y] = 1;
                    floorplan[vector.x + 1, vector.y] = 1;
                    floorplan[vector.x, vector.y + 1] = 1;
                    floorplan[vector.x + 1, vector.y + 1] = 1;
                    current_R2X2++;
                    break;
                }
                case RoomShapeEnum.R1X2: {
                    floorplan[vector.x, vector.y] = 1;
                    floorplan[vector.x, vector.y + 1] = 1;
                    current_R1X2++;
                    break;
                }
                case RoomShapeEnum.R2X1: {
                    floorplan[vector.x, vector.y] = 1;
                    floorplan[vector.x + 1, vector.y] = 1;
                    current_R2X1++;
                    break;
                }
            }

            return true;

        }

        private int NeighbourCount(Vector2Int vector, RoomShapeEnum shape) {

            int count = 0;
            Vector2Int[] shapesToCheck = WorldUtils.GetNeighborsByShapes(shape, vector, bound);
            /* case
            * if x or y == 1 
            * || x or y == bound -1 
            * if search to check neighbor of big room like 2x2 so -1 + -2 is out of bound!
            */
            if (shapesToCheck.Length == 0) {
                return 99;
            }
            foreach (var checkNewPlace in shapesToCheck) {
                count += floorplan[checkNewPlace.x, checkNewPlace.y];
            }
            return count;
        }

        private int CheckEmptySpace(Vector2Int vector, RoomShapeEnum shape) {
            switch (shape) {
                case RoomShapeEnum.R1X1: {
                    return floorplan[vector.x, vector.y];
                }
                case RoomShapeEnum.R2X2: {
                    return floorplan[vector.x, vector.y] +
                        floorplan[vector.x + 1, vector.y] +
                        floorplan[vector.x, vector.y + 1] +
                        floorplan[vector.x + 1, vector.y + 1];
                }
                case RoomShapeEnum.R1X2: {
                    return floorplan[vector.x, vector.y] +
                        floorplan[vector.x, vector.y + 1];
                }
                case RoomShapeEnum.R2X1: {
                    return floorplan[vector.x, vector.y] +
                        floorplan[vector.x + 1, vector.y];
                }
            }
            return 0;
        }

    }
}