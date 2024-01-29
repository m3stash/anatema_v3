using UnityEngine;

namespace RoomUI {
    public class RoomUIManager : MonoBehaviour {

        [SerializeField] GameObject roomGrid;
        [SerializeField] GameObject itemGrid;
        [SerializeField] GameObject stateManager;
        [SerializeField] GameObject itemPool;
        private ObjectPool pool;
        private RoomGridManager roomGridManager;
        private ElementManager elementManager;
        private RoomUIStateManager roomUIStateManager;
        private DatabaseManager dbManager;
        private ItemTableManager itemTableManager;
        private ElementTable elementTable;

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
            InitDb();
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
            if (roomGrid == null) {
                Debug.LogError("Error: SerializeField roomGrid not Set !");
            }
            if (itemGrid == null) {
                Debug.LogError("Error: SerializeField itemGridManager not Set !");
            }
            if (stateManager == null) {
                Debug.LogError("Error: SerializeField stateManager not Set !");
            }
            if (itemPool == null) {
                Debug.LogError("Error: SerializeField pool not Set !");
            }
        }

        private void InitComponents() {
            roomGridManager = roomGrid.GetComponent<RoomGridManager>();
            elementManager = itemGrid.GetComponent<ElementManager>();
            elementManager.Setup(this, elementTable, itemTableManager.GetItemTable());
            roomUIStateManager = stateManager.GetComponent<RoomUIStateManager>();
        }

        private void InitDb() {
            DatabaseManager dbManager = new DatabaseManager();
            elementTable = new ElementTable(dbManager);
            itemTableManager = new ItemTableManager(dbManager, elementTable.GetTableName()); 
            // int elementId = elementTable.GetIdByType("ITEM");
            // ItemTable itemTable = itemTableManager.GetItemTable();
            // itemTable.GetElementsByElementId(elementId);
            //MockDb();
        }

        private void MockDb(){
            // ElementTable.Insert(ElementCategoryType.ITEM.ToString());
            // ElementTable.Insert(ElementCategoryType.BLOCK.ToString());
            // int elementId = ElementTable.GetIdByType("ITEM");
            // Debug.Log($"ID for type ITEM: {elementId}");
            // CreateItemMock(elementId);
        }

        private void CreateItemMock(int elementId){
            /*int lastInsertedItemId = itemTableManager.GetItemTable().Insert(
                elementId, // int elementId,
                false, // bool dropables
                true, // bool consumable
                false, // bool craftable
                10, // int limit
                0f, // float weight
                1, // int sizeX
                1, // int sizeY
                "potion-estus-1", // string icon
                "POTION", // string category
                "Regenerates your health" // string description
            );
            if(lastInsertedItemId != -1){
                itemTableManager.GetPotionTable().Insert("HEALING", lastInsertedItemId);
                // toDO -> rajouter amout & cie !!!
            }*/
        }

    }

}
