﻿using System.Collections.Generic;
using UnityEngine;
using RoomNs;
using DoorNs;
using System.Collections;

namespace DungeonNs {

    public class Generator : MonoBehaviour {

        private Vector2Int roomSize = new Vector2Int(61, 31);
        private int current_R2X2, current_R1X2, current_R2X1 = 0;
        // private float luckForSpecialSHape = 0.5f;

        // InitValues
        private int[,] floorplan;
        private int bound;
        private int roomMaxBound;
        private int totalLoop = 0;
        private int floorplanCount;
        private string seed;
        private Vector2Int vectorStart;
        private List<Vector2Int> cellQueue;
        private List<Vector2Int> endRooms;
        private List<PseudoRoom> listOfPseudoRoom;
        private Config currentDungeonConfig;
        private Dictionary<BiomeEnum, Dictionary<DifficultyEnum, Dictionary<RoomTypeEnum, Dictionary<RoomShapeEnum, List<string>>>>> roomDico = RoomsJsonConfig.GetRoomDictionary();
        private TextAsset jsonFile;
        private Pool<Room> roomPool;
        private GameObject floorGO;
        private Dictionary<DifficultyEnum, float> roomRepartition = new Dictionary<DifficultyEnum, float>();
        DungeonValues dungeonValues;

        public string SeedGenerator(int length) {
            const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string seed = "";
            for (int i = 0; i < length; i++) {
                int randomIndex = UnityEngine.Random.Range(0, characters.Length);
                seed += characters[randomIndex];
            }
            Debug.Log("Seed générée : " + seed);
            return seed;
        }

