using UnityEngine;

public class GlobalConfig {

    private static readonly GlobalConfig instance = new GlobalConfig();

    private readonly string prefabRoomsPath;
    private readonly string prefabRoomsVariantsPath;
    private readonly string prefabDoorsPath;
    private readonly string resourcesPath;
    private readonly string prefabsRoomConfigJsonFile;
    private readonly string directoryResourceFolder;

    private GlobalConfig() {
        prefabRoomsPath = "Prefabs/Rooms/";
        prefabRoomsVariantsPath = prefabRoomsPath + "Variants/";
        prefabDoorsPath = "Prefabs/Doors/";
        resourcesPath = "/Resources/";
        prefabsRoomConfigJsonFile = prefabRoomsPath + "rooms_prefab_config";
        directoryResourceFolder = Application.dataPath + "/Resources/";
    }

    public static GlobalConfig Instance {
        get {
            return instance;
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

    public string PrefabDoorsPath {
        get {
            return prefabDoorsPath;
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
