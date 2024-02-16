using UnityEngine;
namespace RoomUI {
    public class ModalRoomManageRowPool : Pool<ModalRoomManagerRowGO> {

        [SerializeField] private PoolConfig config;

        public PoolConfig GetConfig() {
            return config;
        }

        public void Setup(GameObject prefab, int poolSize) {
            GameObject obj = Instantiate(prefab, transform);
            ModalRoomManagerRowGO rowComponent = obj.GetComponent<ModalRoomManagerRowGO>();
            obj.SetActive(false);
            base.Setup(rowComponent, poolSize);
        }
    }
}
