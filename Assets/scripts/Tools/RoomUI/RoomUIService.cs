using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoomUI {
    public class RoomUIService : MonoBehaviour {

        private RoomUIStateManager roomUIStateManager;
        private RoomUiTable roomUiTable;
        public string prefabPathModalRoomManager = $"{GlobalConfig.Instance.PrefabRoomUI}/modals/modalRoomManager/ModalRoomManagement";
        private ModalRoomManageRowPool pool;
        private ElementTableManager elementTableManager;
        private SpriteLoader spriteLoader;

        public void Setup(DatabaseManager dbManager, RoomUIStateManager roomUIStateManager, ElementTableManager elementTableManager, SpriteLoader spriteLoader) {
            this.roomUIStateManager = roomUIStateManager;
            this.elementTableManager = elementTableManager;
            this.spriteLoader = spriteLoader;
            roomUiTable = new RoomUiTable(dbManager);
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
            ModalRoomMananger modalComponent = InstanciateRoomManagerModal(transform);
            if (modalComponent != null) {
                modalComponent.Setup(roomUiTable, pool, this);
            }
        }

        private ModalRoomMananger InstanciateRoomManagerModal(Transform transform) {
            GameObject prefab = Resources.Load<GameObject>(prefabPathModalRoomManager);
            if (prefab != null) {
                GameObject roomUIManagerModal = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                roomUIManagerModal.transform.SetParent(transform);
                roomUIManagerModal.transform.localPosition = Vector3.zero;
                roomUIManagerModal.transform.localScale = Vector3.one;
                ModalRoomMananger modalComponent = roomUIManagerModal.GetComponent<ModalRoomMananger>();
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
                List<Tuple<int, int>> idsList = new List<Tuple<int, int>>();
                List<GridElementModel> topLayerElements = roomUIModel.TopLayer;
                topLayerElements.ForEach(element => {
                    int elementId = element.GetElementId();
                    int id = element.GetId();
                    idsList.Add(new Tuple<int, int>(elementId, id));
                });
                List<Element> elements = elementTableManager.GetAllElementsByElementIdAndID(idsList);
                topLayerElements.ForEach(element => {
                    int elementId = element.GetElementId();
                    int itemId = element.GetId();
                    Element elementToCopy = elements.Find(e => e.GeElementId() == elementId);
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