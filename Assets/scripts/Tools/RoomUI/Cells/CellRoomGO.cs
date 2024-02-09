using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class CellRoomGO: MonoBehaviour, IPointerEnterHandler {

    [SerializeField] private GameObject cell;
    [SerializeField] private GameObject background;
    [SerializeField] private Sprite defaultIcon;
    private RectTransform rectTransform;
    private RectTransform cellTransform;
    private RectTransform backgroundTransform;
    private bool isDoorOrWall;
    private Image image;
    private Button button;
    private Element config;
    private Vector2 spacing;
    private Vector2Int position;
    private Vector2 cellSize;
    private bool isDesactivatedCell;
    private CellRoomGO rootCellRoomGO;

    public delegate void CellClickEvent(CellRoomGO cellRoomGO);
    public static event CellClickEvent OnClick;

    public delegate void CellRoomGOEvent(CellRoomGO cellRoomGO);
    public static event CellRoomGOEvent OnPointerEnterEvent;

    public void OnPointerEnter(PointerEventData eventData) {
        OnPointerEnterEvent?.Invoke(this);
    }

    void Awake() {
        button = cell.GetComponent<Button>();
        button.onClick.AddListener(OnCellClick);
        image = cell.GetComponent<Image>();
        defaultIcon = image.sprite;
        rectTransform = GetComponent<RectTransform>();
        cellTransform = cell.GetComponent<RectTransform>();
        backgroundTransform = background.GetComponent<RectTransform>();
    }

    public Transform GetCellTransform() {
        return cellTransform.transform;
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

    public void DesactivateDisplay(){
        isDesactivatedCell = true;
        background.SetActive(false);
    }

    public void ActivateDisplay(){
        isDesactivatedCell = false;
        background.SetActive(true);
    }

    public void DesactivateCell() {
        DesactivateDisplay();
        ResetCell();
    }

    public void ActivateCell() {
        ActivateDisplay();
        ResetCell();
    }

    public void ResetPoolCell() {
        background.SetActive(true);
        button.interactable = true;
        isDoorOrWall = false;
        isDesactivatedCell = false;
        rootCellRoomGO = null;
        config = null;
        image.sprite = defaultIcon;
        image.color = Color.white;
        backgroundTransform.anchoredPosition = Vector2.zero;
        cellTransform.anchoredPosition = Vector2.zero;
    }

    public void ResetCell(){
        ResetPoolCell();
        ResizeCellSize();
    }

    public void ForbiddenAction() {
        button.interactable = false;
        button.interactable = true;
    }

    public void AddWall() {
        image.color = Color.gray;
        button.interactable = false;
        image.sprite = null;
        background.SetActive(false);
    }

    public void AddDoor() {
        image.color = Color.yellow;
        button.interactable = false;
        image.sprite = null;
        background.SetActive(false);
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
        cellTransform.sizeDelta = vSize;
        backgroundTransform.sizeDelta = vSize;
    }

    private void ChangeSpriteYPosition(int sizeY, float height){
        if(sizeY > 1){
            Vector2 currentPosition = cellTransform.anchoredPosition;
            currentPosition.y = -(height - cellSize.y);
            cellTransform.anchoredPosition = currentPosition;
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
        ActivateDisplay();
        if (config != null) {
            SetComponentValues(config);
        }
    }

    public CellRoomGO GetRootCellRoomGO() {
        if(rootCellRoomGO == null){
            return this;
        }
        return rootCellRoomGO;
    }

    public void SetupDesactivatedCell(CellRoomGO rootCellRoomGO, Element config) {
        this.rootCellRoomGO = rootCellRoomGO;
        this.config = config;
        DesactivateDisplay();
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

