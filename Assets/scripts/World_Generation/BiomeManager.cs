using UnityEngine;
using System.Collections.Generic;

public class BiomeManager : MonoBehaviour, IBiomeManager {

    [System.Serializable]
    public class BiomeEntry {
        public BiomeEnum biome;
        public Biome biomeScriptable;
    }

    public List<BiomeEntry> biomeEntries;

    private Dictionary<BiomeEnum, Biome> biomes = new Dictionary<BiomeEnum, Biome>();

    private void Awake() {
        foreach (BiomeEntry entry in biomeEntries) {
            biomes.Add(entry.biome, entry.biomeScriptable);
        }
    }

    public Biome GetBiomeConfiguration(BiomeEnum biome) {
        return biomes[biome];
    }

}