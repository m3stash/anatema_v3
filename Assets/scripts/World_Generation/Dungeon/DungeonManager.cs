using System;
using DungeonNs;
using UnityEngine;

public class DungeonManager : MonoBehaviour {

    [SerializeField] private Generator generator;
    [SerializeField] private GameObject floorContainerGO;
    [SerializeField] private BiomeManager biomeManagerReference;

    private IDungeonSeedGenerator dungeonSeedGenerator;
    private IDungeonFloorValues dungeonFloorValues;
    private IRoomManager roomManager;
    private IDungeonUtils dungeonUtils;
    private IDoorManager doorManager;
    private IBiomeManager biomeManager;
    private readonly int seedLengh = 8;
    private string seed;
    private bool isSingletonInstantiate = false;

    private void Awake(){
        biomeManager = biomeManagerReference;
        if (VerifySerialisableFieldInitialised()) {
            isSingletonInstantiate = true;
            InstantiateSingletons();
            CreateSeed();
        }
    }

    private void InstantiateSingletons() {
        dungeonFloorValues = DungeonFloorValues.GetInstance();
        dungeonSeedGenerator = DungeonSeedGenerator.GetInstance();
        dungeonUtils = DungeonUtils.GetInstance();
        roomManager = new RoomManager(dungeonFloorValues, RoomFactory.GetInstance());
        doorManager = new DoorManager(DoorFactory.GetInstance());
    }

    private bool VerifySerialisableFieldInitialised() {
        if (biomeManager == null) {
            throw new Exception("BiomeManager is not assigned in editor !");
        }

        if (generator == null) {
            throw new Exception("Generator is not assigned in editor !");
        }

        if (floorContainerGO == null) {
            throw new Exception("FloorContainerGO is not assigned in editor!");
        }
        return biomeManager != null && generator != null && floorContainerGO != null;
    }

    private void CreateSeed() {
        seed = dungeonSeedGenerator.GetNewSeed(seedLengh);
        if (string.IsNullOrEmpty(seed)) {
            throw new Exception("Error when generating a new seed !");
        }
    }

    public void Setup(IDungeonFloorConfig floorConfig) {
        if (isSingletonInstantiate) {
            dungeonFloorValues.InitValues(floorConfig, seed, dungeonSeedGenerator);
            generator.GenerateDungeon(floorConfig, floorContainerGO, biomeManager, dungeonFloorValues, dungeonUtils, roomManager, doorManager);
        }
    }

    public string GetSeed() {
        return seed;
    }
}

