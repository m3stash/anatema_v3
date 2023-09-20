using UnityEngine;
using DungeonNs;
using RoomNs;
using DoorNs;

public class GameManager : MonoBehaviour {

    [SerializeField] static private GameObject player;
    [SerializeField] private GameObject floorContainer;
    [SerializeField] private Generator generator;
    [SerializeField] private BiomeManager biomeManager;

    // private Vector2Int playerSpawnPoint;
    private static RoomGO roomGO;
    private bool firstRoomInit = false;
    private static string seed;
    private static CurrentFloorConfig floorConfig;
    public static GameManager instance;

    private void Awake() {
        instance = this;

    }

    private void OnEnable() {
        DoorGO.OnChangeRoom += ChangeRoom;
        RoomGO.OnPlayerEnter += PlayerEnterRoom;
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

    private void Start() {
        floorConfig = new CurrentFloorConfig(BiomeEnum.CAVE, DifficultyEnum.EASY, RoomSizeEnum.L, 1);
        generator.GenerateDungeon(floorConfig, floorContainer, biomeManager);
        /*currentRoom = generator.GetRoomFromVector2Int(Vector2Int.zero);
        player = GameObject.FindGameObjectWithTag("Player");*/
    }

    private void ChangeRoom(DoorGO doorGO) {
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
