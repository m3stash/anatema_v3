using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using RoomUI;

public class CellRoomGO : MonoBehaviour, IPointerEnterHandler {

    [SerializeField] private GameObject cellBottom;
    [SerializeField] private GameObject cellMiddle;
    [SerializeField] private GameObject cellTop;
    [SerializeField] private Sprite defaultIcon;
    [SerializeField] private Sprite transparentIcon;
    private RectTransform rectTransform;
    private RectTransform cellMiddleTransform;
    private RectTransform cellTopTransform;
    private RectTransform cellBottomTransform;
    private bool isDoorOrWall;
    private Image imageCell;
    private Image imageTop;
    private Image imageMiddle;
    private Image imageBottom;
    private Button button;
    private Element configTopLayer;
    private Element configMiddleLayer;
    private Element configBottomLayer;
    private Vector2 spacing;
    private Vector2Int position;
    private Vector2 cellSize;
    private bool cellDesactivated;
    private bool isLayerTopDesactivated;
    private bool isLayerMiddleDesactivated;
    private bool isLayerBottomDesactivated;
    private CellRoomGO rootTopCellRoomGO;
    private CellRoomGO rootMiddleCellRoomGO;
    private CellRoomGO rootBottomCellRoomGO;
    private Color32 defaultColorCell = new Color32(200, 200, 200, 255);
    private Color32 doorColor = new Color32(125, 125, 45, 255);
    private Color32 wallColor = new Color32(50, 50, 50, 255);

    public delegate void CellClickEvent(CellRoomGO cellRoomGO);
    public static event CellClickEvent OnClick;

    public delegate void CellRoomGOEvent(CellRoomGO cellRoomGO);
    public static event CellRoomGOEvent OnPointerEnterEvent;

    public void OnPointerEnter(PointerEventData eventData) {
        OnPointerEnterEvent?.Invoke(this);
    }

    void Awake() {
        // issue with localscale on 1080p resolution after instantiate go
        transform.localScale = Vector3Int.one;
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(OnCellClick);
        rectTransform = GetComponent<RectTransform>();
        cellMiddleTransform = cellMiddle.GetComponent<RectTransform>();
        cellTopTransform = cellTop.GetComponent<RectTransform>();
        cellBottomTransform = cellBottom.GetComponent<RectTransform>();
        imageTop = cellTop.GetComponent<Image>();
        imageMiddle = cellMiddle.GetComponent<Image>();
        imageBottom = cellBottom.GetComponent<Image>();
        imageCell = GetComponent<Image>();
        imageCell.color = defaultColorCell;
        // backgroundTransform = background.GetComponent<RectTransform>();
    }

    // public Transform GetCellTransform() {
    //     return cellTransform.transform;
    // }

    public bool IsDesactivatedCell(LayerType layerType) {
        return cellDesactivated;
    }

    private void OnCellClick() {
        OnClick?.Invoke(this);
    }

    public bool IsDoorOrWall() {
        return isDoorOrWall;
    }

    public void SetIsDoorOrWall(bool isDoorOrWall) {
        this.isDoorOrWall = isDoorOrWall;
    }

    private void SetComponentValues(Element config, LayerType layerType) {
        if (layerType == LayerType.TOP) {
            configTopLayer = config;
            imageTop.sprite = configTopLayer.GetSprite();
        }
        else if (layerType == LayerType.MIDDLE) {
            configMiddleLayer = config;
            imageMiddle.sprite = configMiddleLayer.GetSprite();
        }
        else if (layerType == LayerType.BOTTOM) {
            configBottomLayer = config;
            imageBottom.sprite = configBottomLayer.GetSprite();
        }
        imageCell.sprite = transparentIcon;
    }

    void OnDestroy() {
        button.onClick.RemoveListener(OnCellClick);
    }

    public void DesactivateDisplay() {
        imageCell.sprite = transparentIcon;
        // background.SetActive(false);
    }

    public void ActivateDisplay() {
        imageCell.sprite = null;
        // background.SetActive(true);
    }

