using System.Collections.Generic;
using UnityEngine;

namespace RoomUI {
    public class RoomUIService : MonoBehaviour {

        private RoomUIStateManager roomUIStateManager;
        private RoomUiTable roomUiTable;
        public string prefabPathModalRoomManager = $"{GlobalConfig.Instance.PrefabRoomUI}/modals/modalRoomManager/ModalRoomManagement";
        private ModalRoomManageRowPool pool;
        private ElementTable elementTable;
        private SpriteLoader spriteLoader;
        RoomUIInputManager roomUIInputManager;
        RoomUIInput roomUIInput;

        ModalRoomMananger modalComponent;

        public void Setup(DatabaseManager dbManager, RoomUIStateManager roomUIStateManager, ElementTable elementTable, SpriteLoader spriteLoader) {
            this.roomUIStateManager = roomUIStateManager;
            this.elementTable = elementTable;
            this.spriteLoader = spriteLoader;
            roomUiTable = new RoomUiTable(dbManager);
            roomUiTable.CreateTableRoom();
            SetRoomUIInput();
        }

        private void SetRoomUIInput() {
            roomUIInputManager = new RoomUIInputManager();
            roomUIInput = roomUIInputManager.GetRoomUIInput();
            roomUIInput.Modal_RoomMananger.Enable();
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
            ModalRoomMananger modalComponent = InstanciateRoomManagerModal(transform);
            if (modalComponent != null) {
                modalComponent.Setup(roomUiTable, pool, this, roomUIInput.Modal_RoomMananger, OnModalClosed);
            }
        }

        private void OnModalClosed() {
            Destroy(modalComponent.gameObject);
        }

        private ModalRoomMananger InstanciateRoomManagerModal(Transform transform) {
            GameObject prefab = Resources.Load<GameObject>(prefabPathModalRoomManager);
            if (prefab != null) {
                GameObject roomUIManagerModal = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                roomUIManagerModal.transform.SetParent(transform);
                roomUIManagerModal.transform.localPosition = Vector3.zero;
                roomUIManagerModal.transform.localScale = Vector3.one;
                modalComponent = roomUIManagerModal.GetComponent<ModalRoomMananger>();
                return modalComponent;
            }
            Debug.LogError("RoomUIService(OpenRoomManager), no prefab at this path : " + prefabPathModalRoomManager);
            return null;
        }

        public void CopyRoom(RoomUIModel partialModel) {
            int id = partialModel.Id;
            RoomUIModel roomUIModel = roomUiTable.GetRoomById(id);
            if (roomUIModel != null) {
                roomUIModel.Id = -1;
                roomUIModel.Name = "Copy of " + roomUIModel.Name;
                List<GridElementModel> topLayerElements = roomUIModel.TopLayer;
                List<int> idsList = new List<int>();
                topLayerElements.ForEach(element => {
                    idsList.Add(element.GetId());
                });
                List<Element> elements = elementTable.GetElementsByIds(idsList);
                topLayerElements.ForEach(element => {
                    int elementId = element.GetId();
                    int itemId = element.GetId();
                    Element elementToCopy = elements.Find(e => e.GetId() == elementId);
                    elementToCopy.SetSprite(spriteLoader.GetSprite(elementToCopy.GetCategory(), elementToCopy.GetSpriteName()));
                    if (elementToCopy != null) {
                        element.SetElement(elementToCopy);
                    }
                });
                roomUIStateManager.OnCopyRoom(roomUIModel);
            }
            else {
                Debug.LogError("RoomUIService(CopyRoom), roomUIModel is null with id : " + id);
            }

        }

        public bool DeleteRoom(int id) {
            // faire apparaitre une modale YES / NO
            return roomUiTable.Delete(id);
        }

    }

}