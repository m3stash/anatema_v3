using System;
using System.Collections.Generic;
using RoomNs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Modal;

namespace RoomUI {
    public class FormManager : MonoBehaviour {
        [SerializeField] public Button save;
        [SerializeField] public Button delete;
        [SerializeField] public Button open;
        [SerializeField] public GameObject idGO;
        [SerializeField] public TMP_InputField displayName;
        [SerializeField] private TMP_Dropdown shapeDropdown;
        [SerializeField] private TMP_Dropdown difficultyDropdown;
        [SerializeField] private TMP_Dropdown biomeDropdown;
        [SerializeField] private GameObject stateManager;

        private RoomUIStateManager roomUIStateManager;
        private RoomUIFormValues roomUIFormValues;
        private TextMeshProUGUI idText;
        private string biome;
        private string difficulty;
        private string shape;
        private int id = -1;
        private bool canEmit = true;

        private void Awake() {
            InitComponents();
            VerifySerialisables();
            CreateDropdownList();
            CreateListeners();
        }

        private void InitComponents() {
            idText = idGO.GetComponent<TextMeshProUGUI>();
            idText.text = id.ToString();
            roomUIStateManager = stateManager.GetComponent<RoomUIStateManager>();
        }

        public RoomUIFormValues GetFormValues() {
            return roomUIFormValues;
        }

        public void SetRoomId(int newRoomId) {
            id = newRoomId;
            idText.text = newRoomId.ToString();
        }

        private void VerifySerialisables() {
            Dictionary<string, object> serializableFields = new Dictionary<string, object> {
                { "displayName", displayName },
                { "shapeDropdown", shapeDropdown },
                { "difficultyDropdown", difficultyDropdown },
                { "biomeDropdown", biomeDropdown },
                { "stateManager", stateManager },
                { "save", save },
                { "delete", delete },
                { "open", open },
                { "idGO", idGO }
            };
            foreach (var field in serializableFields) {
                if (field.Value == null) {
                    Debug.LogError($"FormManager script:  SerializeField {field.Key} not set !");
                }
            }
        }

        private void CreateDropdownList() {
            CreateDropdownList<DifficultyEnum>(difficultyDropdown);
            CreateDropdownList<BiomeEnum>(biomeDropdown);
            CreateDropdownList<RoomShapeEnum>(shapeDropdown);
        }

        private void CreateDropdownList<T>(TMP_Dropdown dropdown) where T : struct, Enum {
            List<string> options = new List<string>(Enum.GetNames(typeof(T)));
            options.Remove("ALL"); // toDo remove this when we have a better solution
            dropdown.AddOptions(options);
        }

        private void CreateListeners() {
            roomUIStateManager.OnRoomDelete += OnRoomDelete;
            roomUIStateManager.OnRoomLoad += OnRoomLoad;
            save.onClick.AddListener(OnSaveClick);
            delete.onClick.AddListener(() => ResetForm(true));
            open.onClick.AddListener(OnOpenClick);

            shapeDropdown.onValueChanged.AddListener(newValue => {
                if (newValue == -1) return;
                string value = shapeDropdown.options[newValue].text;
                shape = value;
                roomUIStateManager.OnChangeShape(value, canEmit);
            });
            difficultyDropdown.onValueChanged.AddListener(newValue => {
                if (newValue == -1) return;
                string value = difficultyDropdown.options[newValue].text;
                difficulty = value;
                roomUIStateManager.OnChangeDifficulty(value, canEmit);
            });
            biomeDropdown.onValueChanged.AddListener(newValue => {
                if (newValue == -1) return;
                string value = biomeDropdown.options[newValue].text;
                biome = value;
                roomUIStateManager.OnChangeBiome(value, canEmit);
            });
        }

        private void OnRoomLoad(RoomUIModel roomUIModel) {
            if (roomUIModel == null) {
                Debug.LogError("FormManager(OnCopyRoom): RoomUIModel is null copy not possible !");
                return;
            }
            canEmit = false;
            displayName.text = roomUIModel.Name;
            shapeDropdown.value = shapeDropdown.options.FindIndex(option => option.text == roomUIModel.Shape);
            difficultyDropdown.value = difficultyDropdown.options.FindIndex(option => option.text == roomUIModel.Difficulty);
            biomeDropdown.value = biomeDropdown.options.FindIndex(option => option.text == roomUIModel.Biome);
            id = roomUIModel.Id;
            idText.text = id.ToString();
            canEmit = true;
        }

        private void OnSaveClick() {
            string name = displayName.text;
            if (name != null && shape != null && difficulty != null && biome != null) {
                roomUIFormValues = new RoomUIFormValues(displayName.text, shape, difficulty, biome, id);
                roomUIStateManager.OnClickSave();
            }
            else {
                TooltipManager.Instance.CallTooltip(TooltipType.INFORMATION, "Some fields are empty !");
            }
        }

        private void OnOpenClick() {
            roomUIStateManager.OnClickOpen();
        }

        private void ResetForm(bool canEmit = false) {
            // TODO : AJOUTER la gestion de la touche escape (voir refactoriser tout car c'est pas terrible !!!)
            if (canEmit) {
                ModalConfirm modalConfirm = ModalManager.Instance.GetModalConfirm();
                if (modalConfirm != null) {
                    modalConfirm.Setup("the room will be destroyed and all unsaved changes will be lost, do you want to continue? ", () => OnCancelConfirm(modalConfirm), () => OnValideConfirm(modalConfirm));
                }
            }
            else {
                formReset();
            }
        }

        private void formReset() {
            idText.text = "-1";
            displayName.text = "";
            shapeDropdown.value = -1;
            difficultyDropdown.value = -1;
            biomeDropdown.value = -1;
        }

        private void OnRoomDelete(int id) {
            ResetForm(false);
        }

        private void OnCancelConfirm(ModalConfirm modalConfirm) {
            Destroy(modalConfirm.gameObject);
        }

        private void OnValideConfirm(ModalConfirm modalConfirm) {
            roomUIStateManager.OnResetRoom(-1);
            formReset();
            Destroy(modalConfirm.gameObject);
        }

        void OnDestroy() {
            roomUIStateManager.OnRoomLoad -= OnRoomLoad;
            save.onClick.RemoveAllListeners();
            delete.onClick.RemoveAllListeners();
            open.onClick.RemoveAllListeners();
            shapeDropdown.onValueChanged.RemoveAllListeners();
            difficultyDropdown.onValueChanged.RemoveAllListeners();
            biomeDropdown.onValueChanged.RemoveAllListeners();
        }

    }
}