    public void DesactivateCell() {
        cellDesactivated = true;
        DesactivateDisplay();
        ResetCell(LayerType.ALL);
    }

    public Element GetConfig(LayerType layerType) {
        if (layerType == LayerType.TOP) {
            return configTopLayer;
        }
        else if (layerType == LayerType.MIDDLE) {
            return configMiddleLayer;
        }
        else if (layerType == LayerType.BOTTOM) {
            return configBottomLayer;
        }
        return null;
    }

    public bool IsLayersEmpty() {
        return configTopLayer == null && configBottomLayer == null && configMiddleLayer == null;
    }

    public void ResetPoolCell() {
        button.interactable = true;
        isDoorOrWall = false;
        cellDesactivated = false;
        imageCell.color = defaultColorCell;
        imageCell.sprite = null;
        // backgroundTransform.anchoredPosition = Vector2.zero;
    }

    public void ResetCell(LayerType layerType) {
        if (layerType == LayerType.TOP || layerType == LayerType.ALL) {
            rootTopCellRoomGO = null;
            configTopLayer = null;
            imageTop.sprite = transparentIcon;
            cellTopTransform.anchoredPosition = Vector2.zero;
            ResizeCellSize();
        }
        if (layerType == LayerType.MIDDLE || layerType == LayerType.ALL) {
            rootMiddleCellRoomGO = null;
            configMiddleLayer = null;
            imageMiddle.sprite = transparentIcon;
            cellMiddleTransform.anchoredPosition = Vector2.zero;
            ResizeCellSize();
        }
        if (layerType == LayerType.BOTTOM || layerType == LayerType.ALL) {
            rootBottomCellRoomGO = null;
            configBottomLayer = null;
            imageBottom.sprite = transparentIcon;
            cellBottomTransform.anchoredPosition = Vector2.zero;
            ResizeCellSize();
        }
        // TODO ca va pas cette merde
        if (configTopLayer == null && configMiddleLayer == null && configBottomLayer == null || layerType == LayerType.ALL) {
            ResetPoolCell();
        }
    }

    public void ForbiddenAction() {
        button.interactable = false;
        button.interactable = true;
    }

    public void AddWall() {
        imageCell.color = wallColor;
        button.interactable = false;
        // imageCell.sprite = null;
        // background.SetActive(false);
    }

    public void AddDoor() {
        imageCell.color = doorColor;
        button.interactable = false;
        // imageCell.sprite = null;
        // background.SetActive(false);
    }

    private void ResizeCellSize() {
        // if (configTopLayer == null && configMiddleLayer == null && configBottomLayer == null) {
        //     return;
        // }
        float width;
        float height;
        float rectWidth = rectTransform.sizeDelta.x;
        float rectHeight = rectTransform.sizeDelta.y;
        cellSize = new Vector2(rectWidth, rectHeight);
        width = rectWidth;
        height = rectHeight;
        Vector2Int size;
        Vector2Int defaultSize = new Vector2Int(1, 1);
        if (configTopLayer != null) {
            size = configTopLayer.GetSize();
            CalculCellSizeAndManagePosition(size, rectWidth, rectHeight, ref width, ref height, cellTopTransform);
            Vector2 vSize = new Vector2(width, height);
            cellTopTransform.sizeDelta = vSize;
        }
        else {
            CalculCellSizeAndManagePosition(defaultSize, rectWidth, rectHeight, ref width, ref height, cellTopTransform);
            Vector2 vSize = new Vector2(width, height);
            cellTopTransform.sizeDelta = vSize;
        }
        if (configMiddleLayer != null) {
            size = configMiddleLayer.GetSize();
            CalculCellSizeAndManagePosition(size, rectWidth, rectHeight, ref width, ref height, cellMiddleTransform);
            Vector2 vSize = new Vector2(width, height);
            cellMiddleTransform.sizeDelta = vSize;
        }
        else {
            CalculCellSizeAndManagePosition(defaultSize, rectWidth, rectHeight, ref width, ref height, cellMiddleTransform);
            Vector2 vSize = new Vector2(width, height);
            cellMiddleTransform.sizeDelta = vSize;
        }
        if (configBottomLayer != null) {
            size = configBottomLayer.GetSize();
            CalculCellSizeAndManagePosition(size, rectWidth, rectHeight, ref width, ref height, cellBottomTransform);
            Vector2 vSize = new Vector2(width, height);
            cellBottomTransform.sizeDelta = vSize;
        }
        else {
            CalculCellSizeAndManagePosition(defaultSize, rectWidth, rectHeight, ref width, ref height, cellBottomTransform);
            Vector2 vSize = new Vector2(width, height);
            cellBottomTransform.sizeDelta = vSize;
        }
    }

