using System.Collections.Generic;
using UnityEngine;
using Modal;

namespace RoomUI {
    public class RoomUIService : MonoBehaviour {

        [SerializeField] private GameObject modalManagerGO;

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
            SetRoomUIInput();
        }

        private void SetRoomUIInput() {
            roomUIInputManager = new RoomUIInputManager();
            roomUIInput = roomUIInputManager.GetRoomUIInput();
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
                roomUIStateManager.OnLoadRoom(roomUIModel);
            }
            else {
                Debug.LogError("RoomUIService(CopyRoom), roomUIModel is null with id : " + id);
            }

        }

        public void EditRoom(RoomUIModel partialModel) {
            int id = partialModel.Id;
            RoomUIModel roomUIModel = roomUiTable.GetRoomById(id);
            if (roomUIModel != null) {
                List<GridElementModel> topLayerElements = roomUIModel.TopLayer;
                List<int> idsList = new List<int>();
                topLayerElements.ForEach(element => {
                    idsList.Add(element.GetId());
                });
                List<Element> elements = elementTable.GetElementsByIds(idsList);
                topLayerElements.ForEach(element => {
                    int elementId = element.GetId();
                    int itemId = element.GetId();
                    Element elt = elements.Find(e => e.GetId() == elementId);
                    elt.SetSprite(spriteLoader.GetSprite(elt.GetCategory(), elt.GetSpriteName()));
                    if (elt != null) {
                        element.SetElement(elt);
                    }
                });
                roomUIStateManager.OnLoadRoom(roomUIModel);
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