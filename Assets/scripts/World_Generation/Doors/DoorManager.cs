using UnityEngine;
using DoorNs;
using RoomNs;

public class DoorManager : IDoorManager {

    private IDoorFactory doorFactory;

    public DoorManager(IDoorFactory doorFactory) {
        this.doorFactory = doorFactory;
    }

    public GameObject InstantiateRoomPrefab(string path) {
        return doorFactory.InstantiateDoorPrefab(path);
    }

    public GameObject InstantiateDoorGO(GameObject roomPrefab, Vector3 vector3, Transform transform, Transform parentTransform) {
        return doorFactory.InstantiateDoorGO(roomPrefab, vector3, transform, parentTransform);
    }

    public void SetProperties(GameObject doorGameobject, Door door, Biome biome) {
        DoorGO doorInstance = doorGameobject.GetComponent<DoorGO>();
        doorInstance.SetDirection(door.GetDirection());
        doorInstance.SetSpriteRender(biome);
        doorInstance.transform.localPosition = door.LocalPosition;

    }
}
