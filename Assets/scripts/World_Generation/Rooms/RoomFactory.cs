using System;
using System.Collections.Generic;
using RoomNs;
using UnityEngine;

public class RoomFactory: IRoomFactory {

    private static RoomFactory instance;

    public static IRoomFactory GetInstance() {
        instance ??= new RoomFactory();
        return instance;
    }

    public Room InstantiateRoomImpl(RoomShapeEnum shape) {
        Type classType = Type.GetType("Room_" + shape.ToString());
        if (classType != null && typeof(Room).IsAssignableFrom(classType)) {
            return (Room)Activator.CreateInstance(classType);
        } else {
            throw new TypeLoadException("Shape does not exist");
        }
    }

    public GameObject InstantiateRoomPrefab(DifficultyEnum diff, RoomShapeEnum shape, RoomTypeEnum type, IDungeonFloorValues dungeonFloorValues, BiomeEnum biome) {
        try {
            List<string> rooms = dungeonFloorValues.GetRoomConfigDictionary()[biome][type][shape];
            if (rooms.Count == 0) {
                throw new ArgumentNullException("CreateRooms : no room available for this configuration : " + biome + "/" + diff + "/" + type + "/" + shape);
            }
            int rnd = dungeonFloorValues.GetNextRandomValue(rooms.Count);
            return Resources.Load<GameObject>(GlobalConfig.Instance.PrefabRoomsVariantsPath + rooms[rnd]);
        } catch (ArgumentNullException ex) {
            Debug.LogError("Error loading room prefab: " + ex.Message);
            return null;
        }
    }

    public GameObject InstantiateRoomGO(GameObject roomPrefab, Vector3 vector3, Transform transform, GameObject floorContainer) {
        try {
            return UnityEngine.Object.Instantiate(roomPrefab, vector3, transform.rotation, floorContainer.transform);
        } catch (ArgumentNullException ex) {
            Debug.LogError("Error instantiating room game object: " + ex.Message);
            return null;
        }
    }
}