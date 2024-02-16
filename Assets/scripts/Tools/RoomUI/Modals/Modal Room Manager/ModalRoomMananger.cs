using System;
using RoomNs;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModalRoomMananger : MonoBehaviour {

    [SerializeField] public TMP_InputField displayName;
    [SerializeField] public TMP_InputField id;
    [SerializeField] private TMP_Dropdown shapeDropdown;
    [SerializeField] private TMP_Dropdown biomeDropdown;
    [SerializeField] private TMP_Dropdown difficultyDropdown;
    [SerializeField] private GameObject gridLayoutGroup;
    [SerializeField] private GameObject buttonCloseGO;
    private GridLayoutGroup gridLayoutGroupComponent;
    private Button buttonClose;


    void Awake() {
        VerifySerialisables();
        InitComponents();
        CreateListeners();
        InitDropdowns();
    }

    private void VerifySerialisables() {
        Dictionary<string, object> serializableFields = new Dictionary<string, object> {
                { "displayName", displayName },
                { "id", id },
                { "shapeDropdown", shapeDropdown },
                { "biomeDropdown", biomeDropdown },
                { "difficultyDropdown", difficultyDropdown }
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
    }

    private void CreateListeners() {
        buttonClose.onClick.AddListener(() => {
            Debug.Log("coucou");
        });
    }

    private void CreateDropdownList<T>(TMP_Dropdown dropdown) where T : struct, Enum {
        List<string> options = new List<string>(Enum.GetNames(typeof(T)));
        dropdown.AddOptions(options);
    }

    void OnDestroy() {
        buttonClose.onClick.RemoveAllListeners();
    }
}
