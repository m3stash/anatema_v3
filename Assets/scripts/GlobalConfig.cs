using UnityEngine;

public class GlobalConfig {

    private static readonly GlobalConfig instance = new GlobalConfig();

    private readonly string prefabRoomsPath;
    private readonly string prefabRoomsVariantsPath;
    private readonly string resourcesPath;
    private readonly string prefabObjectPath;
    private readonly string prefabsRoomConfigJsonFile;
    private readonly string directoryResourceFolder;
    private readonly string scriptablePath;

    private GlobalConfig() {
        prefabRoomsPath = "Prefabs/Rooms/";
        prefabRoomsVariantsPath = prefabRoomsPath + "Variants/";
        prefabObjectPath = "Prefabs/object/";
        resourcesPath = "/Resources/";
        prefabsRoomConfigJsonFile = prefabRoomsPath + "rooms_prefab_config";
        directoryResourceFolder = Application.dataPath + "/Resources/";
        scriptablePath = "Scriptables/object/";
    }

    public static GlobalConfig Instance {
        get {
            return instance;
        }
    }

    public string ScriptablePath {
        get {
            return scriptablePath;
        }
    }

    public string PrefabRoomsPath {
        get {
            return prefabRoomsPath;
        }
    }

    public string PrefabRoomsVariantsPath {
        get {
            return prefabRoomsVariantsPath;
        }
    }

    public string PrefabObjectPath {
        get {
            return prefabObjectPath;
        }
    }

    public string ResourcesPath {
        get {
            return resourcesPath;
        }
    }

    public string PrefabsRoomConfigJsonFile {
        get {
            return prefabsRoomConfigJsonFile;
        }
    }

    public string DirectoryResourceFolder {
        get {
            return directoryResourceFolder;
        }
    }
}
