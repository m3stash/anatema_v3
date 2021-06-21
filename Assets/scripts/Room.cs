using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Tilemaps;
using Cinemachine;

public class Room : MonoBehaviour {
    [SerializeField] public GameObject cam;
    [SerializeField] public RoomConfig roomConfig;
    [SerializeField] public GameObject tilemapEnnemiesGo;
    [SerializeField] public GameObject tilemapGo;
    [SerializeField] public GameObject tilemapDoorsGo;
    [SerializeField] public GameObject tilemapLimitsGo;
    [SerializeField] public bool enable_door_T = true;
    [SerializeField] public bool enable_door_TL = true;
    [SerializeField] public bool enable_door_TR = true;
    [SerializeField] public bool enable_door_B = true;
    [SerializeField] public bool enable_door_BL = true;
    [SerializeField] public bool enable_door_BR = true;
    [SerializeField] public bool enable_door_L = true;
    [SerializeField] public bool enable_door_LT = true;
    [SerializeField] public bool enable_door_LB = true;
    [SerializeField] public bool enable_door_R = true;
    [SerializeField] public bool enable_door_RT = true;
    [SerializeField] public bool enable_door_RB = true;

    public delegate void OnPlayerEnterRoom(Room room);
    public static event OnPlayerEnterRoom OnPlayerEnter;

    public bool isRootRoom;
    public bool isStartRoom = false;
    public Vector2Int rootPos;
    private RoomShapeEnum roomShape;
    [SerializeField] private int id;
    private List<Door> doors;
    private Tilemap tilemap;
    private Tilemap tilemapEnnemies;
    private Tilemap tilemapDoors;
    private Tilemap tilemapLimits;
    private CinemachineTransposer camTransposer;

    public List<Door> GetDoorsForRoom() {
        return doors;
    }

    public RoomShapeEnum GetRoomShape() {
        return roomShape;
    }

    public int GetId() {
        return id;
    }

    public void Setup(Vector2Int rootPos, RoomShapeEnum roomShape, int id) {
        this.rootPos = rootPos;
        this.roomShape = roomShape;
        this.id = id;
    }

    private void Start() {
        tilemapEnnemies = tilemapEnnemiesGo.GetComponent<Tilemap>();
        tilemap = tilemapGo.GetComponent<Tilemap>();
        tilemapDoors = tilemapDoorsGo.GetComponent<Tilemap>();
        tilemapLimits = tilemapLimitsGo.GetComponent<Tilemap>();
        doors = new List<Door>();
        doors.AddRange(tilemapDoors.GetComponentsInChildren<Door>());
        ManageDoor();
    }

    public Tilemap GetTilemap() {
        return tilemap;
    }

    public Tilemap GetTilemapEnnemies() {
        return tilemapEnnemies;
    }

    private void ManageDoor() {
        if (!enable_door_T || roomConfig.GetDisable_door_T()) {
            FindAndDisableDoor(DoorEnum.T);
        }
        if (!enable_door_TL || roomConfig.GetDisable_door_TL()) {
            FindAndDisableDoor(DoorEnum.TL);
        }
        if (!enable_door_TR || roomConfig.GetDisable_door_TR()) {
            FindAndDisableDoor(DoorEnum.TR);
        }
        if (!enable_door_B || roomConfig.GetDisable_door_B()) {
            FindAndDisableDoor(DoorEnum.B);
        }
        if (!enable_door_BL || roomConfig.GetDisable_door_BL()) {
            FindAndDisableDoor(DoorEnum.BL);
        }
        if (!enable_door_BR || roomConfig.GetDisable_door_BR()) {
            FindAndDisableDoor(DoorEnum.BR);
        }
        if (!enable_door_L || roomConfig.GetDisable_door_L()) {
            FindAndDisableDoor(DoorEnum.L);
        }
        if (!enable_door_LT || roomConfig.GetDisable_door_LT()) {
            FindAndDisableDoor(DoorEnum.LT);
        }
        if (!enable_door_LB || roomConfig.GetDisable_door_LB()) {
            FindAndDisableDoor(DoorEnum.LB);
        }
        if (!enable_door_R || roomConfig.GetDisable_door_R()) {
            FindAndDisableDoor(DoorEnum.R);
        }
        if (!enable_door_RT || roomConfig.GetDisable_door_RT()) {
            FindAndDisableDoor(DoorEnum.RT);
        }
        if (!enable_door_RB || roomConfig.GetDisable_door_RB()) {
            FindAndDisableDoor(DoorEnum.RB);
        }
    }

    private void FindAndDisableDoor(DoorEnum doorType) {
        Door currentDoor = doors.Find(door => door.GetDoorType() == doorType);
        if (!currentDoor) {
            return;
        }
        TileBase tile = Resources.Load<TileBase>("Sprites/Tiles/Rules/Dirt_rules");
        tilemapLimits.SetColliderType(new Vector3Int((int)currentDoor.transform.localPosition.x, (int)currentDoor.transform.localPosition.y, (int)currentDoor.transform.position.z), Tile.ColliderType.Sprite);
        tilemapLimits.SetTile(new Vector3Int((int)currentDoor.transform.localPosition.x, (int)currentDoor.transform.localPosition.y, (int)currentDoor.transform.position.z), tile);
        currentDoor.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.name == "Player") {
            tilemapEnnemiesGo.SetActive(true);
            cam.SetActive(true);
            CinemachineVirtualCamera vcam = cam.GetComponent<CinemachineVirtualCamera>();
            vcam.m_Lens.OrthographicSize = 4; //camera lens
            vcam.m_Follow = collision.transform;
            OnPlayerEnter(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.name == "Player") {
            tilemapEnnemiesGo.SetActive(false);
            cam.SetActive(false);
        }
    }

}
