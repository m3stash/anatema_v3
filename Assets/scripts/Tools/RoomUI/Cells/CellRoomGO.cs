using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class CellRoomGO: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField] private GameObject cell;
    [SerializeField] private Sprite defaultIcon;
    private RectTransform rectTransform;
    private RectTransform childRectTransform;

    private bool isDoorOrWall;

    private Image image;
    private Button button;
    private Color defaultColor;
    private ColorBlock defaultButtonColor;
    private Element config;
    private float lastWith = 0;
    private float ratioCellSize = 1f;
    private Vector2 spacing;
    private Vector2Int position;
    private bool buttonColorHasCHanged;


    public delegate void CellClickEvent(CellRoomGO cellRoomGO);
    public static event CellClickEvent OnClick;

    public delegate void CellRoomGOEvent(CellRoomGO cellRoomGO);
    public static event CellRoomGOEvent OnPointerEnterEvent;
    public static event CellRoomGOEvent OnPointerExitEvent;

    public void OnPointerEnter(PointerEventData eventData) {
        OnPointerEnterEvent?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData) {
        if(buttonColorHasCHanged){
            button.colors = defaultButtonColor;
        }
        OnPointerExitEvent?.Invoke(this);
    }

    void Awake() {
        button = cell.GetComponent<Button>();
        button.onClick.AddListener(OnCellClick);
        defaultButtonColor = button.colors;
        image = cell.GetComponent<Image>();
        defaultColor = image.color;
        rectTransform = GetComponent<RectTransform>();
        childRectTransform = cell.GetComponent<RectTransform>();
    }

    private void OnCellClick() {
        OnClick?.Invoke(this);
    }

    public Element GetConfig() {
        return config;
    }

    public bool IsDoorOrWall() {
        return isDoorOrWall;
    }

    public void SetIsDoorOrWall(bool isDoorOrWall) {
        this.isDoorOrWall = isDoorOrWall;
    }

    private void SetComponentValues(Element config){
        this.config = config;
        Sprite cellIcon = config.GetSprite();
        image.sprite = cellIcon;
    }

    void OnDestroy() {
        button.onClick.RemoveListener(OnCellClick);
    }

    public void DesactivateCell() {
        image.enabled = false;
        button.interactable = false;
        ResetCell();
    }

    public void ActivateCell() {
        image.enabled = true;
        button.interactable = true;
        ResetCell();
    }

    public void AddWall() {
        image.color = Color.gray;
        button.interactable = false;
        image.sprite = null;
    }

    public void ForbidenAction() {
        button.interactable = false;
        button.interactable = true;
        SetHighlightedColor(Color.red);
    }

    private void SetHighlightedColor(Color highlightColor) {
        ColorBlock colors = button.colors;
        colors.highlightedColor = highlightColor;
        button.colors = colors;
    }

    public void AddDoor() {
        image.color = Color.yellow;
        button.interactable = false;
        image.sprite = null;
    }

    public void ResetCell(){
        config = null;
        image.sprite = null;
        image.color = Color.white;
        ResizeCellSize();
    }

    /*public void SetDefaultColor(){
        button.colors = defaultButtonColor;
    }*/

    private void ResizeCellSize() {
        float width;
        float height;
        float rectWidth = rectTransform.sizeDelta.x;
        float rectHeight = rectTransform.sizeDelta.y;
        width = rectWidth;
        height = rectHeight;
        Vector2Int size;
        if(config != null){
            size = config.GetSize();
            if(size.x > 1 || size.y > 1) {
                width = rectWidth  * size.x + (size.x - 1 * spacing.x);
                height = rectHeight  * size.y + (size.y - 1 * spacing.y);
            }
        }  
        childRectTransform.sizeDelta = new Vector2(width, height);
    }

    public Vector2Int GetPosition() {
        return position;
    }

    public void Setup(Element config, Vector2 spacing, Vector2Int position) {
        this.position = position;
        this.spacing = spacing;
        StartCoroutine(AdjustSizeAfterFrame());
        DefaultCellConfiguration();
        if (config != null) {
            SetComponentValues(config);
            // remove current selected param : for issue with selected white color > than highlight color
            button.interactable = false;
            button.interactable = true;
            SetHighlightedColor(Color.red);
            buttonColorHasCHanged = true;
        }
    }

    private void DefaultCellConfiguration(){
        cell.SetActive(true);
        image.enabled = true;
        button.interactable = true;
        image.color = defaultColor;
    }

    public void ResizeCellZiseAfterZoom(){
        StartCoroutine(AdjustSizeAfterFrame());
    }

    // used to solve the problem of recovering the size of the RectTransform, which is erroneous because it is driven by the gridLayout
    private IEnumerator AdjustSizeAfterFrame() {
        yield return new WaitForEndOfFrame();
        ResizeCellSize();
    }

}

