using UnityEngine;
using System;
using DoorNs;

public class DoorManager : MonoBehaviour {
    private DoorPool pool;
    PoolConfig config;

    public void Setup() {
        pool = GetComponent<DoorPool>();
        config = pool.GetConfig();
        if (pool != null) {
            try {
                pool.Setup(config.GetPrefab(), config.GetPoolSize());
            } catch (Exception ex) {
                Debug.LogError("Error initializing pool: " + ex.Message);
            }
        } else {
            Debug.LogError("Component DoorPool not serialise");
        }
    }

    public void CreateDoor(Transform parent, Door door) {
        GameObject doorGO = pool.GetOne().gameObject;
        doorGO.transform.SetParent(parent);
        doorGO.GetComponent<DoorGO>().Setup(door.LocalPosition, door.GetDirection(), null);
        doorGO.SetActive(true);
    }
}

