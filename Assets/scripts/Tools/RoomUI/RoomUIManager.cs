﻿using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace RoomUI {
    public class RoomUIManager : MonoBehaviour {

        [SerializeField] GameObject roomGrid;
        [SerializeField] GameObject tabGrid;
        [SerializeField] GameObject stateManager;
        [SerializeField] private GameObject roomGridManagerGO;
        [SerializeField] private GameObject formManagerGO;
        [SerializeField] private RoomUIStateManager roomUIStateManager;
        [SerializeField] private GameObject modalRoomManageRowPoolGO;

        private TabGridManager tabGridManager;
        private readonly string elementPath = "Sprites/elements/";
        private SpriteLoader spriteLoader;
        private ElementTableManager elementTableManager;
        private RoomUiTable roomUiTable;
        private RoomUIService roomUIService;
        private RoomGridManager roomGridManager;
        private FormManager formManager;
        private ModalRoomManageRowPool modalRoomManageRowPool;
        private List<string> categories;
        private readonly string CATEGORY_STRING = "ITEM BLOCK ENTITY"; // todo changer ça plus tard...

        private void Awake() {
            VerifySerialisables();
            DatabaseManager dbManager = InitDb();
            spriteLoader = new SpriteLoader(elementPath);
            _ = LoadAllSpritesAsync();
            SetupRoomUIService(dbManager);
            InitComponents();
            CreateListeners();
            InitPool();
        }


        private void InitPool() {
            PoolConfig config = modalRoomManageRowPool.GetConfig();
            if (config != null) {
                modalRoomManageRowPool.Setup(config.GetPrefab(), config.GetPoolSize());
            }
            else {
                Debug.LogError("RoomUIManager: PoolConfig not set !");
            }
        }

        private void CreateListeners() {
            roomUIStateManager.OnSaveClick += SaveRoom;
            roomUIStateManager.OnOpenClick += OpenRoomManager;
        }

        private void OnDestroy() {
            roomUIStateManager.OnSaveClick -= SaveRoom;
            roomUIStateManager.OnOpenClick -= OpenRoomManager;
        }

        private void SaveRoom() {
            RoomUIFormValues roomUIFormValues = formManager.GetFormValues();
            (List<GridElementModel> topLayer, List<GridElementModel> bottomLayer, List<GridElementModel> middleLayer) = roomGridManager.GetLayers();
            RoomUIModel roomUI = new RoomUIModel(roomUIFormValues.Name, roomUIFormValues.Shape, roomUIFormValues.Biome, roomUIFormValues.Difficulty, roomUIFormValues.Id, topLayer, bottomLayer, middleLayer);
            int newRoomId = roomUIService.SaveRoom(roomUI);
            if (newRoomId != -1) {
                formManager.SetRoomId(newRoomId);
                TooltipManager.Instance.CallTooltip(TooltipType.SUCCESS, "Room saved successfully !");
            }
            else {
                TooltipManager.Instance.CallTooltip(TooltipType.ERROR, "Room save failed !");
            }
        }

        private void OpenRoomManager() {
            roomUIService.OpenRoomManager(transform, modalRoomManageRowPool);
        }

        private void VerifySerialisables() {
            Dictionary<string, object> serializableFields = new Dictionary<string, object> {
                { "roomGrid", roomGrid },
                { "tabGrid", tabGrid },
                { "stateManager", stateManager },
                { "roomGridManagerGO", roomGridManagerGO },
                { "formManagerGO", formManagerGO },
                { "roomUIStateManager", roomUIStateManager},
                { "modalRoomManageRowPoolGO", modalRoomManageRowPoolGO }
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
            tabGridManager.Setup(elementTableManager.GetElementTable(), categories, spriteLoader);
            roomGridManager = roomGridManagerGO.GetComponent<RoomGridManager>();
            formManager = formManagerGO.GetComponent<FormManager>();
            modalRoomManageRowPool = modalRoomManageRowPoolGO.GetComponent<ModalRoomManageRowPool>();
        }

        async Task LoadAllSpritesAsync() {
            if (categories == null) {
                Debug.LogError("RoomUIManager: categories is null !");
                return;
            }
            var tasks = new List<Task>();
            foreach (string category in categories) {
                tasks.Add(spriteLoader.LoadSpritesAsync(category));
            }
            await Task.WhenAll(tasks);
        }

        private DatabaseManager InitDb() {
            DatabaseManager dbManager = new DatabaseManager();
            elementTableManager = new ElementTableManager(dbManager);
            categories = new List<string>();
            categories.AddRange(CATEGORY_STRING.Split(' '));
            roomUiTable = new RoomUiTable(dbManager);
            roomUiTable.CreateTableRoom();
            //MockDb();
            return dbManager;
        }

        private void SetupRoomUIService(DatabaseManager dbManager) {
            roomUIService = gameObject.GetComponent<RoomUIService>();
            roomUIService.Setup(dbManager, roomUIStateManager, elementTableManager.GetElementTable(), spriteLoader);
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
