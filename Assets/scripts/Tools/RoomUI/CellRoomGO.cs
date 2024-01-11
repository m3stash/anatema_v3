using UnityEngine;
using UnityEngine.UI;

public class CellRoomGO: MonoBehaviour {

    private Image image;
    private Button button;
    private Color defaultColor;
    private ObjectConfig config;
    [SerializeField] private GameObject cell;
    [SerializeField] private Sprite defaultIcon;
    private Image icon;
    private RectTransform rectTransform;
    private RectTransform childRectTransform;

    public delegate void CellClickEvent(CellRoomGO cellRoomGO);
    public static event CellClickEvent OnClick;

    private void OnCellClick() {
        OnClick?.Invoke(this);
    }

    public ObjectConfig GetConfig() {
        return config;
    }

    public void SetConfig(ObjectConfig config) {
        Debug.Log("CellRoomGO SetConfig");
        this.config = config;
        // toTest 
        Sprite cellIcon = config.GetSprite();
        icon.sprite = cellIcon;
    }

    void OnDestroy() {
        button.onClick.RemoveListener(OnCellClick);
    }

    public void DesactivateCell() {
        button.onClick.RemoveListener(OnCellClick);
        image.enabled = false;
        button.interactable = false;
        config = null;
        icon.sprite = null;
        icon.color = Color.white;
    }

    public void AddWall() {
        image.color = Color.gray;
        button.interactable = false;
        icon.sprite = null;
        cell.SetActive(false);
    }

    public void AddDoor() {
        image.color = Color.yellow;
        button.interactable = false;
        icon.sprite = null;
        cell.SetActive(false);
    }

    private void LateUpdate() {
        transform.localScale = Vector3Int.one;
        rectTransform = GetComponent<RectTransform>();
        float width = rectTransform.sizeDelta.x;
        float height = rectTransform.sizeDelta.y;
        childRectTransform = cell.GetComponent<RectTransform>();
        childRectTransform.sizeDelta = new Vector2(width * 0.75f, height * 0.75f);
    }


    public void Setup(ObjectConfig config) {
        // todo revoir la façon de faire !!!
        if (button == null || image == null) {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnCellClick);
            image = GetComponent<Image>();
            defaultColor = image.color;
            icon = cell.GetComponent<Image>();
        } else {
            cell.SetActive(true);
            image.enabled = true;
            button.interactable = true;
            image.color = defaultColor;
        }

        if (config == null) {
            // toDo voir quoi faire...
            // Debug.LogError("CellGO config cannot be null");
        } else {
            this.config = config;
            Sprite cellIcon = config.GetSprite();
            if (cellIcon) {
                icon.sprite = cellIcon;
            } else {
                icon.sprite = defaultIcon;
            }

        }

    }

}

