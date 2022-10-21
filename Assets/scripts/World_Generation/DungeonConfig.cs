using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonConfig {

    private BiomeEnum biomeType;
    private DifficultyEnum difficulty;
    private RoomSizeEnum roomSize;
    private string roomPath = "Prefabs/Rooms";

    public DungeonConfig(BiomeEnum biomeType, DifficultyEnum difficulty, RoomSizeEnum roomSize) {
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

    public string GetRoomsFolderPathByBiomeDifficultyAndRoomSize(DifficultyEnum roomDifficulty, RoomShape shape) {
        return roomPath + "/" + biomeType + "/" + roomDifficulty + "/" + getFolderByEnumShape(shape) + "/";
    }

    public string GetStarterPathRoomByBiome() {
        return roomPath + "/" + biomeType + "/Starter/";
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