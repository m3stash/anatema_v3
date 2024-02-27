using System.Collections.Generic;
using UnityEngine;
using RoomNs;
using DoorNs;
using Debug = UnityEngine.Debug;
#if UNITY_EDITOR
#endif

namespace DungeonNs {

    public class Generator : MonoBehaviour {

        private IDungeonFloorValues dungeonFloorValues;
        private IDungeonFloorConfig floorConfig;
        private IRoomManager roomManager;
        private GameObject floorContainer;
        private IFloorPlanManager floorPlanManager;
        private IDoorManager doorManager;
        private int totalLoop;
        private List<GameObject> roomsGo;
        private int maxLoop = 100;

        public void GenerateDungeon(
            IDungeonFloorConfig floorConfig,
            GameObject floorContainer,
            IDungeonFloorValues dungeonFloorValues,
            IRoomManager roomManager,
            IFloorPlanManager floorPlanManager,
            IDoorManager doorManager
        ) {
            this.floorConfig = floorConfig;
            this.floorContainer = floorContainer;
            this.dungeonFloorValues = dungeonFloorValues;
            this.roomManager = roomManager;
            this.floorPlanManager = floorPlanManager;
            this.doorManager = doorManager;
            StartGeneration();
        }

        private void StartGeneration() {
            InitValues();
            GenerateRooms();
            CreateRoomsGO();
        }

        private void InitValues() {
            totalLoop = 0;
            roomsGo = new List<GameObject>();
        }

        private void TryGenerateRooms() {
            if (IsRoomGenerationUnsuccessful()) {
                AttemptReGeneration();
            } else {
                Debug.Log("Total Loop for current Generation " + totalLoop);
            }
        }

        /*private void TryGenerateRooms1000Times() {
            int count = 0;
            for (var i = 0; i < 1000; i++) {
                count++;
                TryGenerateRooms();
            }
            Debug.Log("Try Succesfull for " + count + " times");

        }*/

        private bool IsRoomGenerationUnsuccessful() {
            return roomManager.GetListOfRoom().Count < dungeonFloorValues.GetNumberOfRooms();
        }

        private void AttemptReGeneration() {
            totalLoop++;
            if (totalLoop <= maxLoop) {
                floorPlanManager.ResetFloorPlan();
                GenerateRooms();
            } else {
                Debug.LogError("TRY GenerateRooms call == 100 tries");
            }
        }

        private void GenerateRooms() {
            roomManager.InitializeRooms();
            TryGenerateRooms();
        }

        private void CreateRoomsGO() {
            CreateStandardRoomsGO();
            CreateSecretRoomsGO();
        }


        private void CreateStandardRoomsGO() {
            foreach (KeyValuePair<DifficultyEnum, float> values in dungeonFloorValues.GetRoomRepartition()) {
                DifficultyEnum diff = values.Key;
                for (var i = 0; i < values.Value; i++) {

                    Room Room = roomManager.GetNextRoom();
                    if (Room == null) {
                        Debug.LogError("CreateRooms : No more rooms available");
                        return;
                    }

                    DifficultyEnum difficulty = Room.GetRoomTypeEnum == RoomTypeEnum.STANDARD ? diff : DifficultyEnum.DEFAULT;
                    GameObject roomGO = InstanciateRoomGo(Room, difficulty);
                    CreateDoorsGo(Room, roomGO);
                    roomsGo.Add(roomGO);
                }
            }

        }

        private void CreateSecretRoomsGO() {
            foreach (Room Room in roomManager.GetListOfRoom()) {
                GameObject roomGO = InstanciateRoomGo(Room, DifficultyEnum.DEFAULT);
                CreateDoorsGo(Room, roomGO);
                roomsGo.Add(roomGO);
            }
        }

        private GameObject InstanciateRoomGo(Room Room, DifficultyEnum difficulty) {
            RoomShapeEnum shape = Room.GetShape();
            Vector2Int worldPos = Room.GetWorldPosition();
            GameObject roomPrefab = roomManager.InstantiateRoomPrefab(difficulty, shape, Room.GetRoomTypeEnum, dungeonFloorValues, floorConfig.GetBiomeType());
            return roomManager.InstantiateRoomGO(roomPrefab, new Vector3(worldPos.x, worldPos.y, 0), transform, floorContainer);
        }

        private void CreateDoorsGo(Room Room, GameObject roomGo) {
            BiomeEnum biome = floorConfig.GetBiomeType();
            Room.SearchNeighborsAndCreateDoor(floorPlanManager, floorConfig.GetBiomeType());
            List<Door> doorList = Room.GetDoors();
            if (doorList.Count > 0) {
                foreach (Door door in doorList) {
                    doorManager.CreateDoor(roomGo.transform, door, Room.GetRoomTypeEnum, biome);
                }
            }
        }

    }
}