using UnityEngine;

public class PoolManager : MonoBehaviour {
    [SerializeField] GameObject doorPool;
    private DoorManager doorManager;

    public void Setup() {
        if(doorPool != null) {
            doorManager = doorPool.GetComponent<DoorManager>();
            doorManager.Setup();
        } else {
            Debug.LogError("PoolManager mising doorPool serialize");
        }
    }

    public DoorManager GetDoorManager() {
        return doorManager;
    }
}