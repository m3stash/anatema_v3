using System.Collections.Generic;
using UnityEngine;
using RoomNs;
using DoorNs;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DungeonNs {

    public class Generator : MonoBehaviour {

        private Vector2Int roomSize = new Vector2Int(61, 31);
        private int current_R2X2, current_R1X2, current_R2X1 = 0;

        // InitValues
        private int[,] floorplan;
        private int bound;
        private int roomMaxBound;
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
            bound = floorplan.GetLength(0);
            roomMaxBound = floorplan.GetLength(0) - 2;
            vectorStart = new Vector2Int(bound / 2, bound / 2);
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

                for (var i = 0; i < values.Value; i++) {

                    PseudoRoom room = null;
                    GameObject roomGo = null;

                    room = listOfPseudoRoom[0];
                    listOfPseudoRoom.RemoveAt(0);

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
            current_R2X2 = 0;
            current_R1X2 = 0;
            current_R2X1 = 0;
            listOfPseudoRoom = new List<PseudoRoom>();
        }

        private void setFloorPlan(RoomShapeEnum shape, Vector2Int vector) {
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
        }

        private bool AddNewRoomInFloor(Vector2Int vector, RoomShapeEnum newRoomShape, List<RoomShapeEnum> enumShapes) {
            if(newRoomShape == RoomShapeEnum.R2X2 && current_R2X2 < DungeonConsts.max_R2X2) return false;
            if (Visit(vector, newRoomShape)) {
                if (UnityEngine.Random.Range(0, 2) > 0) // toDO revoir la gestion de l'aléatoire !!!!! ici cas de 1 sur 2
                    return false;
                PseudoRoom newPseudoRoom = new PseudoRoom(vector, RoomTypeEnum.STANDARD, newRoomShape);
                listOfPseudoRoom.Add(newPseudoRoom);
                setFloorPlan(newRoomShape, vector);
                enumShapes.RemoveAt(0);
                return true;
            }
            return false;
        }

        private int TryToGenerateAllRoomsFloor() {
            // init starter room
            listOfPseudoRoom.Add(new PseudoRoom(vectorStart, RoomTypeEnum.STARTER, RoomShapeEnum.R1X1));
            Queue<PseudoRoom> queue = new Queue<PseudoRoom>();
            queue.Enqueue(listOfPseudoRoom[0]);
            setFloorPlan(RoomShapeEnum.R1X1, vectorStart);

            List<RoomShapeEnum> enumShapes = DungeonValueGeneration.GenerateRandomShapeByBiome(dungeonValues.GetNumberOfRooms(), GameManager.GetSeed, GameManager.GetCurrentDungeonConfig().GetCurrentFloorNumber());

            // Start BFS pattern
            while (queue.Count > 0 && listOfPseudoRoom.Count < dungeonValues.GetNumberOfRooms()) {

                PseudoRoom room = queue.Dequeue();

                RoomShapeEnum newRoomShape = enumShapes[0];
                // RoomShapeEnum newRoomShape = RoomShapeEnum.R1X1;
                Vector2Int roomPosition = room.GetPosition();

                switch (newRoomShape) {
                    case RoomShapeEnum.R1X1: {

                        switch (room.GetShape()) {
                            case RoomShapeEnum.R1X1:
                            // left
                            if (roomPosition.x > 1) {
                                Vector2Int vector = new Vector2Int(roomPosition.x - 1, roomPosition.y);
                                if(AddNewRoomInFloor(vector, newRoomShape, enumShapes)) {
                                    queue.Enqueue(new PseudoRoom(vector, RoomTypeEnum.STANDARD, RoomShapeEnum.R1X1));
                                };
                            }
                            // right
                            if (roomPosition.x < roomMaxBound) {
                                Vector2Int vector = new Vector2Int(roomPosition.x + 1, roomPosition.y);
                                if (AddNewRoomInFloor(vector, newRoomShape, enumShapes)) {
                                    queue.Enqueue(new PseudoRoom(vector, RoomTypeEnum.STANDARD, RoomShapeEnum.R1X1));
                                };
                            }
                            // top
                            if (roomPosition.y < roomMaxBound) {
                                Vector2Int vector = new Vector2Int(roomPosition.x, roomPosition.y + 1);
                                if (AddNewRoomInFloor(vector, newRoomShape, enumShapes)) {
                                    queue.Enqueue(new PseudoRoom(vector, RoomTypeEnum.STANDARD, RoomShapeEnum.R1X1));
                                };
                            }
                            // bottom
                            if (roomPosition.y > 1) {
                                Vector2Int vector = new Vector2Int(roomPosition.x, roomPosition.y - 1);
                                if (AddNewRoomInFloor(vector, newRoomShape, enumShapes)) {
                                    queue.Enqueue(new PseudoRoom(vector, RoomTypeEnum.STANDARD, RoomShapeEnum.R1X1));
                                };
                            }
                            break;
                            case RoomShapeEnum.R2X1:
                            break;
                            case RoomShapeEnum.R1X2:
                            break;
                            case RoomShapeEnum.R2X2:

                            break;
                        }
                        break;
                    }
                    case RoomShapeEnum.R2X2: {
                        switch (room.GetShape()) {
                            case RoomShapeEnum.R1X1:

                                // TODO REVOIR LA GESTION POUR N'EN POSER QU'UNE DANS CE CAS (TOUT CE QUI N'EST PAS 1X1 !!!
                                // REVOIR LA GESTION DU 2X2 rien n'est BON !!!'

                            // left
                            if (roomPosition.x > 1) {
                                if (AddNewRoomInFloor(new Vector2Int(roomPosition.x - 2, roomPosition.y), newRoomShape, enumShapes)) {
                                    current_R2X2++;
                                    queue.Enqueue(new PseudoRoom(new Vector2Int(roomPosition.x - 1, roomPosition.y), RoomTypeEnum.STANDARD, RoomShapeEnum.R1X1));
                                };
                            }
                            // right
                            if (roomPosition.x < roomMaxBound) {
                                if (AddNewRoomInFloor(new Vector2Int(roomPosition.x + 1, roomPosition.y), newRoomShape, enumShapes)) {
                                    current_R2X2++;
                                    queue.Enqueue(new PseudoRoom(new Vector2Int(roomPosition.x + 1, roomPosition.y), RoomTypeEnum.STANDARD, RoomShapeEnum.R1X1));
                                };
                            }
                            // top
                            if (roomPosition.y < roomMaxBound) {
                                if (AddNewRoomInFloor(new Vector2Int(roomPosition.x, roomPosition.y + 1), newRoomShape, enumShapes)) {
                                    current_R2X2++;
                                    queue.Enqueue(new PseudoRoom(new Vector2Int(roomPosition.x, roomPosition.y + 1), RoomTypeEnum.STANDARD, RoomShapeEnum.R1X1));
                                };
                            }
                            // bottom
                            if (roomPosition.y > 1) {
                                if (AddNewRoomInFloor(new Vector2Int(roomPosition.x, roomPosition.y - 2), newRoomShape, enumShapes)) {
                                    current_R2X2++;
                                    queue.Enqueue(new PseudoRoom(new Vector2Int(roomPosition.x, roomPosition.y - 1), RoomTypeEnum.STANDARD, RoomShapeEnum.R1X1));
                                };
                            }

                            break;
                            case RoomShapeEnum.R2X1:
                            break;
                            case RoomShapeEnum.R1X2:
                            break;
                            case RoomShapeEnum.R2X2:
                            break;
                        }
                        break;
                    }
                    case RoomShapeEnum.R1X2: {
                        /*floorplan[vector.x, vector.y] = 1;
                        floorplan[vector.x, vector.y + 1] = 1;
                        current_R1X2++;*/
                        break;
                    }
                    case RoomShapeEnum.R2X1: {
                        /*floorplan[vector.x, vector.y] = 1;
                        floorplan[vector.x + 1, vector.y] = 1;
                        current_R2X1++;*/
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

        private bool Visit(Vector2Int vector, RoomShapeEnum shape) {

            // toDO rajouter le sens T L R B du visite afin de prendre les bonnes distances ex : si 2x2 et sens = L alors on enlève -1 en X 
            // si 2x2 && sens = T alors on ajoute +1 en y
            if (CheckEmptySpace(vector, shape) > 0) {
                return false;
            }

            if (NeighbourCount(vector, shape) > 1) {
                return false;
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