using System.Collections.Generic;
using UnityEngine;
namespace RoomUI {
    public class RoomUIManager : MonoBehaviour {

        [SerializeField] GameObject roomGrid;
        [SerializeField] GameObject itemGrid;
        [SerializeField] GameObject stateManager;
        [SerializeField] GameObject itemPool;

        private ItemPool pool;
        private RoomGridManager roomGridManager;
        private ItemGridManager itemGridManager;
        private RoomStateManager roomStateManager;
        // private List<ItemCellGO> usedCells = new List<ItemCellGO>();

        public Item GetItemCell() {
            Item item = pool.GetOne();
            //cell.transform.SetParent(gridTabs.transform);
            //cell.Setup();
            GameObject cellGo = item.gameObject;
            cellGo.SetActive(true);
            return item;
        }

        public void ReleaseItemCell(Item item) {
            pool.ReleaseOne(item);
        }

        private void Awake() {
            VerifySerialisables();
            CreatePooling();
            InitComponents();
        }

        private ItemPool CreatePooling() {
            pool = itemPool.GetComponent<ItemPool>();
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
            if (itemGridManager == null) {
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
            itemGridManager = itemGrid.GetComponent<ItemGridManager>();
            itemGridManager.Setup(this);
            roomStateManager = stateManager.GetComponent<RoomStateManager>();
        }

    }

}
