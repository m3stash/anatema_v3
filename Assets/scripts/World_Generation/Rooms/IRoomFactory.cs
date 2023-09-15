using RoomNs;
using UnityEngine;

public interface IRoomFactory {
    public Room CreateRoom(RoomShapeEnum shape);
    public GameObject CreateRoomGO(DifficultyEnum diff, RoomShapeEnum shape, RoomTypeEnum type, IDungeonInitializer dungeonInitializer, BiomeEnum biome);
}


