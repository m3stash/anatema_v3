using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour {

    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject roomContainer;

    private LevelGenerator levelGenerator;
    private Vector2Int playerSpawnPoint;
    private CinemachineConfiner confiner;

    public static GameManager instance;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        levelGenerator = GetComponent<LevelGenerator>();
        levelGenerator.StartGeneration(roomContainer);
        playerSpawnPoint = levelGenerator.GetStartPosition();
        confiner = virtualCamera.GetComponent<CinemachineConfiner>();
        Room room = levelGenerator.GetRoomFromVector2Int(playerSpawnPoint);
        confiner.m_BoundingShape2D = room.GetComponent<Room>().Polygone2D.GetComponent<PolygonCollider2D>();
        // player = Instantiate(Resources.Load<GameObject>("Prefabs/Characters/Player"), new Vector3(room.transform.position.x + 10, room.transform.position.y + 10), transform.rotation);
        player = Instantiate(Resources.Load<GameObject>("Prefabs/Characters/Player"), new Vector3(35, -2), transform.rotation);
        virtualCamera.Follow = player.transform;
    }

}
