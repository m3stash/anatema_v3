using RoomNs;

public interface IDungeonFloorConfig {
    public int GetCurrentFloorNumber();
    public BiomeEnum GetBiomeType();
    public DifficultyEnum GetDifficulty();
    public RoomSizeEnum GetRoomSize();
}

