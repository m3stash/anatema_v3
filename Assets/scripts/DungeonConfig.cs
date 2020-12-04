using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonConfig {

    private BiomeEnum biomeType;
    private DifficultyEnum difficulty;
    private RoomSizeEnum roomSize;

    public DungeonConfig(BiomeEnum biomeType, DifficultyEnum difficulty, RoomSizeEnum roomSize) {
        this.biomeType = biomeType;
        this.difficulty = difficulty;
        this.roomSize = roomSize;
    }

    public void Init() {
        
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