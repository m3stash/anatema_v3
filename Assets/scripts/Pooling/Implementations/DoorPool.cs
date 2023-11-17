using UnityEngine;
using DoorNs;

public class DoorPool : Pool<DoorGO> {

    [SerializeField] private PoolConfig config;

    public PoolType GetPoolType() {
        return PoolType.DOOR;
    }

    public PoolConfig GetConfig() {
        return config;
    }

    public void Setup(GameObject prefab, int poolSize) {
        GameObject obj = Instantiate(prefab, new Vector3(0, 0, 0), transform.rotation);
        DoorGO doorGO = obj.GetComponent<DoorGO>();
        obj.SetActive(false);
        base.Setup(doorGO, poolSize);
    }

}