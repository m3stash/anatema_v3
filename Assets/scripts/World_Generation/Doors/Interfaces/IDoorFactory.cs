using RoomNs;
using UnityEngine;

public interface IDoorFactory {
    GameObject InstantiateDoorPrefab(string path);
    GameObject InstantiateDoorGO(GameObject doorPrefab, Vector3 vector3, Transform transform, Transform parentTransform);
}

