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
    private Item.IManager iTemManager;
    private PoolManager poolManager;
    private readonly int SEED_LENGH = 8;
    private string seed;

    private void Awake(){
        if (VerifySerialisableFieldInitialised()) {
            InstantiateSingletons();
            CreateSeed();
        }
    }

    private void InstantiateSingletons() {
        dungeonSeedGenerator = DungeonSeedGenerator.GetInstance();
        dungeonFloorValues = DungeonFloorValues.GetInstance();
        floorPlanManager = new FloorPlanManager();
        poolManager = poolManagerGO.GetComponent<PoolManager>();
        doorManager = DoorManager.GetInstance(poolManager.GetDoorPool());
        iTemManager = Item.Manager.GetInstance(dungeonFloorValues);
        roomManager = RoomManager.GetInstance(dungeonFloorValues, floorPlanManager);
    }

    private bool VerifySerialisableFieldInitialised() {
        if (generator == null) {
            throw new Exception("Generator is not assigned in editor !");
        }

        if (floorContainerGO == null) {
            throw new Exception("FloorContainerGO is not assigned in editor!");
        }

        if(poolManagerGO == null) {
            throw new Exception("PoolManagerGO is not assigned in editor!");
        }

        return generator != null && floorContainerGO != null && poolManagerGO != null;
    }

    private void CreateSeed() {
        seed = dungeonSeedGenerator.GetNewSeed(SEED_LENGH);
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

