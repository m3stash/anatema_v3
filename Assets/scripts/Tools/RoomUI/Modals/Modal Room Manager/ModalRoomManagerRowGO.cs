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

        public delegate void CellClickEvent(RoomUIModel room, string action);
        public static event CellClickEvent OnClick;

        private RoomUIModel roomUIModel;

        private void OnCellClick(string action) {
            OnClick?.Invoke(roomUIModel, action);
        }

        public void Setup(RoomUIModel roomUIModel) {
            this.roomUIModel = roomUIModel;
            if (roomUIModel != null) {
                SetValue();
            }
            CreateListeners();
        }

        private void CreateListeners() {
            delete.onClick.AddListener(() => OnCellClick("delete"));
            copy.onClick.AddListener(() => OnCellClick("copy"));
            edit.onClick.AddListener(() => OnCellClick("edit"));
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
            delete.onClick.RemoveListener(() => OnCellClick("delete"));
            copy.onClick.RemoveListener(() => OnCellClick("copy"));
            edit.onClick.RemoveListener(() => OnCellClick("edit"));
        }

        private void OnDestroy() {
            RemoveListeners();
        }

    }
}
