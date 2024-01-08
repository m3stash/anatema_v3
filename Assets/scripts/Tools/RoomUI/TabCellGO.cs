using UnityEngine;
using UnityEngine.UI;

public class TabCellGO : MonoBehaviour {

    public delegate void TabClickedEvent(ObjectType type);
    public event TabClickedEvent OnTabClicked;
    private ObjectType tabType;

    private void OnTabClick() {
        OnTabClicked?.Invoke(tabType);
    }

    private Button button;
    // private Color defaultColor;
    private Image backgroundImage;
    // private Transform defaultBackgroundTransform;
    private RectTransform backgroundRectTransform;

    private Image icon;
    [SerializeField] private GameObject backgroundGO;
    [SerializeField] private GameObject cellGO;

    public void DesactivateCell() {
        button.interactable = false;
    }

    /*public void AddWall() {
        image.color = Color.gray;
        button.interactable = false;
    }*/

    public void Setup(bool isActive, Sprite sprite, ObjectType objectType) {

        tabType = objectType;

        if (button == null) {
            backgroundImage = backgroundGO.GetComponent<Image>();
            // defaultBackgroundTransform = backgroundGO.transform;
            backgroundRectTransform = backgroundGO.GetComponent<RectTransform>();
            button = GetComponent<Button>();
            button.onClick.AddListener(OnTabClick);
            icon = cellGO.GetComponent<Image>();
            transform.localScale = Vector3Int.one;
            transform.localPosition = Vector3.one;
        } else {
            button.interactable = true;
        }

        Vector3 positon = backgroundGO.transform.position;

        if (!isActive) {
            backgroundGO.transform.position = new Vector3(positon.x, positon.y + 3, 0);
            float newHeight = backgroundRectTransform.sizeDelta.y - 4f;
            backgroundRectTransform.sizeDelta = new Vector2(backgroundRectTransform.sizeDelta.x, newHeight);
            var tempColor = backgroundImage.color;
            tempColor.a = 0.7f;
            backgroundImage.color = tempColor;
        } else {
            backgroundGO.transform.position = new Vector3(positon.x, positon.y - 1, 0);
        }

        icon.sprite = sprite;

    }

}

