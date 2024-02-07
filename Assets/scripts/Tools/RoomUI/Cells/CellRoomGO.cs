using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.AI;

public class CellRoomGO: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField] private GameObject cell;
    [SerializeField] private GameObject background;
    [SerializeField] private Sprite defaultIcon;
    private RectTransform rectTransform;
    private RectTransform childRectTransform;
    private RectTransform backgroundTransform;

    private bool isDoorOrWall;

    private Image image;
    private Button button;
    private Element config;
    private float lastWith = 0;
    private float ratioCellSize = 1f;
    private Vector2 spacing;
    private Vector2Int position;
    private Vector2 cellSize;
    private bool isDesactivatedCell;

    public delegate void CellClickEvent(CellRoomGO cellRoomGO);
    public static event CellClickEvent OnClick;

    public delegate void CellRoomGOEvent(CellRoomGO cellRoomGO);
    public static event CellRoomGOEvent OnPointerEnterEvent;
    public static event CellRoomGOEvent OnPointerExitEvent;

    public bool isRootCell;

    public void OnPointerEnter(PointerEventData eventData) {
        OnPointerEnterEvent?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData) {
        OnPointerExitEvent?.Invoke(this);
    }

    void Awake() {
        button = cell.GetComponent<Button>();
        button.onClick.AddListener(OnCellClick);
        image = cell.GetComponent<Image>();
        defaultIcon = image.sprite;
        rectTransform = GetComponent<RectTransform>();
        childRectTransform = cell.GetComponent<RectTransform>();
        backgroundTransform = background.GetComponent<RectTransform>();
    }


    public bool IsDesactivatedCell() {
        return isDesactivatedCell;
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
        isDesactivatedCell = true;
        // image.enabled = false;
        image.sprite = defaultIcon;
        button.interactable = false;
        background.SetActive(false);
        ResetCell();
    }

    public void ActivateCell() {
        isDesactivatedCell = false;
        // image.enabled = true;
        button.interactable = true;
        background.SetActive(true);
        ResetCell();
    }

    public void AddWall() {
        image.color = Color.gray;
        button.interactable = false;
        image.sprite = null;
        background.SetActive(false);
    }

    public void ForbiddenAction() {
        button.interactable = false;
        button.interactable = true;
    }

    public void AddDoor() {
        image.color = Color.yellow;
        button.interactable = false;
        image.sprite = null;
        background.SetActive(false);
    }

    public void ResetCell(){
        config = null;
        // image.sprite = null;
        image.color = Color.white;
        ResizeCellSize();
    }

    private void ResizeCellSize() {
        float width;
        float height;
        float rectWidth = rectTransform.sizeDelta.x;
        float rectHeight = rectTransform.sizeDelta.y;
        cellSize = new Vector2(rectWidth, rectHeight);
        width = rectWidth;
        height = rectHeight;
        Vector2Int size;
        if(config != null){
            size = config.GetSize();
            if(size.x > 1 || size.y > 1) {
                width = rectWidth  * size.x + (size.x - 1 * spacing.x);
                height = rectHeight  * size.y + (size.y - 1 * spacing.y);
            }
            ChangeSpriteYPosition(size.y, height);
        }
        Vector2 vSize = new Vector2(width, height);
        childRectTransform.sizeDelta = vSize;
        backgroundTransform.sizeDelta = vSize;
    }

    private void ChangeSpriteYPosition(int posY, float height){
        if(posY > 1){
            Vector2 currentPosition = childRectTransform.anchoredPosition;
            currentPosition.y = -(height - cellSize.y);
            childRectTransform.anchoredPosition = currentPosition;
            backgroundTransform.anchoredPosition = currentPosition;
        }
    }

    public Vector2 GetCellSize(){
        return cellSize;
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
        }
    }

    private void DefaultCellConfiguration(){
        background.SetActive(true);
        //image.enabled = true;
        cell.SetActive(true);
        button.interactable = true;
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

