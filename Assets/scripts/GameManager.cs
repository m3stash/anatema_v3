using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour {

    [SerializeField] static private GameObject player;
    [SerializeField] private GameObject dungeonContainer;

    private DungeonGenerator generator;
    private Vector2Int playerSpawnPoint;
    private static Room currentRoom;
    private DungeonConfig dungeon;
    private bool firstRoomInit = false;
    private Dungeon currentDungeon;

    public static GameManager instance;

    private void Awake() {
        instance = this;
    }

    private void OnEnable() {
        Door.OnChangeRoom += ChangeRoom;
        Room.OnPlayerEnter += PlayerEnterRoom;
    }
 
    public void PlayerEnterRoom(Room room) {
        if (!firstRoomInit) {
            firstRoomInit = true;
            // currentDungeon.InitBackgroundContainer();
        }
    }

    public static GameObject GetPlayer() {
        return player;
    }

    public static Room GetCurrentRoom() {
        return currentRoom;
    }

    private void Start() {
        generator = GetComponent<DungeonGenerator>();
        currentDungeon = dungeonContainer.GetComponent<Dungeon>();
        currentDungeon.Setup(new DungeonConfig(BiomeEnum.Cave, DifficultyEnum.Easy, RoomSizeEnum.L));
        generator.StartGeneration(dungeonContainer, currentDungeon.GetConfig());
        /*currentRoom = generator.GetRoomFromVector2Int(Vector2Int.zero);
        player = GameObject.FindGameObjectWithTag("Player");*/
    }

    private void ChangeRoom(Door door) {
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
        switch (door.GetDirection()) {
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
        }
        Debug.Log("PLAYER AS CHANGE ROOM "+door.GetLocalPosition());
    }

    private void OnDisable() {
        Door.OnChangeRoom -= ChangeRoom;
        Room.OnPlayerEnter -= PlayerEnterRoom;
    }

}
