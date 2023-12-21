using UnityEngine;

public class PoolManager : MonoBehaviour {
    [SerializeField] GameObject doorPool;

    public DoorPool GetDoorPool() {
        if(doorPool != null) {
            return doorPool.GetComponent<DoorPool>();
        }
        Debug.LogError("Error : No SerializeField associate to doorPool");
        return null;
    }

}