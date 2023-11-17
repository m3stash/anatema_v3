using UnityEngine;

[CreateAssetMenu(fileName = "PoolConfig", menuName = "Pooling/PoolConfig")]
public class PoolConfig : ScriptableObject {
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize;

    public GameObject GetPrefab() {
        return prefab;
    }

    public int GetPoolSize() {
        return poolSize;
    }

}