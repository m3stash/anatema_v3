using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;

[System.Serializable]
[ExecuteInEditMode]
public class RoomsJsonConfig {

    private static readonly RoomsJsonConfig instance = new RoomsJsonConfig();
    public List<Biomes> biomes;

    public static RoomsJsonConfig Instance {
        get {
            return instance;
        }
    }

    private static int CountPrefbabs(BiomeEnum biome, DifficultyEnum difficulty, RoomShape shape) {
        string shapeFolder = shape.ToString().Split("ROOMSHAPE_").Last();
        string path = Application.dataPath + GlobalConfig.resourcesPath + GlobalConfig.prefabRoomsPath + biome + "/" + difficulty + "/" + shapeFolder;
        int count = 0;
        if (Directory.Exists(path)) {
            string[] prefabs = Directory.GetFiles(path);
            foreach (var item in prefabs) {
                if (!item.Contains(".meta")) {
                    count++;
                }
            }
        }
        return count;
    }

    public static void CreateJson() {
        instance.biomes = new List<Biomes>();
        Enum.GetValues(typeof(BiomeEnum)).Cast<BiomeEnum>().ToList().ForEach(delegate (BiomeEnum biome) {
            instance.biomes.Add(new Biomes() {
                name = biome,
                difficulties = new List<Difficulties>()
            });
            Enum.GetValues(typeof(DifficultyEnum)).Cast<DifficultyEnum>().ToList().ForEach(delegate (DifficultyEnum difficulty) {
                instance.biomes.Find(b => b.name == biome).difficulties.Add(new Difficulties() {
                    name = difficulty,
                    shapes = new List<Shapes>()
                });
                Enum.GetValues(typeof(RoomShape)).Cast<RoomShape>().ToList().ForEach(delegate (RoomShape shape) {
                    instance.biomes.Find(b => b.name == biome).difficulties.Find(d => d.name == difficulty).shapes.Add(new Shapes() {
                        name = shape,
                        count = CountPrefbabs(biome, difficulty, shape)
                    });
                });
            });
        });
        var toJson = JsonUtility.ToJson(instance);
        // var FromJson = JsonUtility.FromJson<RoomsJsonConfig>(toJson);
        File.WriteAllText(Application.dataPath + GlobalConfig.resourcesPath + GlobalConfig.prefabsRoomConfigJsonFile + ".json", toJson);
    }

}

[System.Serializable]
public class Biomes {
    public BiomeEnum name;
    public List<Difficulties> difficulties;
}

[System.Serializable]
public class Difficulties {
    public DifficultyEnum name;
    public List<Shapes> shapes;
}

[System.Serializable]
public class Shapes {
    public RoomShape name;
    public int count;
}

