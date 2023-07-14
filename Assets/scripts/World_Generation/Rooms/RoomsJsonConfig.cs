using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

namespace RoomNs {

    [System.Serializable]
    [ExecuteInEditMode]
    public class RoomsJsonConfig {

        private static readonly RoomsJsonConfig instance = new RoomsJsonConfig();
        private static Dictionary<BiomeEnum, Dictionary<DifficultyEnum, Dictionary<RoomTypeEnum, Dictionary<RoomShapeEnum, List<string>>>>> biomeDico;

        public Dictionary<BiomeEnum, Dictionary<DifficultyEnum, Dictionary<RoomTypeEnum, Dictionary<RoomShapeEnum, List<string>>>>> GetBiomeDico() {
            return biomeDico;
        }

        public static RoomsJsonConfig Instance {
            get {
                return instance;
            }
        }

        private static void CreateDico() {
            biomeDico = new Dictionary<BiomeEnum, Dictionary<DifficultyEnum, Dictionary<RoomTypeEnum, Dictionary<RoomShapeEnum, List<string>>>>>();
            Enum.GetValues(typeof(BiomeEnum)).Cast<BiomeEnum>().ToList().ForEach(delegate (BiomeEnum biome) {
                biomeDico[biome] = new Dictionary<DifficultyEnum, Dictionary<RoomTypeEnum, Dictionary<RoomShapeEnum, List<string>>>>();
                Enum.GetValues(typeof(DifficultyEnum)).Cast<DifficultyEnum>().ToList().ForEach(delegate (DifficultyEnum difficulty) {
                    biomeDico[biome][difficulty] = new Dictionary<RoomTypeEnum, Dictionary<RoomShapeEnum, List<string>>>();
                    Enum.GetValues(typeof(RoomTypeEnum)).Cast<RoomTypeEnum>().ToList().ForEach(delegate (RoomTypeEnum type) {
                        biomeDico[biome][difficulty][type] = new Dictionary<RoomShapeEnum, List<string>>();
                        Enum.GetValues(typeof(RoomShapeEnum)).Cast<RoomShapeEnum>().ToList().ForEach(delegate (RoomShapeEnum shape) {
                            biomeDico[biome][difficulty][type][shape] = new List<string>();
                        });
                    });
                });
            });
        }

        private static void PopulateDico() {
            string path = Application.dataPath + GlobalConfig.resourcesPath + GlobalConfig.prefabRoomsVariantsPath;
            string[] prefabs = Directory.GetFiles(path);
            foreach (var prefab in prefabs) {
                if (prefab.Contains(".meta"))
                    continue;
                BiomeEnum biomeEnum;
                DifficultyEnum diffEnum;
                RoomShapeEnum shapeEnum;
                RoomTypeEnum typeEnum;
                string nameWithoutPath = prefab.Replace(path, "");
                string[] prefabValues = nameWithoutPath.Split('_');

                if (prefabValues.Length > 0) {
                    string strBiome = prefabValues[0].ToUpper();
                    string strDiff = prefabValues[1].ToUpper();
                    string strType = prefabValues[2].ToUpper();
                    string strShape = prefabValues[3].ToUpper();
                    // string name = prefabValues[4];
                    string name = nameWithoutPath.Replace(".prefab", "");
                    if (Enum.TryParse(strBiome, out biomeEnum) && Enum.TryParse(strDiff, out diffEnum) && Enum.TryParse(strType, out typeEnum) && Enum.TryParse(strShape, out shapeEnum)) {
                        biomeDico[biomeEnum][diffEnum][typeEnum][shapeEnum].Add(name);
                    }
                } else {
                    Debug.Log("PopulateDico: Error No Prefabs Available");
                }
            }
        }

        private static void SaveJson() {
            var toJson = JsonConvert.SerializeObject(biomeDico);
            File.WriteAllText(Application.dataPath + GlobalConfig.resourcesPath + GlobalConfig.prefabsRoomConfigJsonFile + ".json", toJson);
        }

        public static Dictionary<BiomeEnum, Dictionary<DifficultyEnum, Dictionary<RoomTypeEnum, Dictionary<RoomShapeEnum, List<string>>>>> GetRoomDictionary() {
            string jsonFile = File.ReadAllText(Application.dataPath + GlobalConfig.resourcesPath + GlobalConfig.prefabsRoomConfigJsonFile + ".json");
            if (jsonFile != null) {
                return JsonConvert.DeserializeObject<Dictionary<BiomeEnum, Dictionary<DifficultyEnum, Dictionary<RoomTypeEnum, Dictionary<RoomShapeEnum, List<string>>>>>>(jsonFile);
            }
            Debug.LogError("File not found at Resources/myData");
            return null;
        }

        public static void CreateJson() {
            CreateDico();
            PopulateDico();
            SaveJson();
        }

    }
}