using UnityEngine;
using DoorNs;

public interface IDoorManager {
    GameObject InstantiateRoomPrefab(string path);
    GameObject InstantiateDoorGO(GameObject roomPrefab, Vector3 vector3, Transform transform, Transform parentTransform);
    void SetProperties(GameObject doorGO, Door door, Biome biome);
}

