using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour {

    [SerializeField] static private GameObject player;
    [SerializeField] private GameObject roomContainer;

    private LevelGenerator levelGenerator;
    private Vector2Int playerSpawnPoint;
    private Room currentRoom;

    public static GameManager instance;

    private void Awake() {
        instance = this;
    }

    private void OnEnable() {
        Door.OnChangeRoom += ChangeRoom;
    }

    public static GameObject GetPlayer() {
        return player;
    }

    private void Start() {
        levelGenerator = GetComponent<LevelGenerator>();
        levelGenerator.StartGeneration(roomContainer);
        currentRoom = levelGenerator.GetRoomFromVector2Int(Vector2Int.zero);
        player = Instantiate(Resources.Load<GameObject>("Prefabs/Characters/Player"), new Vector3(currentRoom.transform.position.x + 10, currentRoom.transform.position.y + 10), transform.rotation);
    }

    private void ChangeRoom(Door door) {
        switch (door.GetDoorType()) {
            case DoorEnum.T:
            GoToNeighboorDoor(door, 0, 1);
            break;
            case DoorEnum.TL:
            GoToNeighboorDoor(door, 0, 1);
            break;
            case DoorEnum.TR:
            GoToNeighboorDoor(door, 0, 1);
            break;
            case DoorEnum.B:
            GoToNeighboorDoor(door, 0, -1);
            break;
            case DoorEnum.BL:
            GoToNeighboorDoor(door, 0, -1);
            break;
            case DoorEnum.BR:
            GoToNeighboorDoor(door, 0, -1);
            break;
            case DoorEnum.L:
            GoToNeighboorDoor(door, -1, 0);
            break;
            case DoorEnum.LT:
            GoToNeighboorDoor(door, -1, 0);
            break;
            case DoorEnum.LB:
            GoToNeighboorDoor(door, -1, 0);
            break;
            case DoorEnum.R:
            GoToNeighboorDoor(door, 1, 0);
            break;
            case DoorEnum.RT:
            GoToNeighboorDoor(door, 1, 0);
            break;
            case DoorEnum.RB:
            GoToNeighboorDoor(door, 1, 0);
            break;
        }
    }

    private void GoToNeighboorDoor(Door door, int xTo, int yTo) {
        Vector2Int pos = currentRoom.rootPos;
        Room roomTo = null;
        int x = pos.x + xTo;
        int y = pos.y + yTo;
        /*
         * if room > 1*1 so room pos is not a good position player can be in top of room or top right of room
         * then yo must add a position of player in room not only root position !
        */
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
        roomTo = levelGenerator.GetRoomFromVector2Int(new Vector2Int(x, y));
        currentRoom = roomTo;
        Vector3 neighboor = new Vector3(door.transform.position.x + xTo, door.transform.position.y + yTo, door.transform.position.z);
        List<Door> doorList = roomTo.GetDoorsForRoom();
        Door neighboorDoor = doorList.Find(d => d.transform.position == neighboor);
        if (neighboorDoor) {
            // *2 because if tp just on door then player is infinit re TP on other room etc..
            player.transform.position = new Vector3(neighboorDoor.transform.position.x + xTo, neighboorDoor.transform.position.y + yTo, player.transform.position.z);
        } else {
            Debug.Log("ERROR NO DOORS FIND at => GoToNeighboorDoor()");
        }
    }

    private void OnDisable() {
        Door.OnChangeRoom -= ChangeRoom;
    }

}
