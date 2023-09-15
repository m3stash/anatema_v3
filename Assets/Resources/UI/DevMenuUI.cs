using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DevMenuUI : MonoBehaviour {

    private DevMenu devMenu;
    [SerializeField] private GameObject biomePanel;
    [SerializeField] private GameObject buttonBiomePanelGo;
    private CanvasGroup biomePanelGroup;
    private Button buttonBiomePanel;

    private void Awake() {
        devMenu = new DevMenu();
        devMenu.Enable();
        Init();
    }

    private void Init() {
        biomePanelGroup = biomePanel.GetComponent<CanvasGroup>();
        buttonBiomePanel = buttonBiomePanelGo.GetComponent<Button>();
        buttonBiomePanel.onClick.AddListener(() => OpenNewBiomePanel());
        devMenu.Menu.Enable();
        SwitchPanel(biomePanelGroup, false);
    }

    private void OpenNewBiomePanel() {
        SwitchPanel(biomePanelGroup, true);
    }

    void Start() {
        //devMenu.Menu.GenerateNewBiomeFolders.performed += OnClickGenerateButton;
    }
    private void OnDestroy() {
        buttonBiomePanel.onClick.RemoveListener(() => OpenNewBiomePanel());
        devMenu.Menu.Disable();
    }

    private void SwitchPanel(CanvasGroup canvas, bool hideOrShow) {
        if (hideOrShow) {
            canvas.alpha = 1;
            canvas.interactable = true;
            canvas.blocksRaycasts = true;
        } else {
            canvas.alpha = 0;
            canvas.interactable = false;
            canvas.blocksRaycasts = false;
        }
        
    }

}
