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
            listOfPseudoRoom = new List<PseudoRoom>();
        }

        private void SetFloorPlan(RoomShapeEnum shape, Vector2Int vector) {
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
                    break;
                }
                case RoomShapeEnum.R1X2: {
                    floorplan[vector.x, vector.y] = 1;
                    floorplan[vector.x, vector.y + 1] = 1;
                    break;
                }
                case RoomShapeEnum.R2X1: {
                    floorplan[vector.x, vector.y] = 1;
                    floorplan[vector.x + 1, vector.y] = 1;
                    break;
                }

            }
        }

        private bool CheckEmptySpaceByShape(Vector2Int vector, RoomShapeEnum shape) {
            if (CheckEmptySpace(vector, shape) > 0) {
                return false;
            }
            if (NeighbourCount(vector, shape) > 1) {
                return false;
            }
            return true;
        }

        private int TryToGenerateAllRoomsFloor() {
            int current_R2X2 = 0;
            int current_R1X2 = 0;
            int current_R2X1 = 0;
            // init starter room
            listOfPseudoRoom.Add(new PseudoRoom(vectorStart, RoomTypeEnum.STARTER, RoomShapeEnum.R1X1));
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            queue.Enqueue(vectorStart);
            SetFloorPlan(RoomShapeEnum.R1X1, vectorStart);

            // List<RoomShapeEnum> enumShapes = DungeonValueGeneration.GenerateRandomShapeByBiome(dungeonValues.GetNumberOfRooms(), GameManager.GetSeed, GameManager.GetCurrentDungeonConfig().GetCurrentFloorNumber());

            // Start BFS pattern
            while (queue.Count > 0 && listOfPseudoRoom.Count < dungeonValues.GetNumberOfRooms()) {

                Vector2Int roomQueue = queue.Dequeue();
                // RoomShapeEnum newRoomShape = enumShapes[0];
                RoomShapeEnum newRoomShape = RoomShapeEnum.R1X1;

                // toDO a ameliorer
                bool randoShape_R2X2 = Random.Range(0, 4) == 1 && current_R2X2 < DungeonConsts.max_R2X2;
                bool randoShape_R1X2 = Random.Range(0, 4) == 1 && current_R1X2 < DungeonConsts.max_R1X2;
                bool randoShape_R2X1 = Random.Range(0, 4) == 1 && current_R2X1 < DungeonConsts.max_R2X1;

                List<RoomShapeEnum> specialShape = new List<RoomShapeEnum>();

                if (randoShape_R2X2) {
                    current_R2X2++;
                    specialShape.Add(RoomShapeEnum.R2X2);
                }
                if (randoShape_R1X2) {
                    current_R1X2++;
                    specialShape.Add(RoomShapeEnum.R1X2);
                }
                if (randoShape_R2X1) {
                    current_R2X1++;
                    specialShape.Add(RoomShapeEnum.R2X1);
                }

                if (specialShape.Count > 0) {
                    newRoomShape = specialShape[Random.Range(0, specialShape.Count)];
                }

                switch (newRoomShape) {
                    case RoomShapeEnum.R1X1: {

                        Vector2Int[] directions = {
                            new Vector2Int(roomQueue.x - 1, roomQueue.y),
                            new Vector2Int(roomQueue.x + 1, roomQueue.y),
                            new Vector2Int(roomQueue.x, roomQueue.y + 1),
                            new Vector2Int(roomQueue.x, roomQueue.y - 1)
                        };

                        List<Vector2Int> listOfEmptySpaces = new List<Vector2Int>();

                        for (int i = 0; i < directions.Length; i++) {
                            if (CheckEmptySpaceByShape(directions[i], newRoomShape)) {
                                listOfEmptySpaces.Add(directions[i]);
                            }
                        }

                        if (listOfEmptySpaces.Count > 0) {
                            for (int i = 0; i < listOfEmptySpaces.Count; i++) {
                                int random = Random.Range(0, 2);
                                if (listOfEmptySpaces.Count > 1 && random == 1)
                                    continue;
                                queue.Enqueue(listOfEmptySpaces[i]);
                                listOfPseudoRoom.Add(new PseudoRoom(listOfEmptySpaces[i], RoomTypeEnum.STANDARD, newRoomShape));
                                SetFloorPlan(newRoomShape, listOfEmptySpaces[i]);
                            }
                        }

                        break;
                    }
                    case RoomShapeEnum.R2X2: {

                        Vector2Int[] directions = {
                            new Vector2Int(roomQueue.x - 2, roomQueue.y),
                            new Vector2Int(roomQueue.x + 1, roomQueue.y),
                            new Vector2Int(roomQueue.x, roomQueue.y + 1),
                            new Vector2Int(roomQueue.x, roomQueue.y - 2)
                        };

                        List<Vector2Int> listOfEmptySpaces = new List<Vector2Int>();

                        for (int i = 0; i < directions.Length; i++) {
                            if (CheckEmptySpaceByShape(directions[i], newRoomShape)) {
                                listOfEmptySpaces.Add(directions[i]);
                            }
                        }

                        if (listOfEmptySpaces.Count > 0) {
                            int randomPlace = Random.Range(0, listOfEmptySpaces.Count);
                            queue.Enqueue(listOfEmptySpaces[randomPlace]);
                            listOfPseudoRoom.Add(new PseudoRoom(listOfEmptySpaces[randomPlace], RoomTypeEnum.STANDARD, newRoomShape));
                            SetFloorPlan(newRoomShape, listOfEmptySpaces[randomPlace]);
                            current_R2X2++;
                        }

                        break;
                    }
                    case RoomShapeEnum.R1X2: {

                        Vector2Int[] directions = {
                            new Vector2Int(roomQueue.x - 1, roomQueue.y),
                            new Vector2Int(roomQueue.x + 1, roomQueue.y),
                            new Vector2Int(roomQueue.x, roomQueue.y + 1),
                            new Vector2Int(roomQueue.x, roomQueue.y - 2)
                        };

                        List<Vector2Int> listOfEmptySpaces = new List<Vector2Int>();

                        for (int i = 0; i < directions.Length; i++) {
                            if (CheckEmptySpaceByShape(directions[i], newRoomShape)) {
                                listOfEmptySpaces.Add(directions[i]);
                            }
                        }

                        if (listOfEmptySpaces.Count > 0) {
                            int randomPlace = Random.Range(0, listOfEmptySpaces.Count);
                            queue.Enqueue(listOfEmptySpaces[randomPlace]);
                            listOfPseudoRoom.Add(new PseudoRoom(listOfEmptySpaces[randomPlace], RoomTypeEnum.STANDARD, newRoomShape));
                            SetFloorPlan(newRoomShape, listOfEmptySpaces[randomPlace]);
                            current_R1X2++;
                        }

                        break;
                    }
                    case RoomShapeEnum.R2X1: {

                        Vector2Int[] directions = {
                            new Vector2Int(roomQueue.x - 2, roomQueue.y),
                            new Vector2Int(roomQueue.x + 1, roomQueue.y),
                            new Vector2Int(roomQueue.x, roomQueue.y + 1),
                            new Vector2Int(roomQueue.x, roomQueue.y - 1)
                        };

                        List<Vector2Int> listOfEmptySpaces = new List<Vector2Int>();

                        for (int i = 0; i < directions.Length; i++) {
                            if (CheckEmptySpaceByShape(directions[i], newRoomShape)) {
                                listOfEmptySpaces.Add(directions[i]);
                            }
                        }

                        if (listOfEmptySpaces.Count > 0) {
                            int randomPlace = Random.Range(0, listOfEmptySpaces.Count);
                            queue.Enqueue(listOfEmptySpaces[randomPlace]);
                            listOfPseudoRoom.Add(new PseudoRoom(listOfEmptySpaces[randomPlace], RoomTypeEnum.STANDARD, newRoomShape));
                            SetFloorPlan(newRoomShape, listOfEmptySpaces[randomPlace]);
                            current_R2X1++;
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

        private int NeighbourCount(Vector2Int vector, RoomShapeEnum shape) {

            int count = 0;
            Vector2Int[] shapesToCheck = WorldUtils.GetNeighborsByShapes(shape, vector);
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

        private int CheckEmptySpace(Vector2Int vector, RoomShapeEnum shape) {
            switch (shape) {
                case RoomShapeEnum.R1X1: {
                    if (CheckIsOutOfBound(vector, floorplanBound))
                        return 1;
                    return floorplan[vector.x, vector.y];
                }
                case RoomShapeEnum.R2X2: {
                    if (CheckIsOutOfBound(new Vector2Int(vector.x, vector.y), floorplanBound)
                        || CheckIsOutOfBound(new Vector2Int(vector.x + 1, vector.y), floorplanBound)
                        || CheckIsOutOfBound(new Vector2Int(vector.x, vector.y + 1), floorplanBound)
                        || CheckIsOutOfBound(new Vector2Int(vector.x + 1, vector.y + 1), floorplanBound))
                        return 1;
                    return floorplan[vector.x, vector.y] +
                        floorplan[vector.x + 1, vector.y] +
                        floorplan[vector.x, vector.y + 1] +
                        floorplan[vector.x + 1, vector.y + 1];
                }
                case RoomShapeEnum.R1X2: {
                    if (CheckIsOutOfBound(new Vector2Int(vector.x, vector.y), floorplanBound) 
                        || CheckIsOutOfBound(new Vector2Int(vector.x, vector.y + 1), floorplanBound))
                        return 1;
                    return floorplan[vector.x, vector.y] +
                        floorplan[vector.x, vector.y + 1];
                }
                case RoomShapeEnum.R2X1: {
                    if (CheckIsOutOfBound(new Vector2Int(vector.x, vector.y), floorplanBound)
                        || CheckIsOutOfBound(new Vector2Int(vector.x + 1, vector.y), floorplanBound))
                        return 1;
                    return floorplan[vector.x, vector.y] +
                        floorplan[vector.x + 1, vector.y];
                }
            }
            return 0;
        }

    }
}