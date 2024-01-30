using System.Collections.Generic;
using UnityEngine;

namespace RoomUI {
    public class RoomUIManager : MonoBehaviour {

        [SerializeField] GameObject roomGrid;
        [SerializeField] GameObject itemGrid;
        [SerializeField] GameObject stateManager;
        private RoomGridManager roomGridManager;
        private ElementManager elementManager;
        private RoomUIStateManager roomUIStateManager;
        private DatabaseManager dbManager;
        private ItemTableManager itemTableManager;
        private ElementTable elementTable;
        private readonly string elementPath = "Sprites/elements/";
        private Dictionary<string, Sprite> spriteDictionary = new Dictionary<string, Sprite>();
        private SpriteLoader spriteLoader;

        private void Awake() {
            VerifySerialisables();
            InitDb();
            InitComponents(); 
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
        }

        private void InitComponents() {
            roomGridManager = roomGrid.GetComponent<RoomGridManager>();
            elementManager = itemGrid.GetComponent<ElementManager>();
            spriteLoader = new SpriteLoader(elementPath);
            elementManager.Setup(this, elementTable, itemTableManager.GetItemTable(), spriteLoader);
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
