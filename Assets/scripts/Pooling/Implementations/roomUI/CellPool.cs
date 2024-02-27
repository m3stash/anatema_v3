using TreeEditor;
using UnityEngine;

public class CellPool : Pool<CellGO> {

    [SerializeField] private PoolConfig config;

    public PoolType GetPoolType() {
        return PoolType.UICell;
    }

    public PoolConfig GetConfig() {
        return config;
    }

    public void Setup(GameObject prefab, int poolSize) {
        GameObject obj = Instantiate(prefab, transform);
        CellGO cellGO = obj.GetComponent<CellGO>();
        obj.SetActive(false);
        base.Setup(cellGO, poolSize);
    }
}