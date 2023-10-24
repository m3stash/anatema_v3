using System;
using DungeonNs;
using UnityEditor.EditorTools;
using UnityEngine;

public class DungeonManager : MonoBehaviour {

    [SerializeField] private Generator generator;
    [SerializeField] private GameObject floorContainerGO;
    [SerializeField] private GameObject poolManagerGO;

    private IDungeonSeedGenerator dungeonSeedGenerator;
    private IDungeonFloorValues dungeonFloorValues;
    private IRoomManager roomManager;
    private IDungeonUtils dungeonUtils;
    private IDoorManager doorManager;
    private IFloorPlanManager floorPlanManager;
    private PoolManager poolManager;
    private readonly int seedLengh = 8;
    private string seed;

    private void Awake(){
        if (VerifySerialisableFieldInitialised()) {
            InstantiateSingletons();
            CreateSeed();
        }
    }

    private void InstantiateSingletons() {
        //toDo garder les singletons car ils devraient être supprimer entre deux scènes non dungeon type ???
        dungeonFloorValues = DungeonFloorValues.GetInstance();
        dungeonSeedGenerator = DungeonSeedGenerator.GetInstance();
        dungeonUtils = DungeonUtils.GetInstance();
        floorPlanManager = new FloorPlanManager();
        poolManager = poolManagerGO.GetComponent<PoolManager>();
        doorManager = new DoorManager();
        doorManager.Setup(poolManager.GetDoorPool());
        roomManager = new RoomManager(dungeonFloorValues, RoomFactory.GetInstance(), floorPlanManager, dungeonUtils);
    }

    private bool VerifySerialisableFieldInitialised() {
        if (generator == null) {
            throw new Exception("Generator is not assigned in editor !");
        }

        if (floorContainerGO == null) {
            throw new Exception("FloorContainerGO is not assigned in editor!");
        }

        return generator != null && floorContainerGO != null;
    }

    private void CreateSeed() {
        seed = dungeonSeedGenerator.GetNewSeed(seedLengh);
        if (string.IsNullOrEmpty(seed)) {
            throw new Exception("Error when generating a new seed !");
        }
    }

    public void Setup(IDungeonFloorConfig floorConfig) {
        dungeonFloorValues.InitValues(floorConfig, seed, dungeonSeedGenerator, floorPlanManager.GetFloorPlanBound());
        generator.GenerateDungeon(floorConfig, floorContainerGO, dungeonFloorValues, dungeonUtils, roomManager, floorPlanManager, doorManager);
    }

    public string GetSeed() {
        return seed;
    }

    public void InitPooling() {

    }
}

