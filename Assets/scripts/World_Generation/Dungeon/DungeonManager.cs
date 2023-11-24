using System;
using DungeonNs;
using UnityEngine;

public class DungeonManager : MonoBehaviour {

    [SerializeField] private Generator generator;
    [SerializeField] private GameObject floorContainerGO;
    [SerializeField] private GameObject poolManagerGO;

    private IDungeonSeedGenerator dungeonSeedGenerator;
    private IDungeonFloorValues dungeonFloorValues;
    private IRoomManager roomManager;
    private IDoorManager doorManager;
    private IFloorPlanManager floorPlanManager;
    private IITemManager iTemManager;
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
        dungeonFloorValues = DungeonFloorValues.GetInstance();
        dungeonSeedGenerator = DungeonSeedGenerator.GetInstance();
        floorPlanManager = new FloorPlanManager();
        poolManager = poolManagerGO.GetComponent<PoolManager>();
        doorManager = DoorManager.GetInstance(poolManager.GetDoorPool());
        iTemManager = ItemManager.GetInstance();
        roomManager = RoomManager.GetInstance(dungeonFloorValues, floorPlanManager);
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
        generator.GenerateDungeon(floorConfig, floorContainerGO, dungeonFloorValues, roomManager, floorPlanManager, doorManager, iTemManager);
    }

    public string GetSeed() {
        return seed;
    }
}

