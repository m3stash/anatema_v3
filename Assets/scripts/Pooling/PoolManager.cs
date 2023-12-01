using UnityEngine;

public class PoolManager : MonoBehaviour {
    [SerializeField] GameObject doorPool;
    [SerializeField] GameObject itemPool;

    public DoorPool GetDoorPool() {
        if(doorPool != null) {
            return doorPool.GetComponent<DoorPool>();
        }
        Debug.LogError("Error : No SerializeField associate to doorPool");
        return null;
    }

    public ItemPool GetItemPool() {
        if (itemPool != null) {
            return itemPool.GetComponent<ItemPool>();
        }
        Debug.LogError("Error : No SerializeField associate to itemPool");
        return null;
    }
}