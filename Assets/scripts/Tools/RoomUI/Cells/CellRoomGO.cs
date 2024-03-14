using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using RoomUI;
using NUnit.Framework;

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
    private Element configTopLayer;
    private Element configMiddleLayer;
    private Element configBottomLayer;
    private Vector2 spacing;
    private Vector2Int position;
    private Vector2 cellSize;
    private bool cellDesactivated;
    private int rootTopCellRoomGOInstanceID;
    private int rootMiddleCellRoomGOInstanceID;
    private int rootBottomCellRoomGOInstanceID;
    private Color32 defaultColorCell = new Color32(200, 200, 200, 255);
    private Color32 doorColor = new Color32(125, 125, 45, 255);
    private Color32 wallColor = new Color32(50, 50, 50, 255);

    public delegate void CellRoomGOEvent(CellRoomGO cellRoomGO);
    public static event CellRoomGOEvent OnPointerEnterEvent;

    public void OnPointerEnter(PointerEventData eventData) {
        OnPointerEnterEvent?.Invoke(this);
    }

    void Awake() {
        // issue with localscale on 1080p resolution after instantiate go
        transform.localScale = Vector3Int.one;
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

    public int GetInstanceIDByLayer(LayerType layerType) {
        if (layerType == LayerType.TOP) {
            return rootTopCellRoomGOInstanceID;
        }
        if (layerType == LayerType.MIDDLE) {
            return rootMiddleCellRoomGOInstanceID;

        }
        if (layerType == LayerType.BOTTOM) {
            return rootBottomCellRoomGOInstanceID;
        }
        return -1;
    }

    public bool IsDesactivatedCell(LayerType layerType) {
        return cellDesactivated;
    }

    public bool IsDoorOrWall() {
        return isDoorOrWall;
    }

    public void SetIsDoorOrWall(bool isDoorOrWall) {
        this.isDoorOrWall = isDoorOrWall;
    }

    private void SetComponentValues(Element config, LayerType layerType) {
        if (layerType == LayerType.TOP || layerType == LayerType.ALL) {
            rootTopCellRoomGOInstanceID = GetInstanceID();
            configTopLayer = config;
            imageTop.sprite = configTopLayer.GetSprite();
            imageTop.raycastTarget = false;  // avoids interaction with cells above due to image size if larger than 1x1
        }
        if (layerType == LayerType.MIDDLE || layerType == LayerType.ALL) {
            rootMiddleCellRoomGOInstanceID = GetInstanceID();
            configMiddleLayer = config;
            imageMiddle.sprite = configMiddleLayer.GetSprite();
            imageMiddle.raycastTarget = false;
        }
        if (layerType == LayerType.BOTTOM || layerType == LayerType.ALL) {
            rootBottomCellRoomGOInstanceID = GetInstanceID();
            configBottomLayer = config;
            imageBottom.sprite = configBottomLayer.GetSprite();
            imageBottom.raycastTarget = false;
        }
        imageCell.sprite = transparentIcon;
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
        ResetCellLayers(LayerType.ALL);
    }

    public Element GetConfig(LayerType layerType) {
        if (layerType == LayerType.TOP) {
            return configTopLayer;
        }
        if (layerType == LayerType.MIDDLE) {
            return configMiddleLayer;
        }
        if (layerType == LayerType.BOTTOM) {
            return configBottomLayer;
        }
        return null;
    }

    public bool IsLayersEmpty() {
        return configTopLayer == null && configBottomLayer == null && configMiddleLayer == null;
    }

    public void ResetCellLayers(LayerType layerType) {
        if (layerType == LayerType.TOP || layerType == LayerType.ALL) {
            configTopLayer = null;
            if (imageTop.sprite != transparentIcon) {
                imageTop.sprite = transparentIcon;
                imageTop.raycastTarget = true;
                ResizeCellSize(LayerType.TOP);
            }
            rootTopCellRoomGOInstanceID = -1;
            cellTopTransform.anchoredPosition = Vector2.zero;
        }
        if (layerType == LayerType.MIDDLE || layerType == LayerType.ALL) {
            configMiddleLayer = null;
            if (imageMiddle.sprite != transparentIcon) {
                imageMiddle.sprite = transparentIcon;
                imageMiddle.raycastTarget = true;
                ResizeCellSize(LayerType.MIDDLE);
            }
            rootMiddleCellRoomGOInstanceID = -1;
            cellMiddleTransform.anchoredPosition = Vector2.zero;
        }
        if (layerType == LayerType.BOTTOM || layerType == LayerType.ALL) {
            configBottomLayer = null;
            if (imageBottom.sprite != transparentIcon) {
                imageBottom.sprite = transparentIcon;
                imageBottom.raycastTarget = true;
                ResizeCellSize(LayerType.BOTTOM);
            }
            rootBottomCellRoomGOInstanceID = -1;
            cellBottomTransform.anchoredPosition = Vector2.zero;
        }
        if (LayerType.ALL == layerType || configTopLayer == null && configMiddleLayer == null && configBottomLayer == null) {
            isDoorOrWall = false;
            cellDesactivated = false;
            imageCell.color = defaultColorCell;
            imageCell.sprite = null;
        }

    }

    public void AddWall() {
        imageCell.color = wallColor;
        // imageCell.sprite = null;
        // background.SetActive(false);
    }

    public void AddDoor() {
        imageCell.color = doorColor;
        // imageCell.sprite = null;
        // background.SetActive(false);
    }

    private void ResizeCellSize(LayerType layerType) {
        float rectWidth = rectTransform.sizeDelta.x;
        float rectHeight = rectTransform.sizeDelta.y;
        cellSize = new Vector2(rectWidth, rectHeight);
        int InstanceId = GetInstanceID();
        Vector2Int defaultSize = new Vector2Int(1, 1);
        switch (layerType) {
            case LayerType.TOP:
                ResizeLayerCell(rectWidth, rectHeight, defaultSize, configTopLayer, cellTopTransform);
                break;
            case LayerType.MIDDLE:
                ResizeLayerCell(rectWidth, rectHeight, defaultSize, configMiddleLayer, cellMiddleTransform);
                break;
            case LayerType.BOTTOM:
                ResizeLayerCell(rectWidth, rectHeight, defaultSize, configBottomLayer, cellBottomTransform);
                break;
            case LayerType.ALL:
                bool isRootTop = rootTopCellRoomGOInstanceID == InstanceId;
                bool isRootMiddle = rootMiddleCellRoomGOInstanceID == InstanceId;
                bool isRootBottom = rootBottomCellRoomGOInstanceID == InstanceId;
                if (isRootTop) {
                    ResizeLayerCell(rectWidth, rectHeight, defaultSize, configTopLayer, cellTopTransform);
                }
                if (isRootMiddle) {
                    ResizeLayerCell(rectWidth, rectHeight, defaultSize, configMiddleLayer, cellMiddleTransform);
                }
                if (isRootBottom) {
                    ResizeLayerCell(rectWidth, rectHeight, defaultSize, configBottomLayer, cellBottomTransform);
                }
                break;
        }
    }
    private void ResizeLayerCell(float rectWidth, float rectHeight, Vector2Int defaultSize, Element config, RectTransform cellTransform) {
        Vector2Int size = config != null ? config.GetSize() : defaultSize;
        CalculCellSizeAndManagePosition(size, rectWidth, rectHeight, cellTransform);
    }

    private void CalculCellSizeAndManagePosition(Vector2Int size, float rectWidth, float rectHeight, RectTransform rectTransform) {
        float height = rectHeight;
        float width = rectWidth;
        if (size.x > 1 || size.y > 1) {
            width = rectWidth * size.x + (size.x - 1 * spacing.x);
            height = rectHeight * size.y + (size.y - 1 * spacing.y);
        }
        rectTransform.sizeDelta = new Vector2(width, height);
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
        StartCoroutine(AdjustSizeAfterFrame(layerType));
        ActivateDisplay();
        if (config != null) {
            SetComponentValues(config, layerType);
        }
    }

    public void SetOpacity(LayerType layerType, float opacity) {
        Debug.Log("SetOpacity " + layerType + " " + opacity);
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

    public bool IsVoidCell(LayerType layerType) {
        if (layerType == LayerType.TOP) {
            return rootTopCellRoomGOInstanceID <= 0 && configTopLayer == null;
        }
        else if (layerType == LayerType.MIDDLE) {
            return rootMiddleCellRoomGOInstanceID <= 0 && configMiddleLayer == null;
        }
        else if (layerType == LayerType.BOTTOM) {
            return rootBottomCellRoomGOInstanceID <= 0 && configBottomLayer == null;
        }
        return false;
    }

    public int GetRootCellRoomGOInstanceID(LayerType layerType) {
        if (layerType == LayerType.TOP) {
            if (rootTopCellRoomGOInstanceID == -1) {
                return GetInstanceID();
            }
            return rootTopCellRoomGOInstanceID;
        }
        else if (layerType == LayerType.MIDDLE) {
            if (rootMiddleCellRoomGOInstanceID == -1) {
                return GetInstanceID();
            }
            return rootMiddleCellRoomGOInstanceID;
        }
        else if (layerType == LayerType.BOTTOM) {
            if (rootBottomCellRoomGOInstanceID == -1) {
                return GetInstanceID();
            }
            return rootBottomCellRoomGOInstanceID;
        }
        return -1;
    }

    public (Image, Image, Image) GetImages() {
        return (imageTop, imageMiddle, imageBottom);
    }

    public void SetupBigCell(int instanceID, Element config, LayerType layerType) {
        if (layerType == LayerType.TOP) {
            configTopLayer = config;
            rootTopCellRoomGOInstanceID = instanceID;
        }
        else if (layerType == LayerType.MIDDLE) {
            configMiddleLayer = config;
            rootMiddleCellRoomGOInstanceID = instanceID;
        }
        else if (layerType == LayerType.BOTTOM) {
            configBottomLayer = config;
            rootBottomCellRoomGOInstanceID = instanceID;
        }
        DesactivateDisplay();
    }

    public void ResizeCellZiseAfterZoom() {
        StartCoroutine(AdjustSizeAfterFrame(LayerType.ALL));
    }

    // used to solve the problem of recovering the size of the RectTransform, which is erroneous because it is driven by the gridLayout
    private IEnumerator AdjustSizeAfterFrame(LayerType layerType) {
        yield return new WaitForEndOfFrame();
        ResizeCellSize(layerType);
    }

}

