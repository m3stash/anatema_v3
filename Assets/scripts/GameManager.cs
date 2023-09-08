using UnityEngine;
using DungeonNs;
using RoomNs;
using DoorNs;
using UnityEditor.PackageManager;

public class GameManager : MonoBehaviour {

    [SerializeField] static private GameObject player;
    [SerializeField] private GameObject floorContainer;

    private Generator generator;
    // private Vector2Int playerSpawnPoint;
    private static RoomGO roomGO;
    private bool firstRoomInit = false;
    private static string seed;
    private static Config config;
    public static GameManager instance;

    public static string GetSeed { get { return seed; } }

    private void Awake() {
        instance = this;
        seed = SeedGenerator(8);
    }

    private void OnEnable() {
        DoorGO.OnChangeRoom += ChangeRoom;
        RoomGO.OnPlayerEnter += PlayerEnterRoom;
    }

    private string SeedGenerator(int length) {
        const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string seed = "";
        for (int i = 0; i < length; i++) {
            int randomIndex = UnityEngine.Random.Range(0, characters.Length);
            seed += characters[randomIndex];
        }
        return seed;
    }

    public void PlayerEnterRoom(RoomGO roomGO) {
        if (!firstRoomInit) {
            firstRoomInit = true;
            // currentDungeon.InitBackgroundContainer();
        }
    }

    public static GameObject GetPlayer() {
        return player;
    }

    public static RoomGO GetCurrentRoom() {
        return roomGO;
    }

    public static GameObject GetFloorContainer() {
        return instance.floorContainer;
    }

    private void Start() {
        generator = GetComponent<Generator>();
        config = new Config(BiomeEnum.CAVE, DifficultyEnum.EASY, RoomSizeEnum.L, 1);
        generator.StartGeneration();
        /*currentRoom = generator.GetRoomFromVector2Int(Vector2Int.zero);
        player = GameObject.FindGameObjectWithTag("Player");*/
    }

    public static Config GetCurrentDungeonConfig() {
        return config;
    }

    private void ChangeRoom(DoorGO doorGO) {
        /*switch(door.GetDirection()){
            case DirectionalEnum.L:
                player.transform.position = new Vector3(door.GetLocalPosition().x - 10, door.GetLocalPosition().y, player.transform.position.z);
            break;
            case DirectionalEnum.R:
                player.transform.position = new Vector3(door.GetLocalPosition().x + 10, door.GetLocalPosition().y, player.transform.position.z);
            break;
            case DirectionalEnum.T:
                player.transform.position = new Vector3(door.GetLocalPosition().x + 10, door.GetLocalPosition().y, player.transform.position.z);
            break;
            case DirectionalEnum.B:
                player.transform.position = new Vector3(door.GetLocalPosition().x - 10, door.GetLocalPosition().y, player.transform.position.z);
            break;
        }*/
        print(GetPlayer());
        switch (doorGO.GetDirection()) {
            case DirectionalEnum.L:
            player.transform.position = new Vector3(doorGO.GetLocalPosition().x - 10, doorGO.GetLocalPosition().y, player.transform.position.z);
            break;
            case DirectionalEnum.R:
            player.transform.position = new Vector3(doorGO.GetLocalPosition().x + 10, doorGO.GetLocalPosition().y, player.transform.position.z);
            break;
            case DirectionalEnum.T:
            player.transform.position = new Vector3(doorGO.GetLocalPosition().x + 10, doorGO.GetLocalPosition().y, player.transform.position.z);
            break;
            case DirectionalEnum.B:
            player.transform.position = new Vector3(doorGO.GetLocalPosition().x - 10, doorGO.GetLocalPosition().y, player.transform.position.z);
            break;
        }
        Debug.Log("PLAYER AS CHANGE ROOM "+ doorGO.GetLocalPosition());
    }

    private void OnDisable() {
        DoorGO.OnChangeRoom -= ChangeRoom;
        RoomGO.OnPlayerEnter -= PlayerEnterRoom;
    }

}
