using System;
using RoomNs;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Modal;
using DungeonNs;

namespace RoomUI {

    public delegate void ModalClosedCallback();

    public class ModalRoomMananger : MonoBehaviour {

        [SerializeField] public TMP_InputField displayName;
        [SerializeField] public TMP_InputField id;
        [SerializeField] private TMP_Dropdown shapeDropdown;
        [SerializeField] private TMP_Dropdown biomeDropdown;
        [SerializeField] private TMP_Dropdown difficultyDropdown;
        [SerializeField] private GameObject gridLayoutGroup;
        [SerializeField] private GameObject buttonCloseGO;
        [SerializeField] private GameObject buttonSearchGO;
        private GridLayoutGroup gridLayoutGroupComponent;
        private Button buttonClose;
        private Button buttonSearch;
        private RoomUiTable roomUiTable;
        private List<RoomUIModel> roomUIModels;
        private ModalRoomManageRowPool pool;
        private List<ModalRoomManagerRowGO> usedRows = new List<ModalRoomManagerRowGO>();
        private RoomUIService roomUIService;
        private ModalClosedCallback modalClosedCallback;
        private ModalConfirm modalConfirm;
        private ModalManager modalManager;
        private RoomUIInput.Modal_RoomManangerActions modal_RoomManangerActions;
        private RoomUIInput.Modal_ConfirmActions modal_ConfirmActions;

        void Awake() {
            VerifySerialisables();
            InitComponents();
            CreateListeners();
            InitDropdowns();
        }

        public void Setup(RoomUiTable table, ModalRoomManageRowPool pool, RoomUIService roomUIService, RoomUIInput roomUIInput, ModalClosedCallback callback, ModalManager modalManager) {
            if (table == null || pool == null || roomUIService == null)
                throw new ArgumentNullException("ModalRoomMananger Setup, table, pool or roomUIService is null !");
            ManageInput(roomUIInput);
            this.roomUIService = roomUIService;
            this.pool = pool;
            roomUiTable = table;
            modalClosedCallback = callback;
            this.modalManager = modalManager;
        }

        private void ManageInput(RoomUIInput roomUIInput) {
            modal_RoomManangerActions = roomUIInput.Modal_RoomMananger;
            modal_RoomManangerActions.Enable();
            modal_RoomManangerActions.Close.performed += ctx => Close();
            modal_ConfirmActions = roomUIInput.Modal_Confirm;
            modal_ConfirmActions.Disable();
            modal_ConfirmActions.Close.performed += ctx => CloseConfirmation();
        }

        private void VerifySerialisables() {
            Dictionary<string, object> serializableFields = new Dictionary<string, object> {
                    { "displayName", displayName },
                    { "id", id },
                    { "shapeDropdown", shapeDropdown },
                    { "biomeDropdown", biomeDropdown },
                    { "difficultyDropdown", difficultyDropdown },
                    { "gridLayoutGroup", gridLayoutGroup },
                    { "buttonCloseGO", buttonCloseGO },
                    { "buttonSearchGO", buttonSearchGO }
                };
            foreach (var field in serializableFields) {
                if (field.Value == null) {
                    Debug.LogError($"ModalRoomMananger SerializeField {field.Key} not set !");
                }
            }
        }

        private void InitDropdowns() {
            CreateDropdownList<DifficultyEnum>(difficultyDropdown);
            CreateDropdownList<BiomeEnum>(biomeDropdown);
            CreateDropdownList<RoomShapeEnum>(shapeDropdown);
        }

        private void InitComponents() {
            gridLayoutGroupComponent = gridLayoutGroup.GetComponent<GridLayoutGroup>();
            if (gridLayoutGroupComponent == null) {
                Debug.LogError("ModalRoomMananger gridLayoutGroupComponent not set !");
            }
            buttonClose = buttonCloseGO.GetComponent<Button>();
            buttonSearch = buttonSearchGO.GetComponent<Button>();
        }

        private void CreateListeners() {
            ModalRoomManagerRowGO.OnButtonClick += OnActionButtonClick;
            buttonClose.onClick.AddListener(() => {
                Close();
            });
            buttonSearch.onClick.AddListener(() => {
                Search();
            });
        }

        private void Search() {
            string displayNameText = displayName.text;
            string shape = GetDropdownSelectedText(shapeDropdown);
            string difficulty = GetDropdownSelectedText(difficultyDropdown);
            string biome = GetDropdownSelectedText(biomeDropdown);
            int? parsedId = ParseID(id.text);
            roomUIModels = roomUiTable.SearchRoomsByParams(parsedId, displayNameText, shape, difficulty, biome);
            RefreshTable(roomUIModels);
        }

