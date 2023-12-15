using System;
using System.Collections.Generic;
using RoomNs;
using TMPro;
using UnityEngine;

public class FormManager: MonoBehaviour {

    [SerializeField] private TMP_Dropdown shapeDropdown;
    [SerializeField] private TMP_Dropdown difficultyDropdown;
    [SerializeField] private TMP_Dropdown biomeDropdown;
    [SerializeField] private GameObject stateManager;

    private RoomStateManager roomStateManager;

    private void Awake() {
        roomStateManager = stateManager.GetComponent<RoomStateManager>();
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
            RoomShapeEnum shape = GetRoomShapeEnumFromDropdownValue(value);
            roomStateManager.OnChangeShape(shape);
        });
        difficultyDropdown.onValueChanged.AddListener(newValue => {
            string value = shapeDropdown.options[newValue].text;
            DifficultyEnum difficulty = GetDifficultyEnumFromDropdownValue(value);
            roomStateManager.OnChangeDifficulty(difficulty);
        });
        biomeDropdown.onValueChanged.AddListener(newValue => {
            string value = biomeDropdown.options[newValue].text;
            BiomeEnum biome = GetBiomeEnumFromDropdownValue(value);
            roomStateManager.OnChangeBiome(biome);
        });
    }

    private RoomShapeEnum GetRoomShapeEnumFromDropdownValue(string value) {
        RoomShapeEnum shapeEnumValue;
        if (Enum.TryParse(value, out shapeEnumValue)) {
            return shapeEnumValue;
        }
        Debug.Log("Unkown Enum GetEnumFromDropdownValue: " + value);
        return RoomShapeEnum.R1X1;
    }

    private DifficultyEnum GetDifficultyEnumFromDropdownValue(string value) {
        DifficultyEnum difficultyEnumValue;
        if (Enum.TryParse(value, out difficultyEnumValue)) {
            return difficultyEnumValue;
        }
        Debug.Log("Unkown Enum GetEnumFromDropdownValue: " + value);
        return DifficultyEnum.DEFAULT;
    }

    private BiomeEnum GetBiomeEnumFromDropdownValue(string value) {
        BiomeEnum biomeEnumValue;
        if (Enum.TryParse(value, out biomeEnumValue)) {
            return biomeEnumValue;
        }
        Debug.Log("Unkown Enum GetEnumFromDropdownValue: " + value);
        return BiomeEnum.CAVE;
    }

}

