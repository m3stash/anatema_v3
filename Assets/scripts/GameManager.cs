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
        /*switch (door.GetDoorType()) {
            case DoorEnum.T:
            GoToNeighborDoor(door, 0, 1);
            break;
            case DoorEnum.TL:
            GoToNeighborDoor(door, 0, 1);
            break;
            case DoorEnum.TR:
            GoToNeighborDoor(door, 0, 1);
            break;
            case DoorEnum.B:
            GoToNeighborDoor(door, 0, -1);
            break;
            case DoorEnum.BL:
            GoToNeighborDoor(door, 0, -1);
            break;
            case DoorEnum.BR:
            GoToNeighborDoor(door, 0, -1);
            break;
            case DoorEnum.L:
            GoToNeighborDoor(door, -1, 0);
            break;
            case DoorEnum.LT:
            GoToNeighborDoor(door, -1, 0);
            break;
            case DoorEnum.LB:
            GoToNeighborDoor(door, -1, 0);
            break;
            case DoorEnum.R:
            GoToNeighborDoor(door, 1, 0);
            break;
            case DoorEnum.RT:
            GoToNeighborDoor(door, 1, 0);
            break;
            case DoorEnum.RB:
            GoToNeighborDoor(door, 1, 0);
            break;
        }*/
        Debug.Log("PLAYER AS CHANGE ROOM");
    }

    /*private void GoToNeighborDoor(Door door, int xTo, int yTo) {
        Vector2Int pos = currentRoom.rootPos;
        Room roomTo = null;
        int x = pos.x + xTo;
        int y = pos.y + yTo;
         // if room > 1*1 so room pos is not a good position player can be in top of room or top right of room
         // then yo must add a position of player in room not only root position !
        if (currentRoom.GetRoomShape() != RoomShapeEnum.ROOMSHAPE_1x1) {
            int playerGridPosInRoomX = (int)player.transform.position.x / 61;
            int playerGridPosInRoomY = (int)player.transform.position.y / 31;
            int RoomGridWorldPositionX = (int)currentRoom.transform.position.x / 61;
            int RoomGridWorldPositionY = (int)currentRoom.transform.position.y / 31;
            if (playerGridPosInRoomX > RoomGridWorldPositionX) {
                x += (playerGridPosInRoomX - RoomGridWorldPositionX);
            }
            if (playerGridPosInRoomY > RoomGridWorldPositionY) {
                y += (playerGridPosInRoomY - RoomGridWorldPositionY);
            }
        }
        roomTo = generator.GetRoomFromVector2Int(new Vector2Int(x, y));
        currentRoom = roomTo;
        Vector3 neighbor = new Vector3(door.transform.position.x + xTo, door.transform.position.y + yTo, door.transform.position.z);
        List<Door> doorList = roomTo.GetDoorsForRoom();
        Door neighborDoor = doorList.Find(d => d.transform.position == neighbor);
         if (neighborDoor != null) {
            // *2 because if tp just on door then player is infinit re TP on other room etc..
            player.transform.position = new Vector3(neighborDoor.transform.position.x + xTo, neighborDoor.transform.position.y + yTo, player.transform.position.z);
        } else {
            Debug.Log("ERROR NO DOORS FIND at => GoToNeighborDoor()");
        }
    }*/

    private void OnDisable() {
        Door.OnChangeRoom -= ChangeRoom;
        Room.OnPlayerEnter -= PlayerEnterRoom;
    }

}
