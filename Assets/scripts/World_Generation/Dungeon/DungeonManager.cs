using System;
using DungeonNs;
using UnityEngine;

public class DungeonManager : MonoBehaviour {

    [SerializeField] private Generator generator;
    [SerializeField] private GameObject floorContainerGO;
    [SerializeField] private PoolManager poolManager;

    private IDungeonSeedGenerator dungeonSeedGenerator;
    private IDungeonFloorValues dungeonFloorValues;
    private IRoomManager roomManager;
    private IDungeonUtils dungeonUtils;
    // private IDoorManager doorManager;
    private IFloorPlanManager floorPlanManager;
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
        roomManager = new RoomManager(dungeonFloorValues, RoomFactory.GetInstance(), floorPlanManager, dungeonUtils);
        // doorManager = new DoorManager(DoorFactory.GetInstance());
    }

    private bool VerifySerialisableFieldInitialised() {
        if (generator == null) {
            throw new Exception("Generator is not assigned in editor !");
        }

        if (floorContainerGO == null) {
            throw new Exception("FloorContainerGO is not assigned in editor!");
        }

        if (poolManager == null) {
            throw new Exception("PoolManager is not assigned in editor!");
        }
        
        return generator != null && floorContainerGO != null && poolManager != null;
    }

    private void CreateSeed() {
        seed = dungeonSeedGenerator.GetNewSeed(seedLengh);
        if (string.IsNullOrEmpty(seed)) {
            throw new Exception("Error when generating a new seed !");
        }
    }

    public void Setup(IDungeonFloorConfig floorConfig) {
        dungeonFloorValues.InitValues(floorConfig, seed, dungeonSeedGenerator, floorPlanManager.GetFloorPlanBound());
        poolManager.GetComponent<PoolManager>().Setup();
        generator.GenerateDungeon(floorConfig, floorContainerGO, dungeonFloorValues, dungeonUtils, roomManager, floorPlanManager, poolManager);
    }

    public string GetSeed() {
        return seed;
    }

    public void InitPooling() {

    }
}

