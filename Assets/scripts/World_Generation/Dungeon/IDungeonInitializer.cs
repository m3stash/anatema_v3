using UnityEngine;

public interface IDungeonInitializer {
    void InitValues();
    int[,] GetFloorPlan();
    int GetNumberOfRooms();
    Vector2Int GetVectorStart();
    BiomeEnum GetBiome();
    int GetNextRandomValue(int maxValue);
    System.Random GetRandomFromSeedHash();
}