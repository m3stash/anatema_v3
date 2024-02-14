﻿using System.Collections.Generic;
using UnityEngine;

namespace RoomUI {
    public class RoomUIManager : MonoBehaviour {

        [SerializeField] GameObject roomGrid;
        [SerializeField] GameObject tabGrid;
        [SerializeField] GameObject stateManager;
        [SerializeField] private GameObject roomGridManagerGO;
        [SerializeField] private GameObject formManagerGO;
        [SerializeField] private RoomUIStateManager roomUIStateManager;

        private TabGridManager tabGridManager;
        private readonly string elementPath = "Sprites/elements/";
        private SpriteLoader spriteLoader;
        private ElementTableManager elementTableManager;
        private RoomUiTable roomUiTable;
        private RoomUIService roomUIService;
        private RoomGridManager roomGridManager;
        private FormManager formManager;


        private void Awake() {
            VerifySerialisables();
            InitDb();
            InitComponents();
            CreateListeners();
        }

        private void CreateListeners() {
            roomUIStateManager.OnSaveClick += SaveRoom;
        }

        private void OnDestroy() {
            roomUIStateManager.OnSaveClick -= SaveRoom;
        }

        private void SaveRoom() {
            RoomUIFormValues roomUIFormValues = formManager.GetFormValues();
            (List<GridElementModel> topLayer, List<GridElementModel> groundLayer) = roomGridManager.GetLayers();
            RoomUIModel roomUI = new RoomUIModel(roomUIFormValues.Name, roomUIFormValues.Shape, roomUIFormValues.Biome, roomUIFormValues.Difficulty, roomUIFormValues.Id, topLayer, groundLayer);
            int newRoomId = roomUIService.SaveRoom(roomUI);
            if (newRoomId != -1) {
                formManager.SetRoomId(newRoomId);
            }
            else {
                // ajouter une gestion de tooltip d'erreur !
                Debug.LogError($"RoomUIManager: Room {roomUIFormValues.Name} not saved !");
            }
        }

        private void VerifySerialisables() {
            Dictionary<string, object> serializableFields = new Dictionary<string, object> {
                { "roomGrid", roomGrid },
                { "tabGrid", tabGrid },
                { "stateManager", stateManager },
                { "roomGridManagerGO", roomGridManagerGO },
                { "formManagerGO", formManagerGO },
                { "roomUIStateManager", roomUIStateManager}
            };
            foreach (var field in serializableFields) {
                if (field.Value == null) {
                    Debug.LogError($"RoomUIMananger: SerializeField {field.Key} not set !");
                }
            }
        }

        private void InitComponents() {
            roomUIStateManager = stateManager.GetComponent<RoomUIStateManager>();
            tabGridManager = tabGrid.GetComponent<TabGridManager>();
            spriteLoader = new SpriteLoader(elementPath);
            tabGridManager.Setup(elementTableManager, spriteLoader);
            roomGridManager = roomGridManagerGO.GetComponent<RoomGridManager>();
            formManager = formManagerGO.GetComponent<FormManager>();
        }

        private void InitDb() {
            DatabaseManager dbManager = new DatabaseManager();
            elementTableManager = new ElementTableManager(dbManager);
            roomUiTable = new RoomUiTable(dbManager);
            roomUiTable.CreateTableRoom();
            roomUIService = new RoomUIService(dbManager);
            //MockDb();
        }

        public RoomUiTable GetRoomUiTable() {
            return roomUiTable;
        }

        private void MockDb() {
            // ElementTable.Insert(ElementCategoryType.ITEM.ToString());
            // ElementTable.Insert(ElementCategoryType.BLOCK.ToString());
            // int elementId = ElementTable.GetIdByType("ITEM");
            // Debug.Log($"ID for type ITEM: {elementId}");
            // CreateItemMock(elementId);
        }

        private void CreateItemMock(int elementId) {
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
