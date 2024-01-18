using UnityEngine;

namespace RoomUI {
    public class RoomUIManager : MonoBehaviour {

        [SerializeField] GameObject roomGrid;
        [SerializeField] GameObject itemGrid;
        [SerializeField] GameObject stateManager;
        [SerializeField] GameObject itemPool;

        private ObjectPool pool;
        private RoomGridManager roomGridManager;
        private ObjectsManager objectsManager;
        private RoomUIStateManager roomUIStateManager;

        public ObjectSlotGO GetObjectCell() {
            ObjectSlotGO slot = pool.GetOne();
            GameObject cellGo = slot.gameObject;
            cellGo.SetActive(true);
            return slot;
        }

        public void ReleaseSlot(ObjectSlotGO slot) {
            pool.ReleaseOne(slot);
        }

        private void Awake() {
            VerifySerialisables();
            CreatePooling();
            InitComponents();
        }

        private ObjectPool CreatePooling() {
            pool = itemPool.GetComponent<ObjectPool>();
            PoolConfig config = pool.GetConfig();
            if (!config) {
                Debug.LogError("Error no config for cellPool on GridManager awake !");
            } else {
                pool.Setup(config.GetPrefab(), config.GetPoolSize());
            }
            return pool;
        }

        private void VerifySerialisables() {
            if (roomGridManager == null) {
                Debug.Log("Error: SerializeField roomGrid not Set !");
            }
            if (objectsManager == null) {
                Debug.Log("Error: SerializeField itemGridManager not Set !");
            }
            if (stateManager == null) {
                Debug.Log("Error: SerializeField stateManager not Set !");
            }
            if (pool == null) {
                Debug.Log("Error: SerializeField pool not Set !");
            }
        }

        private void InitComponents() {
            roomGridManager = roomGrid.GetComponent<RoomGridManager>();
            objectsManager = itemGrid.GetComponent<ObjectsManager>();
            objectsManager.Setup(this);
            roomUIStateManager = stateManager.GetComponent<RoomUIStateManager>();
        }

    }

}
