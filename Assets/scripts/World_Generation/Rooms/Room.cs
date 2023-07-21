using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Tilemaps;
using Cinemachine;
using DoorNs;

namespace RoomNs {

    public class Room : MonoBehaviour {
        [SerializeField] private GameObject cam;
        [SerializeField] private GameObject tilemapEnnemiesGo;
        [SerializeField] private GameObject tilemapGo;
        [SerializeField] private GameObject tilemapLimitsGo;
        [SerializeField] private GameObject Door;
        [SerializeField] public GameObject DoorsContainer;

        public delegate void OnPlayerEnterRoom(Room room);
        public static event OnPlayerEnterRoom OnPlayerEnter;

        public bool isRootRoom;
        public Vector2Int rootPos;
        private RoomShapeEnum roomShape;
        private List<Door> doors;
        private Tilemap tilemap;
        private Tilemap tilemapEnnemies;
        private Tilemap tilemapLimits;
        private CinemachineTransposer camTransposer;

        public List<Door> GetDoorsForRoom() {
            return doors;
        }

        public RoomShapeEnum GetShape() {
            return roomShape;
        }

        public void Setup(Vector2Int rootPos, RoomShapeEnum roomShape) {
            this.rootPos = rootPos;
            this.roomShape = roomShape;
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