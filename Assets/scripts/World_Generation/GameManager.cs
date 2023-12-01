using UnityEngine;
using DungeonNs;
using RoomNs;
using DoorNs;

public class GameManager : MonoBehaviour {

    [SerializeField] private GameObject player;
    [SerializeField] private DungeonManager dungeonManager;
    private IDungeonFloorConfig floorConfig;

    private void OnEnable() {
        DoorGO.OnChangeRoom += ChangeRoom;
        RoomGO.OnPlayerEnter += PlayerEnterRoom;
    }

    public void PlayerEnterRoom(RoomGO roomGO) {
        //
    }
  
    private void Start() {
        floorConfig = new DungeonFloorConfig(BiomeEnum.CAVE, DifficultyEnum.EASY, 1);
        dungeonManager.Setup(floorConfig);
        /*currentRoom = generator.GetRoomFromVector2Int(Vector2Int.zero);
        player = GameObject.FindGameObjectWithTag("Player");*/
    }

    private void ChangeRoom(DoorGO doorGO) {
        // Debug.Log("PLAYER AS CHANGE ROOM "+ doorGO.GetLocalPosition());
        /*switch (doorGO.GetDirection()) {
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
        }*/
    }

    private void OnDisable() {
        DoorGO.OnChangeRoom -= ChangeRoom;
        RoomGO.OnPlayerEnter -= PlayerEnterRoom;
    }

}
