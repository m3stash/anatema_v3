﻿using UnityEngine;
using System;
using DoorNs;
using RoomNs;

public class DoorManager : IDoorManager {
    private DoorPool pool;

    public void Setup(DoorPool doorPool) {
        pool = doorPool;
        PoolConfig config = doorPool.GetConfig();
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

    public void CreateDoor(Transform parent, Door door, RoomTypeEnum roomType, BiomeEnum biome) {
        GameObject doorGO = pool.GetOne().gameObject;
        doorGO.transform.SetParent(parent);
        doorGO.GetComponent<DoorGO>().Setup(door.LocalPosition, door.GetDirection(), roomType, biome);
        doorGO.SetActive(true);
    }
}

