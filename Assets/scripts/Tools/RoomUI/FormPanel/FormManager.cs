using System;
using System.Collections.Generic;
using RoomNs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoomUI {
    public class FormManager : MonoBehaviour {
        [SerializeField] public Button save;
        [SerializeField] public Button delete;
        [SerializeField] public Button open;
        [SerializeField] public TMP_InputField displayName;
        [SerializeField] private TMP_Dropdown shapeDropdown;
        [SerializeField] private TMP_Dropdown difficultyDropdown;
        [SerializeField] private TMP_Dropdown biomeDropdown;
        [SerializeField] private GameObject stateManager;

        private RoomUIStateManager roomUIStateManager;
        private RoomUIFormValues roomUIFormValues;
        private string biome;
        private string difficulty;
        private string shape;

        private void Awake() {
            VerifySerialisables();
            roomUIStateManager = stateManager.GetComponent<RoomUIStateManager>();
            CreateDropdownList();
            CreateListeners();
        }

        private void VerifySerialisables() {
            Dictionary<string, object> serializableFields = new Dictionary<string, object> {
                { "roomUIStateManager", roomUIStateManager },
                { "displayName", displayName },
                { "shapeDropdown", shapeDropdown },
                { "difficultyDropdown", difficultyDropdown },
                { "biomeDropdown", biomeDropdown },
                { "stateManager", stateManager }
            };
            foreach (var field in serializableFields) {
                if (field.Value == null) {
                    Debug.LogError($"FormManager SerializeField {field.Key} not set !");
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
            options.Remove("ALL");
            dropdown.AddOptions(options);
        }

        private void CreateListeners() {
            save.onClick.AddListener(OnSaveClick);
            delete.onClick.AddListener(OnOpenClick);
            open.onClick.AddListener(OnDeleteClick);

            shapeDropdown.onValueChanged.AddListener(newValue => {
                string value = shapeDropdown.options[newValue].text;
                shape = value;
                roomUIStateManager.OnChangeShape(value);
            });
            difficultyDropdown.onValueChanged.AddListener(newValue => {
                string value = shapeDropdown.options[newValue].text;
                difficulty = value;
                roomUIStateManager.OnChangeDifficulty(value);
            });
            biomeDropdown.onValueChanged.AddListener(newValue => {
                string value = biomeDropdown.options[newValue].text;
                biome = value;
                roomUIStateManager.OnChangeBiome(value);
            });
        }

        private void OnSaveClick() {
            string name = displayName.text;
            if (name != null && shape != null && difficulty != null && biome != null) {
                roomUIFormValues = new RoomUIFormValues(displayName.text, shape, difficulty, biome);
                roomUIStateManager.OnClickSave(roomUIFormValues);
            }
            else {
                Debug.LogError("FormManager: Some fields are empty");
                roomUIStateManager.OnClickSave(null);
            }
        }

        private void OnOpenClick() {

        }

        private void OnDeleteClick() {

        }

        void OnDestroy() {
            save.onClick.RemoveAllListeners();
            delete.onClick.RemoveAllListeners();
            open.onClick.RemoveAllListeners();
            shapeDropdown.onValueChanged.RemoveAllListeners();
            difficultyDropdown.onValueChanged.RemoveAllListeners();
            biomeDropdown.onValueChanged.RemoveAllListeners();
        }

    }
}