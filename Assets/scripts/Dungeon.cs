using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon : MonoBehaviour {
    [SerializeField] private DungeonConfig config;

    public void Setup(DungeonConfig config) {
        this.config = config;
    }

    private string GetFolderNameByBiome() {
        switch (config.GetBiomeType()) {
            case BiomeEnum.CAVE:
            return "Cave";
            default:
            return "Cave";
        }
    }

    public DungeonConfig GetConfig() {
        return config;
    }

    public void InitBackgroundContainer() {
        Instantiate(Resources.Load<GameObject>("Prefabs/Backgrounds/Background_3/BackgroundParallax_" + GetFolderNameByBiome()));
    }

}
