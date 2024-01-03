using UnityEngine;

public class TabCellPool : Pool<TabCellGO> {

    [SerializeField] private PoolConfig config;

    public PoolConfig GetConfig() {
        return config;
    }

    public void Setup(GameObject prefab, int poolSize) {
        GameObject obj = Instantiate(prefab);
        TabCellGO tabcellGO = obj.GetComponent<TabCellGO>();
        obj.SetActive(false);
        base.Setup(tabcellGO, poolSize);
    }
}