    private void CalculCellSizeAndManagePosition(Vector2Int size, float rectWidth, float rectHeight, ref float width, ref float height, RectTransform rectTransform) {
        if (size.x > 1 || size.y > 1) {
            width = rectWidth * size.x + (size.x - 1 * spacing.x);
            height = rectHeight * size.y + (size.y - 1 * spacing.y);
        }
        ChangeSpriteYPosition(size.y, height, rectTransform);
    }

    private void ChangeSpriteYPosition(int sizeY, float height, RectTransform rectTransform) {
        if (sizeY > 1) {
            Vector2 currentPosition = rectTransform.anchoredPosition;
            currentPosition.y = -(height - cellSize.y);
            rectTransform.anchoredPosition = currentPosition;
            //backgroundTransform.anchoredPosition = currentPosition;
        }
    }

    public Vector2 GetCellSize() {
        return cellSize;
    }

    public Vector2Int GetPosition() {
        return position;
    }

    public void Setup(Element config, LayerType layerType, Vector2 spacing, Vector2Int position) {
        this.position = position;
        this.spacing = spacing;
        StartCoroutine(AdjustSizeAfterFrame());
        ActivateDisplay();
        if (config != null) {
            SetComponentValues(config, layerType);
        }
    }

    public void SetOpacity(LayerType layerType, float opacity) {
        if (layerType == LayerType.TOP) {
            SetOpacityForLayer(imageTop, opacity);
        }
        else if (layerType == LayerType.MIDDLE) {
            SetOpacityForLayer(imageMiddle, opacity);
        }
        else if (layerType == LayerType.BOTTOM) {
            SetOpacityForLayer(imageBottom, opacity);
        }
    }

    private void SetOpacityForLayer(Image image, float opacity) {
        image.color = new Color(image.color.r, image.color.g, image.color.b, opacity);
    }

    public CellRoomGO GetRootCellRoomGO(LayerType layerType) {
        if (layerType == LayerType.TOP) {
            if (rootTopCellRoomGO == null) {
                return this;
            }
            return rootTopCellRoomGO;
        }
        else if (layerType == LayerType.MIDDLE) {
            if (rootMiddleCellRoomGO == null) {
                return this;
            }
            return rootMiddleCellRoomGO;
        }
        else if (layerType == LayerType.BOTTOM) {
            if (rootBottomCellRoomGO == null) {
                return this;
            }
            return rootBottomCellRoomGO;
        }
        return null;
    }

    public void SetupDesactivatedCell(CellRoomGO rootCellRoomGO, Element config, LayerType layerType) {
        if (layerType == LayerType.TOP) {
            configTopLayer = config;
            rootTopCellRoomGO = rootCellRoomGO;
        }
        else if (layerType == LayerType.MIDDLE) {
            configMiddleLayer = config;
            rootMiddleCellRoomGO = rootCellRoomGO;
        }
        else if (layerType == LayerType.BOTTOM) {
            configBottomLayer = config;
            rootBottomCellRoomGO = rootCellRoomGO;
        }
        DesactivateDisplay();
    }

    public void ResizeCellZiseAfterZoom() {
        StartCoroutine(AdjustSizeAfterFrame());
    }

    // used to solve the problem of recovering the size of the RectTransform, which is erroneous because it is driven by the gridLayout
    private IEnumerator AdjustSizeAfterFrame() {
        yield return new WaitForEndOfFrame();
        ResizeCellSize();
    }

}

