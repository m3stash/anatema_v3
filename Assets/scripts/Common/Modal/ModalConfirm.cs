
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Modal {

    public class ModalConfirm : MonoBehaviour {

        [SerializeField] private GameObject buttonCancelGO;
        [SerializeField] private GameObject buttonValidateGO;
        [SerializeField] private GameObject buttonCloseGO;
        [SerializeField] private GameObject textGO;

        public delegate void ConfirmCancelCallback();
        public delegate void ConfirmValidateCallback();

        private ConfirmCancelCallback cancelCallback;
        private ConfirmValidateCallback validateCallback;
        private Button buttonValidate;
        private Button buttonClose;
        private Button buttonCancel;
        private TextMeshProUGUI text;

        void Awake() {
            VerifySerialisables();
            CreateListeners();
        }

        public void Setup(string textContent, ConfirmCancelCallback cancelCallback, ConfirmValidateCallback validateCallback) {
            this.cancelCallback = cancelCallback;
            this.validateCallback = validateCallback;
            text = textGO.GetComponent<TextMeshProUGUI>();
            text.text = textContent;
        }

        private void VerifySerialisables() {
            Dictionary<string, object> serializableFields = new Dictionary<string, object> {
                    { "buttonCancelGO", buttonCancelGO },
                    { "buttonValidateGO", buttonValidateGO },
                    { "buttonCloseGO", buttonCloseGO },
                    { "textGO", textGO }
                };
            foreach (var field in serializableFields) {
                if (field.Value == null) {
                    Debug.LogError($"ModalConfirm SerializeField {field.Key} not set !");
                }
            }
        }


        private void CreateListeners() {
            buttonClose = buttonCloseGO.GetComponent<Button>();
            buttonCancel = buttonCancelGO.GetComponent<Button>();
            buttonValidate = buttonValidateGO.GetComponent<Button>();
            buttonClose.onClick.AddListener(Close);
            buttonCancel.onClick.AddListener(Close);
            buttonValidate.onClick.AddListener(Validate);

        }

        public void Close() {
            cancelCallback?.Invoke();
        }

        public void Validate() {
            validateCallback?.Invoke();
        }

        void OnDestroy() {
            buttonClose.onClick.RemoveAllListeners();
            buttonValidate.onClick.RemoveAllListeners();
            buttonCancel.onClick.RemoveAllListeners();
        }
    }

}