using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
namespace RoomUI {
    public class PanelItemInformationManager : MonoBehaviour {
        
        [SerializeField] private RoomUIStateManager roomUIStateManager;
        [SerializeField] private GameObject iconGO;
        [SerializeField] private GameObject nameGo;
        [SerializeField] private GameObject typeGo;
        [SerializeField] private GameObject descGo;
        [SerializeField] private GameObject categoryGO;
        private TextMeshProUGUI objectNameText;
        private TextMeshProUGUI typeText;
        private TextMeshProUGUI descriptionText;
        private TextMeshProUGUI categoryText;

        private Image icon; 

        void Awake() {
            if(roomUIStateManager == null) {
                roomUIStateManager = FindFirstObjectByType<RoomUIStateManager>();
            }
            SetComponents();
            CreateListeners();
        }

        private void CreateListeners() {
            roomUIStateManager.OnObjectSelected += OnObjectSelectedHandler;
        }

        private void SetComponents() {
            objectNameText = nameGo.GetComponent<TextMeshProUGUI>();
            typeText = typeGo.GetComponent<TextMeshProUGUI>();
            descriptionText = descGo.GetComponent<TextMeshProUGUI>();
            icon = iconGO.GetComponent<Image>();
            categoryText = categoryGO.GetComponent<TextMeshProUGUI>();
        }
        
        private void OnObjectSelectedHandler(Element selectedObject) {
            SetInformation(selectedObject);
        }

        private void SetInformation(Element selectedObject) {
            if(selectedObject == null) {
                objectNameText.text = "-";
                typeText.text = "-";
                descriptionText.text = "-";
                icon.sprite = null;
                categoryText.text = "-";
                return;
            }
            objectNameText.text = selectedObject.GetDisplayName();
            typeText.text = selectedObject.GetSubCategory();
            descriptionText.text = selectedObject.GetDescription();
            icon.sprite = selectedObject.GetSprite();
            categoryText.text = selectedObject.GetCategory();
        }

    }
}