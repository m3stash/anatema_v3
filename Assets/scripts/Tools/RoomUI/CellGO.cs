using UnityEngine;
using UnityEngine.UI;

public class CellGO: MonoBehaviour {

    private Image image;
    private Button button;
    private Color defaultColor;
    private Element config;
    [SerializeField] private GameObject cell;
    [SerializeField] private Sprite defaultIcon;
    private Image icon;
    private RectTransform rectTransform;
    private RectTransform childRectTransform;

    public delegate void CellClickEvent(Element type);
    public static event CellClickEvent OnClick;

    private void OnCellClick() {
        OnClick?.Invoke(config);
    }

    void OnDestroy() {
        if(button != null){
            button.onClick.RemoveListener(OnCellClick);
        }
    }

    public void DesactivateCell() {
        image.enabled = false;
        button.interactable = false;
    }

    private void LateUpdate() {
        transform.localScale = Vector3Int.one;
        rectTransform = GetComponent<RectTransform>();
        float width = rectTransform.sizeDelta.x;
        float height = rectTransform.sizeDelta.y;
        childRectTransform = cell.GetComponent<RectTransform>();
        childRectTransform.sizeDelta = new Vector2(width * 0.75f, height * 0.75f);
    }

    public void Setup(Element config) {
        // todo revoir la façon de faire !!!
        if (button == null || image == null) {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnCellClick);
            image = GetComponent<Image>();
            defaultColor = image.color;
            icon = cell.GetComponent<Image>();
        } else {
            image.enabled = true;
            button.interactable = true;
            image.color = defaultColor;
        }

        if (config == null) {
            // toDo voir quoi faire...
            // Debug.LogError("CellGO config cannot be null");
        } else {
            this.config = config;
            Sprite cellIcon = config.Sprite;
            if (cellIcon) {
                icon.sprite = cellIcon;
            } else {
                icon.sprite = defaultIcon;
            }

        }

    }

}