        private void ResetPool() {
            if (usedRows.Count > 0) {
                pool.ReleaseMany(usedRows);
                usedRows.Clear();
            }
        }

        private void RefreshTable(List<RoomUIModel> roomUIModels) {
            ResetPool();
            foreach (var room in roomUIModels) {
                ModalRoomManagerRowGO row = pool.GetOne();
                row.Setup(room);
                usedRows.Add(row);
                row.transform.SetParent(gridLayoutGroup.transform);
                row.transform.localScale = Vector3.one;
                row.gameObject.SetActive(true);
            }
        }

        private int? ParseID(string idText) {
            int? parsedId = null;
            if (!string.IsNullOrEmpty(idText) && int.TryParse(idText, out int parsedIdValue)) {
                parsedId = parsedIdValue;
                return parsedId;
            }
            return null;
        }

        private string GetDropdownSelectedText(TMP_Dropdown dropdown) {
            if (dropdown.value != -1 && dropdown.value < dropdown.options.Count) {
                return dropdown.options[dropdown.value].text;
            }
            else {
                return "";
            }
        }

        private void CreateDropdownList<T>(TMP_Dropdown dropdown) where T : struct, Enum {
            List<string> options = new List<string>(Enum.GetNames(typeof(T)));
            dropdown.AddOptions(options);
        }

        private void OnActionButtonClick(RoomUIModel room, string action, ModalRoomManagerRowGO modalRoomManagerRowGO) {
            switch (action) {
                case "delete":
                    OpenConfirmationModalDelete(room, modalRoomManagerRowGO);
                    break;
                case "copy":
                    OpenConfirmationModalCopy(room);
                    break;
                case "edit":
                    OpenConfirmationModalEdit(room);
                    break;
                default:
                    Debug.LogError("ModalRoomMananger: OnActionButtonClick, action not found !");
                    break;
            }
        }



        public void CloseConfirmation() {
            modal_RoomManangerActions.Enable();
            modal_ConfirmActions.Disable();
            Destroy(modalConfirm.gameObject);
        }

        public void OpenConfirmationModalDelete(RoomUIModel room, ModalRoomManagerRowGO modalRoomManagerRowGO) {
            modalConfirm = modalManager.GetModalConfirm();
            if (modalConfirm != null) {
                modalConfirm.Setup("Are you sure you want to delete this room ?", CloseConfirmation, () => OnConfirm_ValidateDeleteRoomUI(room, modalRoomManagerRowGO));
            }
        }

        public void OpenConfirmationModalEdit(RoomUIModel room) {
            modal_RoomManangerActions.Disable();
            modal_ConfirmActions.Enable();
            modalConfirm = modalManager.GetModalConfirm();
            if (modalConfirm != null) {
                modalConfirm.Setup("Do you want to edit this room ?", CloseConfirmation, () => OnConfirm_ValidateEditRoomUI(room));
            }
        }

        public void OpenConfirmationModalCopy(RoomUIModel room) {
            modal_RoomManangerActions.Disable();
            modal_ConfirmActions.Enable();
            modalConfirm = modalManager.GetModalConfirm();
            if (modalConfirm != null) {
                modalConfirm.Setup("Do you want to create a new room from a copy of this one?", CloseConfirmation, () => OnConfirm_ValidateCopyRoomUI(room));
            }
        }

        public void OnConfirm_ValidateDeleteRoomUI(RoomUIModel room, ModalRoomManagerRowGO modalRoomManagerRowGO) {
            bool roomHasBeenDelete = roomUIService.DeleteRoom(room.Id);
            if (roomHasBeenDelete) {
                CloseConfirmation();
                pool.ReleaseOne(modalRoomManagerRowGO);
            }
        }

        public void OnConfirm_ValidateEditRoomUI(RoomUIModel room) {
            CloseConfirmation();
            Close();
            roomUIService.EditRoom(room);
        }

        public void OnConfirm_ValidateCopyRoomUI(RoomUIModel room) {
            CloseConfirmation();
            Close();
            roomUIService.CopyRoom(room);
        }

        private void Close() {
            modalClosedCallback?.Invoke();
        }

        void OnDestroy() {
            modal_RoomManangerActions.Disable();
            modal_ConfirmActions.Disable();
            modal_RoomManangerActions.Close.performed -= ctx => Close();
            modal_ConfirmActions.Close.performed -= ctx => Close();
            buttonClose.onClick.RemoveAllListeners();
            buttonSearch.onClick.RemoveAllListeners();
            ModalRoomManagerRowGO.OnButtonClick -= OnActionButtonClick;
            ResetPool();
        }
    }

}