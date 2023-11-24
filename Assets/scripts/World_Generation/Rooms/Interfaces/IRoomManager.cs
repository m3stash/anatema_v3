using System.Collections.Generic;
using DungeonNs;
using RoomNs;
using UnityEngine;

public interface IRoomManager {
    Room GenerateRoom(List<RoomShapeEnum> roomShapes, ref int currentShapeIndex);
    GameObject InstantiateRoomPrefab(DifficultyEnum diff, RoomShapeEnum shape, RoomTypeEnum type, IDungeonFloorValues dungeonFloorValues, BiomeEnum biome);
    GameObject InstantiateRoomGO(GameObject roomPrefab, Vector3 vector3, Transform transform, GameObject floorContainer);
    Room InstantiateRoomImplWithProperties(RoomShapeEnum shape, Vector2Int vector, RoomTypeEnum type);
    List<Room> GetListOfRoom();
    void InitializeRooms();
    void AddRoom(Room room);
    Room GetNextRoom();
 }

