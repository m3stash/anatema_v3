using UnityEngine;

namespace RoomUI {
    public class RoomUIService : MonoBehaviour {

        private RoomUiTable roomUiTable;
        public string prefabPathModalRoomManager = $"{GlobalConfig.Instance.PrefabRoomUI}/modals/modalRoomManager/ModalRoomManagement";
        private ModalRoomManageRowPool pool;

        public void Setup(DatabaseManager dbManager) {
            roomUiTable = new RoomUiTable(dbManager);
            roomUiTable.CreateTableRoom();
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
                modalComponent.Setup(roomUiTable, pool);
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

    }

}