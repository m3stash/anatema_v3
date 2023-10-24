using System;
using DoorNs;
using UnityEngine;

public interface IDoorManager {
    void Setup(DoorPool doorPool);
    void CreateDoor(Transform parent, Door door);
}

