using UnityEngine;
using UnityEngine.UI;

public class CellGO: MonoBehaviour {

    private Image image;
    private Button button;
    private Color defaultColor;
    private ObjectConfig config;
    [SerializeField] private GameObject cell;
    private Image icon;

    public void DesactivateCell() {
        image.enabled = false;
        button.interactable = false;
    }

    public void AddWall() {
        image.color = Color.gray;
        button.interactable = false;
    }

    public void AddDoor() {
        image.color = Color.yellow;
        button.interactable = false;
    }

    public void Setup(ObjectConfig config) {

        transform.localScale = Vector3Int.one;

        if (button == null || image == null) {
            button = GetComponent<Button>();
            image = GetComponent<Image>();
            defaultColor = image.color;
            icon = cell.GetComponent<Image>();
        } else {
            image.enabled = true;
            button.interactable = true;
            image.color = defaultColor;
        }

        this.config = config;

        if (config == null) {
            Debug.LogError("CellGO config cannot be null");
        } else {
            Sprite cellIcon = config.GetSprite();
            if (cellIcon) {
                icon.sprite = cellIcon;
            } else {
                // Debug.LogError("Icon sprite not set in config : " + config.GetName());
            }

        }

    }

}

