using UnityEngine;
using UnityEngine.UI;

public class CellPool : Pool<CellGO> {

    [SerializeField] private PoolConfig config;

    public PoolType GetPoolType() {
        return PoolType.UICell;
    }

    public PoolConfig GetConfig() {
        return config;
    }

    public void Setup(GameObject prefab, int poolSize) {
        GameObject obj = Instantiate(prefab);
        CellGO cellGO = obj.GetComponent<CellGO>();
        obj.SetActive(false);
        base.Setup(cellGO, poolSize);
    }
}