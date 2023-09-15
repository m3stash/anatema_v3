using System;
using System.Collections.Generic;
using RoomNs;
using UnityEngine;

public class RoomFactory : IRoomFactory {
    public Room CreateRoom(RoomShapeEnum shape) {
        Type classType = Type.GetType("Room_" + shape.ToString());
        if (classType != null && typeof(Room).IsAssignableFrom(classType)) {
            return (Room)Activator.CreateInstance(classType);
        } else {
            throw new TypeLoadException("Shape does not exist");
        }
    }

    public GameObject CreateRoomGO(DifficultyEnum diff, RoomShapeEnum shape, RoomTypeEnum type, IDungeonInitializer dungeonInitializer, BiomeEnum biome) {
        try {
            List<string> rooms = dungeonInitializer.GetRoomConfigDictionary()[biome][diff][type][shape];
            if (rooms.Count == 0) {
                throw new ArgumentNullException("CreateRooms : no room available for this configuration : " + biome + "/" + diff + "/" + type + "/" + shape);
            }
            int rnd = dungeonInitializer.GetNextRandomValue(rooms.Count);
            return Resources.Load<GameObject>(GlobalConfig.Instance.PrefabRoomsVariantsPath + rooms[rnd]);
        } catch (ArgumentNullException ex) {
            Debug.LogError("Error loading room prefab: " + ex.Message);
            return null;
        }
    }
}