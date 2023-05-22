using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Tilemaps;
using Cinemachine;
using DoorNs;

namespace RoomNs {

    public class Room : MonoBehaviour {
        [SerializeField] public GameObject cam;
        [SerializeField] public GameObject tilemapEnnemiesGo;
        [SerializeField] public GameObject tilemapGo;
        [SerializeField] public GameObject tilemapLimitsGo;
        [SerializeField] public GameObject DoorsContainer;
        [SerializeField] public GameObject Door;

        public delegate void OnPlayerEnterRoom(Room room);
        public static event OnPlayerEnterRoom OnPlayerEnter;

        public bool isRootRoom;
        public bool isStartRoom = false;
        public Vector2Int rootPos;
        private RoomShape roomShape;
        [SerializeField] private int id;
        private List<Door> doors;
        private Tilemap tilemap;
        private Tilemap tilemapEnnemies;
        private Tilemap tilemapLimits;
        private CinemachineTransposer camTransposer;

        public List<Door> GetDoorsForRoom() {
            return doors;
        }

        public RoomShape GetRoomShape() {
            return roomShape;
        }

        public int GetId() {
            return id;
        }

        public void Setup(Vector2Int rootPos, RoomShape roomShape/*, int id*/) {
            this.rootPos = rootPos;
            this.roomShape = roomShape;
            // this.id = id;
        }

        private void Start() {
            tilemapEnnemies = tilemapEnnemiesGo.GetComponent<Tilemap>();
            tilemap = tilemapGo.GetComponent<Tilemap>();
            tilemapLimits = tilemapLimitsGo.GetComponent<Tilemap>();
            doors = new List<Door>();
            // ManageDoor();
        }

        public Tilemap GetTilemap() {
            return tilemap;
        }

        public Tilemap GetTilemapEnnemies() {
            return tilemapEnnemies;
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.name == "Player") {
                tilemapEnnemiesGo.SetActive(true);
                cam.SetActive(true);
                CinemachineVirtualCamera vcam = cam.GetComponent<CinemachineVirtualCamera>();
                vcam.m_Lens.OrthographicSize = 8; //camera lens
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

}