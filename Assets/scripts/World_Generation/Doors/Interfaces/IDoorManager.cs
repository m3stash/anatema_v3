using System;
using DoorNs;
using RoomNs;
using UnityEngine;

public interface IDoorManager {
    void Setup(DoorPool doorPool);
    void CreateDoor(Transform parent, Door door, RoomTypeEnum roomType, BiomeEnum biome);
}

