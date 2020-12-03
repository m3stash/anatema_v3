using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon : MonoBehaviour {
    [SerializeField] private DungeonConfig config;

    public void Setup(DungeonConfig config) {
        this.config = config;
    }

    // toDo faire une Enum avec tout ça !
    private string GetBackgroundByBiome() {
        switch (config.GetBiomeEnum()) {
            case BiomeEnum.CAVE:
            return "BackgroundParallax Cave";
            default:
            return "BackgroundParallax Cave";
        }
    }

    public DungeonConfig GetConfig() {
        return config;
    }

    public void InitBackgroundContainer() {
        Instantiate(Resources.Load<GameObject>("Prefabs/Backgrounds/ParallaxContainers/" + GetBackgroundByBiome()));
    }

}
