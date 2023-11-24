using UnityEngine;

public class ItemPool : Pool<ItemGO> {

    [SerializeField] private PoolConfig config;

    public PoolType GetPoolType() {
        return PoolType.ITEM;
    }

    public PoolConfig GetConfig() {
        return config;
    }

    public void Setup(GameObject prefab, int poolSize) {
        GameObject obj = Instantiate(prefab, new Vector3(0, 0, 0), transform.rotation);
        ItemGO itemGO = obj.GetComponent<ItemGO>();
        obj.SetActive(false);
        base.Setup(itemGO, poolSize);
    }

}