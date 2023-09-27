using System;
using UnityEngine;

public class DoorFactory: IDoorFactory {

    private static IDoorFactory instance;

    public static IDoorFactory GetInstance() {
        instance ??= new DoorFactory();
        return instance;
    }

    public GameObject InstantiateDoorPrefab(string path) {
        try {
            return Resources.Load<GameObject>(path);
        } catch (ArgumentNullException ex) {
            Debug.LogError("Error loading door prefab: " + ex.Message);
            return null;
        }
    }

    public GameObject InstantiateDoorGO(GameObject doorPrefab, Vector3 vector3, Transform transform, Transform parentTransform) {
        try {
            return UnityEngine.Object.Instantiate(doorPrefab, vector3, transform.rotation, parentTransform);
        } catch (ArgumentNullException ex) {
            Debug.LogError("Error instantiating Door game object: " + ex.Message);
            return null;
        }
    }

}
