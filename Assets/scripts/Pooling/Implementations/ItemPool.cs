using UnityEngine;

public class ItemPool : Pool<Item> {

    [SerializeField] private PoolConfig config;

    public PoolConfig GetConfig() {
        return config;
    }

    public void Setup(GameObject prefab, int poolSize) {
        GameObject obj = Instantiate(prefab);
        Item itemGo = obj.GetComponent<Item>();
        obj.SetActive(false);
        base.Setup(itemGo, poolSize);
    }
}