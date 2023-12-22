using UnityEngine;

public class ObjectPool : Pool<ObjectSlotGO> {

    [SerializeField] private PoolConfig config;

    public PoolConfig GetConfig() {
        return config;
    }

    public void Setup(GameObject prefab, int poolSize) {
        GameObject obj = Instantiate(prefab);
        ObjectSlotGO itemGo = obj.GetComponent<ObjectSlotGO>();
        obj.SetActive(false);
        base.Setup(itemGo, poolSize);
    }
}