using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon : MonoBehaviour {
    [SerializeField] private DungeonConfig config;

    public void Setup(DungeonConfig config) {
        this.config = config;
        Instantiate(Resources.Load<GameObject>("Prefabs/Backgrounds/ParallaxContainers/" + GetBackgroundByBiome()));
    }

    private string GetBackgroundByBiome() {
        switch (config.GetBiomeEnum()) {
            case BiomeEnum.CAVE:
            return "BackgroundParallax Cave";
            default:
            return "BackgroundParallax Cave";
        }
    }

}