        public void StartGeneration(GameObject floorGO, Config config) {
            InitValues(floorGO, config);
            // CreatePool();
            GenerateRooms();
            SetSpecialRooms();
            ManageRoomsDoors();
            CreateRooms();
            // SetRoomNeighborsDoors();
        }
        private void InitValues(GameObject floorGO, Config config) {
            currentDungeonConfig = config;
            seed = SeedGenerator(8);
            dungeonValues = DungeonValueGeneration.CreateRandomValues(seed, config.GetCurrentFloorNumber());
            RoomRepartition.SetRoomRepartition(config.GetDifficulty(), dungeonValues.GetNumberOfRooms(), roomRepartition);
            floorplan = new int[12, 12];
            bound = floorplan.GetLength(0);
            roomMaxBound = floorplan.GetLength(0) - 2;
            vectorStart = new Vector2Int(bound / 2, bound / 2);
            totalLoop = 0;
            this.floorGO = floorGO;
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

        private void CreateRooms() {

            foreach (KeyValuePair<DifficultyEnum, float> values in roomRepartition) {

                DifficultyEnum diff = values.Key;

                for(var i = 0; i < values.Value; i++) {

                    PseudoRoom room = null;

                    if (listOfPseudoRoom.Count > 0) {
                        // get last element
                        room = listOfPseudoRoom[listOfPseudoRoom.Count - 1];
                        listOfPseudoRoom.RemoveAt(listOfPseudoRoom.Count - 1);
                    } else {
                        Debug.LogError("CreateRooms : error the number of parts to be added is greater than the number of parts distributed");
                        return;
                    }

                    GameObject roomGo = null;

                    if (room.GetIsStartRoom()) {
                        List<string> rooms = roomDico[currentDungeonConfig.GetBiomeType()][diff][RoomTypeEnum.STARTER][room.GetShape()];
                        if (rooms.Count == 0) {
                            Debug.LogError("CreateRooms : no room available for this configuration : " + currentDungeonConfig.GetBiomeType() + "/" + diff + "/" + RoomTypeEnum.STANDARD + "/" + room.GetShape());
                            return;
                        }
                        int rnd = Random.Range(0, rooms.Count - 1);
                        roomGo = Resources.Load<GameObject>(GlobalConfig.prefabRoomsVariantsPath + rooms[rnd]);
                        if(roomGo == null) {
                            Debug.LogError("CreateRooms : no starter found for : " + roomDico[currentDungeonConfig.GetBiomeType()][diff][RoomTypeEnum.STARTER][room.GetShape()]);
                            return;
                        }
                    } else {
                        List<string> rooms = roomDico[currentDungeonConfig.GetBiomeType()][diff][RoomTypeEnum.STANDARD][room.GetShape()];
                        if (rooms.Count == 0) {
                            Debug.LogError("CreateRooms : no room available for this configuration : " + currentDungeonConfig.GetBiomeType() + "/" + diff + "/" + RoomTypeEnum.STANDARD + "/" + room.GetShape());
                            return;
                        }
                        int rnd = Random.Range(0, rooms.Count - 1);
                        roomGo = Resources.Load<GameObject>(GlobalConfig.prefabRoomsVariantsPath + rooms[rnd]);
                        if (roomGo == null) {
                            Debug.LogError("CreateRooms : no starter found for path : " + GlobalConfig.prefabRoomsVariantsPath + rooms[rnd]);
                            return;
                        }
                    }

                    // toDO refacto tout ça pour trouver mieux et ajouter tous les params que pourra utiliser la room

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









            // roomPool.Setup()
            foreach (KeyValuePair<DifficultyEnum, float> paire in roomRepartition) {
                DifficultyEnum diff = paire.Key;
                float valeur = paire.Value;
                // currentDungeonConfig.GetBiomeType()
                
                // roomGo = Resources.Load<GameObject>(currentDungeonConfig.GetRoomsFolderPathByBiomeAndRoomSize(room.GetShape()) + rnd);
                // Faire quelque chose avec la clé et la valeur
                Debug.Log("Clé : " + diff + ", Valeur : " + valeur);
                
            }

            /*foreach (PseudoRoom room in listOfPseudoRoom) {

                int rnd = Random.Range(0, roomJson[currentDungeonConfig.GetBiomeType()][diff][RoomTypeEnum.STANDARD][].count);

                GameObject roomGo = null;
                // create get room
                if (room.GetIsStartRoom()) {
                    // roomGo = Resources.Load<GameObject>(currentDungeonConfig.GetStarterPathRoomByBiome() + 0);
                } else {
                    // toDO refacto et manage avec un pool de piece déjà prise !!!
                    int rnd = 0;
                    //rnd = UnityEngine.Random.Range(0, currentRoomFolder.shapes.Find(s => s.name == room.GetShape()).count);
                    // TODO UTILISER LA NOUVELLE FACON  !!!!!!!!!!!!!!!!!!
                    
                    // roomGo = Resources.Load<GameObject>(currentDungeonConfig.GetRoomsFolderPathByBiomeAndRoomSize(room.GetShape()) + rnd);
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
                // room.room = obj;
                // gridOfRoom.Add(obj);
            }*/
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
            listOfPseudoRoom.Add(new PseudoRoom(vectorStart, RoomTypeEnum.STARTER));
        }

        /*public Room GetRoomFromVector2Int(Vector2Int position) {
            return gridOfRoom.Find(room => room.GetId() == rooms[position.x + maxCol / 2, position.y + maxCol / 2].id);
        }*/

        private int TryToGenerateAllRoomsFloor() {
            floorplanCount = 0;
            // Start BFS pattern
            while (cellQueue.Count > 0 && floorplanCount < dungeonValues.GetNumberOfRooms()) {
                RoomShapeEnum roomShape = GetRandomShapeRoom(bound);
                Vector2Int vector = cellQueue[0];
                cellQueue.RemoveAt(0); // get the first of list
                int createdCount = 0;

                if (vector.x > 1 && !CheckLimitOfSpecialRooms(roomShape)) {
                    createdCount += CheckIsEmtyPlaceAndAddRoomToQueue(new Vector2Int(vector.x - 1, vector.y), roomShape) ? 1 : 0;
                }
                if (vector.x < roomMaxBound && !CheckLimitOfSpecialRooms(roomShape)) {
                    createdCount += CheckIsEmtyPlaceAndAddRoomToQueue(new Vector2Int(vector.x + 1, vector.y), roomShape) ? 1 : 0;
                }
                if (vector.y > 1 && !CheckLimitOfSpecialRooms(roomShape)) {
                    createdCount += CheckIsEmtyPlaceAndAddRoomToQueue(new Vector2Int(vector.x, vector.y - 1), roomShape) ? 1 : 0;
                }
                if (vector.y < roomMaxBound && !CheckLimitOfSpecialRooms(roomShape)) {
                    createdCount += CheckIsEmtyPlaceAndAddRoomToQueue(new Vector2Int(vector.x, vector.y + 1), roomShape) ? 1 : 0;
                }
                if (createdCount == 0) {
                    endRooms.Add(vector);
                    listOfPseudoRoom.Find(room => room.GetPosition() == vector).SetIsEndRoom(true);
                }
            }
            return floorplanCount;
        }
        private bool CheckIsEmtyPlaceAndAddRoomToQueue(Vector2Int vector, RoomShapeEnum shape) {
            if (floorplanCount >= dungeonValues.GetNumberOfRooms())
                return false;
            bool isEmptyPlace = Visit(vector, shape);
            if (isEmptyPlace) {
                cellQueue.Add(vector);
                listOfPseudoRoom.Add(new PseudoRoom(vector, RoomTypeEnum.STANDARD));
                floorplanCount += 1;
            }
            return isEmptyPlace;
        }

        private bool CheckLimitOfSpecialRooms(RoomShapeEnum roomShape) {
            switch (roomShape) {
                case RoomShapeEnum.R2X2: {
                    if (current_R2X2 > DungeonConsts.max_R2X2) {
                        return true;
                    }
                    break;
                }
                case RoomShapeEnum.R1X2: {
                    if (current_R1X2 > DungeonConsts.max_R1X2) {
                        return true;
                    }
                    break;
                }
                case RoomShapeEnum.R2X1: {
                    if (current_R2X1 > DungeonConsts.max_R2X1) {
                        return true;
                    }
                    break;
                }
            }
            return false;
        }

        private bool Visit(Vector2Int vector, RoomShapeEnum shape) {

            if (UnityEngine.Random.value < 0.5f && vector != vectorStart || NeighbourCount(vector, shape) > 1) {
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

        private RoomShapeEnum GetRandomShapeRoom(int bound) {
            float rng = UnityEngine.Random.value;
            if (rng < 0.3) {
                if (UnityEngine.Random.value < 0.5f) {
                    return 0;
                }
                if (current_R2X2 < DungeonConsts.max_R2X2) {
                    return RoomShapeEnum.R2X2;
                }
            }
            if (rng > 0.3 && UnityEngine.Random.value < 0.6) {
                if (UnityEngine.Random.value < 0.5f) {
                    return 0;
                }
                if (current_R1X2 < DungeonConsts.max_R1X2) {
                    return RoomShapeEnum.R1X2;
                }
            }
            if (rng > 0.6) {
                if (UnityEngine.Random.value < 0.5f) {
                    return 0;
                }
                if (current_R2X1 < DungeonConsts.max_R2X1) {
                    return RoomShapeEnum.R2X1;
                }
            }
            return RoomShapeEnum.R1X1;
        }

    }
}