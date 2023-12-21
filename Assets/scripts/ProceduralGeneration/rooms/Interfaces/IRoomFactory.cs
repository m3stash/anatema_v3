using RoomNs;
using UnityEngine;

public interface IRoomFactory {
    Room InstantiateRoomImpl(RoomShapeEnum shape);
    GameObject InstantiateRoomGO(GameObject roomPrefab, Vector3 vector3, Transform transform, GameObject floorContainer);
    GameObject InstantiateRoomPrefab(DifficultyEnum diff, RoomShapeEnum shape, RoomTypeEnum type, IDungeonFloorValues floorConfig, BiomeEnum biome);
}