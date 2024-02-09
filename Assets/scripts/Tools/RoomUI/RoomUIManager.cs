using UnityEngine;

namespace RoomUI {
    public class RoomUIManager : MonoBehaviour {

        [SerializeField] GameObject roomGrid;
        [SerializeField] GameObject tabGrid;
        [SerializeField] GameObject stateManager;
        private TabGridManager tabGridManager;
        private readonly string elementPath = "Sprites/elements/";
        private SpriteLoader spriteLoader;

        private ElementTableManager elementTableManager;

        private void Awake() {
            VerifySerialisables();
            InitDb();
            InitComponents(); 
        }

        private void VerifySerialisables() {
            if (roomGrid == null) {
                Debug.LogError("Error: SerializeField roomGrid not Set !");
            }
            if (tabGrid == null) {
                Debug.LogError("Error: SerializeField itemGridManager not Set !");
            }
            if (stateManager == null) {
                Debug.LogError("Error: SerializeField stateManager not Set !");
            }
        }

        private void InitComponents() {
            tabGridManager = tabGrid.GetComponent<TabGridManager>();
            spriteLoader = new SpriteLoader(elementPath);
            tabGridManager.Setup(elementTableManager, spriteLoader);
        }

        private void InitDb() {
            DatabaseManager dbManager = new DatabaseManager();
            elementTableManager = new ElementTableManager(dbManager);
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
