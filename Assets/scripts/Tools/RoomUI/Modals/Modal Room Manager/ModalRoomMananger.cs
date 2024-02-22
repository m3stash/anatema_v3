using System;
using RoomNs;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static RoomUIInput;

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
        private Modal_RoomManangerActions modalAction;

        private ModalClosedCallback modalClosedCallback;

        void Awake() {
            VerifySerialisables();
            InitComponents();
            CreateListeners();
            InitDropdowns();
        }

        public void Setup(RoomUiTable table, ModalRoomManageRowPool pool, RoomUIService roomUIService, Modal_RoomManangerActions modalAction, ModalClosedCallback callback) {
            if (table == null || pool == null || roomUIService == null)
                throw new ArgumentNullException("ModalRoomMananger Setup, table, pool or roomUIService is null !");
            this.roomUIService = roomUIService;
            this.pool = pool;
            roomUiTable = table;
            this.modalAction = modalAction;
            modalAction.Close.performed += ctx => Close();
            modalClosedCallback = callback;
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
            Debug.Log("Display name : " + displayNameText + " shape : " + shape + " difficulty : " + difficulty + " biome : " + biome + " id : " + parsedId);
            // toDo : obliger  à remplir au moins le champ shape (ou autre pour limiter le nombre d'objets reçu ou lors prévoir une pagination..);
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
                    bool roomHasBeenDelete = roomUIService.DeleteRoom(room.Id);
                    if (roomHasBeenDelete) {
                        pool.ReleaseOne(modalRoomManagerRowGO);
                    }
                    else {
                        Debug.LogError("ModalRoomMananger: OnActionButtonClick, room not deleted !");
                    }
                    break;
                case "copy":
                    roomUIService.CopyRoom(room);
                    Close();
                    break;
                case "edit":
                    // toDo : ouvrir une modal pour éditer la room
                    break;
                default:
                    Debug.LogError("ModalRoomMananger: OnActionButtonClick, action not found !");
                    break;
            }
        }

        private void Close() {
            modalClosedCallback?.Invoke();
        }

        void OnDestroy() {
            modalAction.Close.performed -= ctx => Close();
            buttonClose.onClick.RemoveAllListeners();
            buttonSearch.onClick.RemoveAllListeners();
            ModalRoomManagerRowGO.OnButtonClick -= OnActionButtonClick;
            ResetPool();
        }
    }

}