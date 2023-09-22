using RoomNs;

namespace DungeonNs {
    public class DungeonFloorConfig: IDungeonFloorConfig {

        private BiomeEnum biomeType;
        private DifficultyEnum difficulty;
        private RoomSizeEnum roomSize;
        private int currentFloor;

        public DungeonFloorConfig(BiomeEnum biomeType, DifficultyEnum difficulty, RoomSizeEnum roomSize, int currentFloor) {
            this.biomeType = biomeType;
            this.difficulty = difficulty;
            this.roomSize = roomSize;
            this.currentFloor = currentFloor;
        }

        public int GetCurrentFloorNumber() {
            return currentFloor;
        }

        public BiomeEnum GetBiomeType() {
            return biomeType;
        }

        public DifficultyEnum GetDifficulty() {
            return difficulty;
        }

        public RoomSizeEnum GetRoomSize() {
            return roomSize;
        }

    }
}