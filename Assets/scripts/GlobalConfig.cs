public class GlobalConfig {

    private static readonly GlobalConfig instance = new GlobalConfig();
    public static readonly string prefabRoomsPath = "Prefabs/Rooms/";
    public static readonly string prefabDoorsPath = "Prefabs/Doors/";
    public static readonly string resourcesPath = "/Resources/";
    public static readonly string prefabsRoomConfigJsonFile = prefabRoomsPath + "rooms_prefab_config";


    public static GlobalConfig Instance {
        get {
            return instance;
        }
    }
}
