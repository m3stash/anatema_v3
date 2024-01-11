using UnityEngine;

public class CellRoomPool : Pool<CellRoomGO> {

    [SerializeField] private PoolConfig config;

    public PoolType GetPoolType() {
        return PoolType.UICell;
    }

    public PoolConfig GetConfig() {
        return config;
    }

    public void Setup(GameObject prefab, int poolSize) {
        GameObject obj = Instantiate(prefab);
        CellRoomGO cellRoomGO = obj.GetComponent<CellRoomGO>();
        obj.SetActive(false);
        base.Setup(cellRoomGO, poolSize);
    }
}