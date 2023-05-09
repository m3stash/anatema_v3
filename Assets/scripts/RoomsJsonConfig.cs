using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using UnityEngine.Purchasing.MiniJSON;

[System.Serializable]
[ExecuteInEditMode]
public class RoomsJsonConfig {

    private static RoomsJsonConfig _instance;
    private string roomPath = "/Resources/Prefabs/Rooms/";
    private string jsonFileName = "rooms_prefab_config.json";
    public List<Biomes> biomes;

    public RoomsJsonConfig() {
    }

    private static int CountPrefbabs(BiomeEnum biome, DifficultyEnum difficulty, RoomShape shape) {
        string shapeFolder = shape.ToString().Split("ROOMSHAPE_").Last();
        string path = Application.dataPath + _instance.roomPath + biome + "/" + difficulty + "/" + shapeFolder;
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

    public static RoomsJsonConfig GetInstance() {
        if (_instance == null) {
            _instance = new RoomsJsonConfig();
            _instance.biomes = new List<Biomes>();
            Enum.GetValues(typeof(BiomeEnum)).Cast<BiomeEnum>().ToList().ForEach(delegate (BiomeEnum biome) {
                _instance.biomes.Add(new Biomes() {
                    name = biome,
                    difficulties = new List<Difficulties>()
                });
                Enum.GetValues(typeof(DifficultyEnum)).Cast<DifficultyEnum>().ToList().ForEach(delegate (DifficultyEnum difficulty) {
                    _instance.biomes.Find(b => b.name == biome).difficulties.Add(new Difficulties() {
                        name = difficulty,
                        shapes = new List<Shapes>()
                    });
                    Enum.GetValues(typeof(RoomShape)).Cast<RoomShape>().ToList().ForEach(delegate (RoomShape shape) {
                        _instance.biomes.Find(b => b.name == biome).difficulties.Find(d => d.name == difficulty).shapes.Add(new Shapes() {
                            name = shape,
                            count = CountPrefbabs(biome, difficulty, shape)
                        });
                    });
                });
            });
        }
        CreateJson();
        return _instance;
    }

    private static void CreateJson() {
        var toJson = JsonUtility.ToJson(_instance);
        // var FromJson = JsonUtility.FromJson<RoomsJsonConfig>(toJson);
        File.WriteAllText(Application.dataPath + _instance.roomPath + _instance.jsonFileName, toJson);
    }

    public static void DeleteInstance() {
        _instance = null;
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

