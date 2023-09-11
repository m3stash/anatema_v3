using System.Collections.Generic;
using UnityEngine;
using RoomNs;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DungeonNs {

    public class DungeonInitializer : MonoBehaviour, IDungeonInitializer {

        private int[,] floorplan;
        private int floorplanBound;
        private Vector2Int vectorStart;
        private RoomConfigDictionary roomDico = RoomsJsonConfig.LoadRoomDictionary();
        private Dictionary<DifficultyEnum, float> roomRepartition = new Dictionary<DifficultyEnum, float>();
        private BiomeEnum biome;
        private System.Random randomFromSeedHash;
        private GameObject floorGO;
        private int numberRoomForStage;

        public DungeonInitializer() {}

        public void InitValues() {
            floorGO = GameManager.GetFloorContainer();
            Config config = GameManager.GetCurrentDungeonConfig();
            biome = config.GetBiomeType();
            
            floorplan = new int[12, 12];
            int bound = floorplan.GetLength(0);
            floorplanBound = bound - 1;
            vectorStart = new Vector2Int((bound / 2) - 1, (bound / 2) - 1);

            // string seed = GameManager.GetSeed;
            string seed = "DDT4GAJ9"; // A hardcoded seed for now. Adjust if necessary.
            numberRoomForStage = DungeonValueGeneration.GenerateNumberRoomPerFloor(seed, config.GetCurrentFloorNumber());
            RoomRepartition.SetRoomRepartition(config.GetDifficulty(), numberRoomForStage, roomRepartition);
            int seedHash = DungeonValueGeneration.GetSeedHash(seed);
            randomFromSeedHash = new System.Random(seedHash);
        }

        public int[,] GetFloorPlan() {
            return floorplan;
        }

        public int GetFloorPlanBound() {
            return floorplanBound;
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

        public BiomeEnum GetBiome() {
            return biome;
        }

        public System.Random GetRandomFromSeedHash() {
            return randomFromSeedHash;
        }

        public GameObject GetFloorGameObject() {
            return floorGO;
        }

        public int GetNextRandomValue(int value) {
            return randomFromSeedHash.Next(value);
        }

        public int GetNumberOfRooms() {
            return numberRoomForStage;
        }
    }
}
