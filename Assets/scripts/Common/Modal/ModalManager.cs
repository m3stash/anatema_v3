namespace Modal {
    using UnityEngine;

    public class ModalManager : MonoBehaviour {

        private string prefabPathModalConfig = $"{GlobalConfig.Instance.CommonModalPath}/Modal_Confirm";

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