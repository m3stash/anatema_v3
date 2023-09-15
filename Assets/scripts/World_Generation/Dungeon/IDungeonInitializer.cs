using System.Collections.Generic;
using DungeonNs;
using RoomNs;
using UnityEngine;

public interface IDungeonInitializer {
    void InitValues(CurrentFloorConfig floorConfig, string seed, IDungeonSeedGenerator dungeonSeedGenerator);
    int[,] GetFloorPlan();
    int GetFloorPlanBound();
    int GetNumberOfRooms();
    Vector2Int GetVectorStart();
    int GetNextRandomValue(int maxValue);
    RoomConfigDictionary GetRoomConfigDictionary();
    System.Random GetRandomFromSeedHash();
    Dictionary<DifficultyEnum, float> GetRoomRepartition();
}