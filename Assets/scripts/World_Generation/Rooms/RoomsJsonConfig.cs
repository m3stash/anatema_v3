using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace RoomNs {
    [Serializable]
    [ExecuteInEditMode]
    public class RoomsJsonConfig {
        private static readonly RoomsJsonConfig instance = new RoomsJsonConfig();
        private static RoomConfigDictionary biomeDico;

        public static RoomsJsonConfig Instance => instance;

        public RoomConfigDictionary BiomeConfiguration => biomeDico;

        private static RoomConfigDictionary CreateEmptyDico() {
            var config = new RoomConfigDictionary();

            foreach (BiomeEnum biome in Enum.GetValues(typeof(BiomeEnum))) {
                config[biome] = new Dictionary<DifficultyEnum, Dictionary<RoomTypeEnum, Dictionary<RoomShapeEnum, List<string>>>>();

                foreach (DifficultyEnum difficulty in Enum.GetValues(typeof(DifficultyEnum))) {
                    config[biome][difficulty] = new Dictionary<RoomTypeEnum, Dictionary<RoomShapeEnum, List<string>>>();

                    foreach (RoomTypeEnum type in Enum.GetValues(typeof(RoomTypeEnum))) {
                        config[biome][difficulty][type] = new Dictionary<RoomShapeEnum, List<string>>();

                        foreach (RoomShapeEnum shape in Enum.GetValues(typeof(RoomShapeEnum))) {
                            config[biome][difficulty][type][shape] = new List<string>();
                        }
                    }
                }
            }

            return config;
        }

        private static void PopulateDico(RoomConfigDictionary config) {
            // string path = Path.Combine(Application.dataPath, GlobalConfig.Instance.ResourcesPath, GlobalConfig.Instance.PrefabRoomsVariantsPath);
            string path = Application.dataPath + GlobalConfig.Instance.ResourcesPath + GlobalConfig.Instance.PrefabRoomsVariantsPath;
            string[] prefabs = Directory.GetFiles(path);

            foreach (string prefab in prefabs) {
                if (Path.GetExtension(prefab) == ".meta") continue;

                BiomeEnum biomeEnum;
                DifficultyEnum diffEnum;
                RoomShapeEnum shapeEnum;
                RoomTypeEnum typeEnum;

                string nameWithoutPath = Path.GetFileNameWithoutExtension(prefab);
                string[] prefabValues = nameWithoutPath.Split('_');

                if (prefabValues.Length >= 4 &&
                    Enum.TryParse(prefabValues[0], true, out biomeEnum) &&
                    Enum.TryParse(prefabValues[1], true, out diffEnum) &&
                    Enum.TryParse(prefabValues[2], true, out typeEnum) &&
                    Enum.TryParse(prefabValues[3], true, out shapeEnum)) {

                    config[biomeEnum][diffEnum][typeEnum][shapeEnum].Add(nameWithoutPath);
                } else {
                    Debug.Log($"PopulateDico: Error with prefab {prefab}");
                }
            }
        }

        private static void SaveJson(RoomConfigDictionary config) {
            string jsonPath = Application.dataPath + GlobalConfig.Instance.ResourcesPath + $"{GlobalConfig.Instance.PrefabsRoomConfigJsonFile}.json";
            // string jsonPath = Path.Combine(Application.dataPath, GlobalConfig.Instance.ResourcesPath, $"{GlobalConfig.Instance.PrefabsRoomConfigJsonFile}.json");
            var toJson = JsonConvert.SerializeObject(config);
            File.WriteAllText(jsonPath, toJson);
        }

        public static RoomConfigDictionary LoadRoomDictionary() {
            string jsonPath = Application.dataPath + GlobalConfig.Instance.ResourcesPath + $"{GlobalConfig.Instance.PrefabsRoomConfigJsonFile}.json";
            if (File.Exists(jsonPath)) {
                string jsonFile = File.ReadAllText(jsonPath);
                return JsonConvert.DeserializeObject<RoomConfigDictionary>(jsonFile);
            }

            Debug.LogError("File not found at " + jsonPath);
            return null;
        }

        public static void CreateJson() {
            var config = CreateEmptyDico();
            PopulateDico(config);
            SaveJson(config);
        }
    }

    public class RoomConfigDictionary : Dictionary<BiomeEnum, Dictionary<DifficultyEnum, Dictionary<RoomTypeEnum, Dictionary<RoomShapeEnum, List<string>>>>> { }
}
