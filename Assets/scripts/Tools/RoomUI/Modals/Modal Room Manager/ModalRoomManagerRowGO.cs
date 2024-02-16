using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace RoomUI {
    public class ModalRoomManagerRowGO : MonoBehaviour {

        [SerializeField] private TMP_Text displayName;
        [SerializeField] private TMP_Text date;
        [SerializeField] private TMP_Text id;
        [SerializeField] private TMP_Text biome;
        [SerializeField] private TMP_Text difficulty;
        [SerializeField] private TMP_Text shape;
        [SerializeField] private Button delete;
        [SerializeField] private Button copy;
        [SerializeField] private Button edit;
        public delegate void ButtonClickEvent(RoomUIModel room, string action, ModalRoomManagerRowGO modalRoomManagerRowGO);
        public static event ButtonClickEvent OnButtonClick;

        private RoomUIModel roomUIModel;

        private void OnClick(string action) {
            OnButtonClick?.Invoke(roomUIModel, action, this);
        }

        public void Setup(RoomUIModel roomUIModel) {
            this.roomUIModel = roomUIModel;
            if (roomUIModel != null) {
                SetValue();
            }
            CreateListeners();
        }

        private void CreateListeners() {
            delete.onClick.AddListener(() => OnClick("delete"));
            copy.onClick.AddListener(() => OnClick("copy"));
            edit.onClick.AddListener(() => OnClick("edit"));
        }

        private void SetValue() {
            displayName.text = roomUIModel.Name;
            // date.text = roomUIModel.Date;
            id.text = roomUIModel.Id.ToString();
            biome.text = roomUIModel.Biome;
            difficulty.text = roomUIModel.Difficulty;
            shape.text = roomUIModel.Shape;
        }

        private void RemoveListeners() {
            delete.onClick.RemoveListener(() => OnClick("delete"));
            copy.onClick.RemoveListener(() => OnClick("copy"));
            edit.onClick.RemoveListener(() => OnClick("edit"));
        }

        private void OnDestroy() {
            RemoveListeners();
        }

    }
}
