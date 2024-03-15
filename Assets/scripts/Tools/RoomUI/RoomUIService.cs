using System.Collections.Generic;
using UnityEngine;
using Modal;

namespace RoomUI {
    public class RoomUIService : MonoBehaviour {

        [SerializeField] private GameObject modalManagerGO;
        [SerializeField] private GameObject roomUIInputManagerGO;

        private RoomUIStateManager roomUIStateManager;
        private RoomUiTable roomUiTable;
        private readonly string prefabPathModalRoomManager = $"{GlobalConfig.Instance.PrefabRoomUI}/modals/modalRoomManager/ModalRoomManagement";
        private ModalRoomManageRowPool pool;
        private ElementTable elementTable;
        private SpriteLoader spriteLoader;
        private RoomUIInputManager roomUIInputManager;
        private RoomUIInput roomUIInput;
        private ModalRoomMananger modalRoomManager;
        private ModalManager modalManager;

        private void Awake() {
            if (roomUIInputManagerGO != null) {
                roomUIInputManager = roomUIInputManagerGO.GetComponent<RoomUIInputManager>();
                roomUIInput = roomUIInputManager.GetRoomUIInput();
            }
            else {
                Debug.LogError("RoomUIService(Awake), roomUIInputManager is null");
            }
        }
        public void Setup(DatabaseManager dbManager, RoomUIStateManager roomUIStateManager, ElementTable elementTable, SpriteLoader spriteLoader) {
            this.roomUIStateManager = roomUIStateManager;
            this.elementTable = elementTable;
            this.spriteLoader = spriteLoader;
            roomUiTable = new RoomUiTable(dbManager);
            if (modalManagerGO != null) {
                modalManager = modalManagerGO.GetComponent<ModalManager>();
            }
            else {
                Debug.LogError("RoomUIService(Setup), modalManager is null");
            }
            roomUiTable.CreateTableRoom();
        }

        public SpriteLoader GetSpriteLoader() {
            return spriteLoader;
        }

        public int SaveRoom(RoomUIModel roomUi) {
            int roomId = roomUi.Id;
            if (roomId == -1) {
                return roomUiTable.Insert(roomUi);
            }
            return roomUiTable.Update(roomUi);
        }

        public void OpenRoomManager(Transform transform, ModalRoomManageRowPool pool) {
            if (pool != null) {
                this.pool = pool;
            }
            else {
                Debug.LogError("RoomUIService(OpenRoomManager), pool is null");
            }
            modalRoomManager = InstanciateRoomManagerModal(transform);
            if (modalRoomManager != null) {
                modalRoomManager.Setup(roomUiTable, pool, this, roomUIInput, OnModalClosed, modalManager);
            }
        }

        private void OnModalClosed() {
            Destroy(modalRoomManager.gameObject);
        }

        private ModalRoomMananger InstanciateRoomManagerModal(Transform transform) {
            GameObject prefab = Resources.Load<GameObject>(prefabPathModalRoomManager);
            if (prefab != null) {
                GameObject roomUIManagerModal = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                roomUIManagerModal.transform.SetParent(transform);
                roomUIManagerModal.transform.localPosition = Vector3.zero;
                roomUIManagerModal.transform.localScale = Vector3.one;
                return roomUIManagerModal.GetComponent<ModalRoomMananger>();
            }
            Debug.LogError("RoomUIService(OpenRoomManager), no prefab at this path : " + prefabPathModalRoomManager);
            return null;
        }

        public void CopyRoom(RoomUIModel partialModel) {
            EditRoom(partialModel, true);
        }

        public void EditRoom(RoomUIModel partialModel, bool isCopy = false) {
            int id = partialModel.Id;
            RoomUIModel roomUIModel = roomUiTable.GetRoomById(id);
            if (roomUIModel != null) {
                List<int> idList = new List<int>();
                AddIdInList(roomUIModel.TopLayer, idList);
                AddIdInList(roomUIModel.MiddleLayer, idList);
                AddIdInList(roomUIModel.BottomLayer, idList);
                List<Element> elements = elementTable.GetElementsByIds(idList);
                CreateElementFromId(roomUIModel.TopLayer, elements);
                CreateElementFromId(roomUIModel.MiddleLayer, elements);
                CreateElementFromId(roomUIModel.BottomLayer, elements);
                if (isCopy) {
                    roomUIModel.Id = -1;
                    roomUIModel.Name = "Copy of " + roomUIModel.Name;
                }
                roomUIStateManager.OnLoadRoom(roomUIModel);
            }
            else {
                TooltipManager.Instance.CallTooltip(TooltipType.ERROR, "roomUIModel is null with id: " + id);
            }
        }

        private void AddIdInList(List<GridElementModel> eltlayer, List<int> idList) {
            if (eltlayer == null) return;
            eltlayer.ForEach(element => {
                if (!idList.Contains(element.GetId())) {
                    idList.Add(element.GetId());
                }
            });
        }

        private void CreateElementFromId(List<GridElementModel> eltslayer, List<Element> elements) {
            if (eltslayer == null) return;
            eltslayer.ForEach(element => {
                int elementId = element.GetId();
                int itemId = element.GetId();
                Element elt = elements.Find(e => e.GetId() == elementId);
                elt.SetSprite(spriteLoader.GetSprite(elt.GetCategory(), elt.GetSpriteName()));
                if (elt != null) {
                    element.SetElement(elt);
                }
            });
        }

        public bool DeleteRoom(int id) {
            bool roomIsDeleted = roomUiTable.Delete(id);
            if (roomIsDeleted) {
                TooltipManager.Instance.CallTooltip(TooltipType.SUCCESS, "Room deleted successfully !");
                roomUIStateManager.OnDeleteRoom(id);
                return true;
            }
            TooltipManager.Instance.CallTooltip(TooltipType.ERROR, "An error has been encountered while deleting the room !");
            return false;
        }

    }

}