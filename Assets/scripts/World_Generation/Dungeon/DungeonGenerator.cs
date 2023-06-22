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
        private int floorplanCount;
        private Vector2Int vectorStart;
        //private List<Vector2Int> cellQueue;
        //private List<Vector2Int> endRooms;
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
            int totalRoomPlaced = TryToGenerateAllRoomsFloor();
            // if all the pieces have not been successfully placed then we start again
            if (totalRoomPlaced < dungeonValues.GetNumberOfRooms()) {
                totalLoop++;
                /*if(totalLoop > 100) {
                    Debug.Log("Boucle INFINIE !!!");
                    #if UNITY_EDITOR
                        if (EditorApplication.isPlaying) {
                            EditorApplication.isPlaying = false;
                            Application.Quit();
                        }
                    #endif
                }*/
                if (totalLoop < 500) {
                    GenerateRooms();
                } else {
                    Debug.LogError("NOMBRE ESSAI > 500");
                }
            } else {
                Debug.Log("END " + totalLoop);
                Debug.Log("max_R1X2 " + DungeonConsts.max_R1X2);
                Debug.Log("max_R2X1 " + DungeonConsts.max_R2X1);
                Debug.Log("max_R1X2 " + DungeonConsts.max_R1X2);
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
            // cellQueue = new List<Vector2Int>();
            // endRooms = new List<Vector2Int>();
            listOfPseudoRoom = new List<PseudoRoom>();
            floorplanCount = 0;
        }

        private int TryToGenerateAllRoomsFloor() {

            Visit(vectorStart, RoomShapeEnum.R1X1);
            listOfPseudoRoom.Add(new PseudoRoom(vectorStart, RoomTypeEnum.STARTER, RoomShapeEnum.R1X1));
            Queue<PseudoRoom> queue = new Queue<PseudoRoom>();
            queue.Enqueue(listOfPseudoRoom[0]);
            // floorplanCount = 1; // the first room is already create so we start at 1
            // Start BFS pattern
            while (queue.Count > 0 && listOfPseudoRoom.Count < dungeonValues.GetNumberOfRooms()) {

                PseudoRoom room = queue.Dequeue();

                // int randomIndex = UnityEngine.Random.Range(0, 3);
                RoomShapeEnum newRoomShape = RoomShapeEnum.R1X1;
                Vector2Int roomPosition = room.GetPosition();

                /*bool isEmptyLeft = Visit(new Vector2Int(vector.x - 1, vector.y), roomShape);
                bool isEmptyRight = Visit(new Vector2Int(vector.x + 1, vector.y), roomShape);
                bool isEmptyTop = Visit(new Vector2Int(vector.x, vector.y + 1), roomShape);
                bool isEmptyBottom = Visit(new Vector2Int(vector.x, vector.y - 1), roomShape);*/

                switch (newRoomShape) {
                    case RoomShapeEnum.R1X1: {
                        switch (room.GetShape()) {
                            case RoomShapeEnum.R1X1:
                            // toDO rajouter le moyen de squizz une fois sur deux une salle au hasard
                            List<Vector2Int> emptyVectors = new List<Vector2Int>();
                            if(roomPosition.x > 1) {
                                bool isEmptyLeft = Visit(new Vector2Int(roomPosition.x - 1, roomPosition.y), newRoomShape);
                                if (isEmptyLeft) {
                                    emptyVectors.Add(new Vector2Int(roomPosition.x - 1, roomPosition.y));
                                }
                            }
                            if (roomPosition.x < roomMaxBound) {
                                bool isEmptyRight = Visit(new Vector2Int(roomPosition.x + 1, roomPosition.y), newRoomShape);
                                if (isEmptyRight) {
                                    emptyVectors.Add(new Vector2Int(roomPosition.x + 1, roomPosition.y));
                                }
                            }
                            if (roomPosition.y < roomMaxBound) {
                                bool isEmptyTop = Visit(new Vector2Int(roomPosition.x, roomPosition.y + 1), newRoomShape);
                                if (isEmptyTop) {
                                    emptyVectors.Add(new Vector2Int(roomPosition.x, roomPosition.y + 1));
                                }
                            }
                            if (roomPosition.y > 1) {
                                bool isEmptyBottom = Visit(new Vector2Int(roomPosition.x, roomPosition.y - 1), newRoomShape);
                                if (isEmptyBottom) {
                                    emptyVectors.Add(new Vector2Int(roomPosition.x, roomPosition.y - 1));
                                }
                            }
                            int randomIndex = UnityEngine.Random.Range(0, emptyVectors.Count);
                            if(randomIndex > 0) {
                                Vector2Int newPlace = emptyVectors[randomIndex];
                                PseudoRoom newPseudoRoom = new PseudoRoom(newPlace, RoomTypeEnum.STANDARD, newRoomShape);
                                listOfPseudoRoom.Add(newPseudoRoom);
                                floorplan[newPlace.x, newPlace.y] = 1;
                                // queue.Enqueue(newPseudoRoom);

                                // test aléatoire queue
                                int randomQueue = UnityEngine.Random.Range(0, listOfPseudoRoom.Count);
                                queue.Enqueue(listOfPseudoRoom[randomQueue]);
                            }
                            break;
                            case RoomShapeEnum.R2X1:
                            break;
                            case RoomShapeEnum.R1X2:
                            break;
                            case RoomShapeEnum.R2X2:
                            break;
                        }
                        /*bool isEmptyLeft = Visit(new Vector2Int(vector.x - 1, vector.y), roomShape);
                        bool isEmptyRight = Visit(new Vector2Int(vector.x + 1, vector.y), roomShape);
                        bool isEmptyTop = Visit(new Vector2Int(vector.x, vector.y + 1), roomShape);
                        bool isEmptyBottom = Visit(new Vector2Int(vector.x, vector.y - 1), roomShape);*/
                        break;
                    }
                    case RoomShapeEnum.R2X2: {
                        /*Vector2Int[] vectors = new Vector2Int[3];
                        DirectionalEnum direction;
                        int randomIndex = UnityEngine.Random.Range(0, 3);
                        switch (randomIndex) {
                            case 0:
                                direction = DirectionalEnum.L;
                            break;
                            case 1:
                                direction = DirectionalEnum.R;
                            break;
                            case 2:
                                direction = DirectionalEnum.B;
                            break;
                            default:
                                direction = DirectionalEnum.T;
                            break;
                        }*/


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

                /*switch (roomShape) {
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
                }*/



                /*bool isEmptyPlace = Visit(newPlace, roomShape);
                if (isEmptyPlace) {
                    queue.Enqueue(newPlace);
                    listOfPseudoRoom.Add(new PseudoRoom(vector, RoomTypeEnum.STANDARD, roomShape));
                }*/

                /*if (createdCount == 0) {
                    endRooms.Add(vector);
                    listOfPseudoRoom.Find(room => room.GetPosition() == vector).SetIsEndRoom(true);
                }*/
            }
            return listOfPseudoRoom.Count;
            // return floorplanCount;

        }

        private bool Visit(Vector2Int vector, RoomShapeEnum shape) {

            if (NeighbourCount(vector, shape) > 1) {
                return false;
            }

            // toDO rajouter le sens T L R B du visite afin de prendre les bonnes distances ex : si 2x2 et sens = L alors on enlève -1 en X 
            // si 2x2 && sens = T alors on ajoute +1 en y
            if (CheckEmptySpace(vector, shape) > 0) {
                return false;
            }

            /*switch (shape) {
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
            }*/

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