using System;
using System.Collections.Generic;
using RoomNs;
using TMPro;
using UnityEngine;

namespace RoomUI {
    public class FormManager: MonoBehaviour {

        [SerializeField] private TMP_Dropdown shapeDropdown;
        [SerializeField] private TMP_Dropdown difficultyDropdown;
        [SerializeField] private TMP_Dropdown biomeDropdown;
        [SerializeField] private GameObject stateManager;

        private RoomUIStateManager roomUIStateManager;

        private void Awake() {
            roomUIStateManager = stateManager.GetComponent<RoomUIStateManager>();
            CreateDropdownList();
            CreateListeners();
        }

        private void CreateDropdownList() {
            CreateDropdownList<DifficultyEnum>(difficultyDropdown);
            CreateDropdownList<BiomeEnum>(biomeDropdown);
            CreateDropdownList<RoomShapeEnum>(shapeDropdown);
        }

        private void CreateDropdownList<T>(TMP_Dropdown dropdown) where T : struct, Enum {
            List<string> options = new List<string>(Enum.GetNames(typeof(T)));
            dropdown.AddOptions(options);
        }

        private void CreateListeners() {
            shapeDropdown.onValueChanged.AddListener(newValue => {
                string value = shapeDropdown.options[newValue].text;
                roomUIStateManager.OnChangeShape(value);
            });
            difficultyDropdown.onValueChanged.AddListener(newValue => {
                string value = shapeDropdown.options[newValue].text;
                roomUIStateManager.OnChangeDifficulty(value);
            });
            biomeDropdown.onValueChanged.AddListener(newValue => {
                string value = biomeDropdown.options[newValue].text;
                roomUIStateManager.OnChangeBiome(value);
            });
        }

    }
}