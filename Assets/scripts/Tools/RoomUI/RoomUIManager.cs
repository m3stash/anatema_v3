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
        private DatabaseManager dbManager;
        private ItemTableManager itemTableManager;
        private ObjectTable objectTable;

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
            InitDb();
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
            objectsManager = itemGrid.GetComponent<ObjectsManager>();
            objectsManager.Setup(this);
            roomUIStateManager = stateManager.GetComponent<RoomUIStateManager>();
        }

        private void InitDb() {
            dbManager = new DatabaseManager();
            dbManager.Init();
            objectTable = new ObjectTable(dbManager);
            objectTable.CreateTable();
            itemTableManager = new ItemTableManager();
            itemTableManager.Setup(dbManager);
            MockDb();
        }

        private void MockDb(){
            // objectTable.Insert(ObjectType.ITEM.ToString());
            int objectId = objectTable.GetIdByType("ITEM");
            Debug.Log($"ID for type ITEM: {objectId}");
            CreateItemMock(objectId);
        }

        private void CreateItemMock(int objectId){
            int lastInsertedItemId = itemTableManager.GetItemTable().Insert(
                objectId, // int objectId,
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
            Debug.Log("------ "+lastInsertedItemId);

            if(lastInsertedItemId != -1){
                itemTableManager.GetPotionTable().Insert("HEALING", lastInsertedItemId);
                // toDO -> rajouter amout & cie !!!
            }
            // int lastInsertedItemId = itemTable.GetLastInsertedId();

        }

    }

}
