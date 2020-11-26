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

    public BiomeEnum GetBiomeEnum() {
        return biomeType;
    }

    public DifficultyEnum GetDifficultyEnum() {
        return difficulty;
    }

    public RoomSizeEnum GetRoomSizeEnum() {
        return roomSize;
    }
}