using RoomNs;

namespace DungeonNs {
    public class Config {

        private BiomeEnum biomeType;
        private DifficultyEnum difficulty;
        private RoomSizeEnum roomSize;

        public Config(BiomeEnum biomeType, DifficultyEnum difficulty, RoomSizeEnum roomSize) {
            this.biomeType = biomeType;
            this.difficulty = difficulty;
            this.roomSize = roomSize;
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

        public string GetRoomsFolderPathByBiomeAndRoomSize(RoomShape shape) {
            return GlobalConfig.prefabRoomsPath + biomeType + "/" + getFolderByEnumShape(shape) + "/";
        }

        public string GetStarterPathRoomByBiome() {
            return GlobalConfig.prefabRoomsPath + biomeType + "/Starter/";
        }

        private string getFolderByEnumShape(RoomShape shape) {
            switch (shape) {
                case RoomShape.ROOMSHAPE_1x1:
                return "1x1";
                case RoomShape.ROOMSHAPE_1x2:
                return "1x2";
                case RoomShape.ROOMSHAPE_2x1:
                return "2x1";
                case RoomShape.ROOMSHAPE_2x2:
                return "2x2";
                default:
                return "1x1";
            }
        }
    }
}