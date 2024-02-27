namespace Modal {
    using UnityEngine;

    public class ModalManager : MonoBehaviour {
        public static ModalManager Instance { get; private set; }

        private string prefabPathModalConfig = $"{GlobalConfig.Instance.CommonModalPath}/Modal_Confirm";

        private void Awake() {
            Instance = this;
        }

        public ModalConfirm GetModalConfirm() {
            GameObject prefab = Resources.Load<GameObject>(prefabPathModalConfig);
            if (prefab != null) {
                GameObject modal = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                ModalConfirm modalComponent = modal.GetComponent<ModalConfirm>();
                return modalComponent;
            }
            Debug.LogError("RoomUIService(OpenConfirmationModal), no prefab at this path : " + prefabPathModalConfig);
            return null;
        }
    }

}