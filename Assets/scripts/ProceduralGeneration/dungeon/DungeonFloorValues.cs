using System.Collections.Generic;
using UnityEngine;
using RoomNs;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace DungeonNs {

    public class DungeonFloorValues : IDungeonFloorValues {

        private static DungeonFloorValues instance;

        public static DungeonFloorValues GetInstance() {
            instance ??= new DungeonFloorValues();
            return instance;
        }

        private Vector2Int vectorStart;
        private RoomConfigDictionary roomDico = RoomsJsonConfig.LoadRoomDictionary();
        private Dictionary<DifficultyEnum, float> roomRepartition = new Dictionary<DifficultyEnum, float>();
        private System.Random randomFromSeedHash;
        private int numberRoomForStage;

        public void InitValues(IDungeonFloorConfig floorConfig, string seed, IDungeonSeedGenerator dungeonSeedGenerator, int bound) {
            vectorStart = new Vector2Int((bound / 2) - 1, (bound / 2) - 1);
            numberRoomForStage = dungeonSeedGenerator.GenerateNumberRoomPerFloor(seed, floorConfig.GetCurrentFloorNumber());
            roomRepartition = SetRoomRepartition(floorConfig.GetDifficulty(), numberRoomForStage, roomRepartition);
            int seedHash = dungeonSeedGenerator.GetSeedHash(seed);
            randomFromSeedHash = new System.Random(seedHash);
        }

        private Dictionary<DifficultyEnum, float> SetRoomRepartition(DifficultyEnum difficulty, int sizeFloor, Dictionary<DifficultyEnum, float> roomRepartition) {
            switch (difficulty) {
                case (DifficultyEnum.EASY):
                    /*
                     * 70% Easy
                     * 20% Normal
                     * 10% Hard
                     */
                    roomRepartition[DifficultyEnum.EASY] = Mathf.Round(sizeFloor * 0.7f);
                    roomRepartition[DifficultyEnum.NORMAL] = Mathf.Round(sizeFloor * 0.3f);
                    roomRepartition[DifficultyEnum.HARD] = 0;
                    break;
                case (DifficultyEnum.NORMAL):
                    /*
                    * 20% Easy
                    * 50% Normal
                    * 30% Hard
                    */
                    roomRepartition[DifficultyEnum.EASY] = Mathf.Round(sizeFloor * 0.2f);
                    roomRepartition[DifficultyEnum.NORMAL] = Mathf.Round(sizeFloor * 0.5f);
                    roomRepartition[DifficultyEnum.HARD] = Mathf.Round(sizeFloor * 0.3f);
                    break;
                case (DifficultyEnum.HARD):
                    /*
                    * 10% Easy
                    * 30% Normal
                    * 60% Hard
                    */
                    roomRepartition[DifficultyEnum.EASY] = Mathf.Round(sizeFloor * 0.1f);
                    roomRepartition[DifficultyEnum.NORMAL] = Mathf.Round(sizeFloor * 0.3f);
                    roomRepartition[DifficultyEnum.HARD] = Mathf.Round(sizeFloor * 0.6f);
                    break;
            }

            return roomRepartition;
            /*Debug.Log("TOTAL ROOM FLOOR - " + (int)(roomRepartition[DifficultyEnum.Easy] + roomRepartition[DifficultyEnum.Normal] + roomRepartition[DifficultyEnum.Hard]));
            Debug.Log("EASY ROOM - " + roomRepartition[DifficultyEnum.Easy]);
            Debug.Log("NORML ROOM - " + roomRepartition[DifficultyEnum.Normal]);
            Debug.Log("HARD ROOM - " + roomRepartition[DifficultyEnum.Hard]);*/
        }

        public Vector2Int GetVectorStart() {
            return vectorStart;
        }

        public RoomConfigDictionary GetRoomConfigDictionary() {
            return roomDico;
        }

        public Dictionary<DifficultyEnum, float> GetRoomRepartition() {
            return roomRepartition;
        }

        public System.Random GetRandomFromSeedHash() {
            return randomFromSeedHash;
        }

        public int GetNextRandomValue(int value) {
            return randomFromSeedHash.Next(value);
        }

        public int GetNumberOfRooms() {
            return numberRoomForStage;
        }
    }
}
