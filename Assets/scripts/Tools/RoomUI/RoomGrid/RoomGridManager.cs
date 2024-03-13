using UnityEngine;
using RoomNs;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace RoomUI {
    public class RoomGridManager : MonoBehaviour, IPointerExitHandler {
        [SerializeField] private RoomUIStateManager roomUIStateManager;
        [SerializeField] private GameObject modalRoomManageRowPoolGO;
        [SerializeField] private GameObject cellPool;
        [SerializeField] private GameObject cellPreviewGO;
        [SerializeField] private Button gridZoomMinus;
        [SerializeField] private Button gridZoomPlus;
        [SerializeField] private Button selectButton;
        [SerializeField] private Button copyButton;
        [SerializeField] private Button trashButton;
        [SerializeField] private Button layerTopButton;
        [SerializeField] private Button layerMiddleButton;
        [SerializeField] private Button layerBottomButton;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Sprite cursorSprite;
        [SerializeField] private GameObject roomUIInputManagerGO;

        private CellRoomPool pool;
        private GridLayoutGroup gridLayout;
        private Dictionary<RoomShapeEnum, Room> roomByShape = new Dictionary<RoomShapeEnum, Room>();
        private Dictionary<LayerType, Dictionary<int, (int, Image, Image, Image)>> cellRoomGoDictionary = new Dictionary<LayerType, Dictionary<int, (int, Image, Image, Image)>>();
        private CreateRoomGrid currentGrid;
        private RectTransform rectTransform;
        private int cellSize = 28;
        private int cellSpacing = 1;
        private int defaultWidth;
        private int defaultHeight;
        private float currentZoom = 1;
        private float zoomIncrement = 0.5f;
        private Color selectedButtonColor = Color.yellow;
        private Color defaultButtonColor;
        private RoomUIAction currentAction;
        private LayerType layerType = LayerType.MIDDLE;
        private Element currenSelectedObject;
        private CellPreviewManager cellPreviewManager;
        private RoomGridService roomGridService;
        private RoomUIInput.Page_EventActions inputAction;
        private RoomUIInputManager roomUIInputManager;
        private bool holdClick = false;

        private CellRoomGO currentHoverCell;

        private void Awake() {
            VerifySerialisables();
            CreateListeners();
            InitCellDictionary();
            InitButtonPanel();
            CreatePooling();
            InitGrid();
            CreateRoomInstance();
            CreateInputAction();
            // ChangeCursor();
        }

        private void CreateInputAction() {
            if (roomUIInputManagerGO != null) {
                roomUIInputManager = roomUIInputManagerGO.GetComponent<RoomUIInputManager>();
                inputAction = roomUIInputManager.GetRoomUIInput().Page_Event;
                inputAction.CellClickHold.performed += OnCellClickHoldHandler;
                inputAction.CellClickLeave.performed += OnCellClickLeaveHandler;
            }
            else {
                Debug.LogError("RoomUIService(Awake), roomUIInputManager is null");
            }

        }

        private void OnCellClickHoldHandler(InputAction.CallbackContext context) {
            if (currentHoverCell != null && !holdClick) {
                Element cellConfig = currentHoverCell.GetConfig(layerType);
                switch (currentAction) {
                    case RoomUIAction.COPY:
                        CopyCell(cellConfig);
                        break;
                    case RoomUIAction.SELECT:
                        SelectCell(currentHoverCell);
                        break;
                    case RoomUIAction.TRASH:
                        DeleteCell(currentHoverCell);
                        break;
                }
            }
            holdClick = true;
        }

        private void OnCellClickLeaveHandler(InputAction.CallbackContext context) {
            holdClick = false;
        }

        private void InitCellDictionary() {
            cellRoomGoDictionary.Add(LayerType.TOP, new Dictionary<int, (int, Image, Image, Image)>());
            cellRoomGoDictionary.Add(LayerType.MIDDLE, new Dictionary<int, (int, Image, Image, Image)>());
            cellRoomGoDictionary.Add(LayerType.BOTTOM, new Dictionary<int, (int, Image, Image, Image)>());
        }

        private void InitGrid() {
            gridLayout = gameObject.GetComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(cellSize, cellSize);
            gridLayout.spacing = new Vector2(cellSpacing, cellSpacing);
            rectTransform = gridLayout.GetComponent<RectTransform>();
            roomGridService = new RoomGridService(gridLayout);
            cellPreviewManager = new CellPreviewManager(cellPreviewGO, roomGridService);
            currentGrid = new CreateRoomGrid(pool);
        }

        public (List<GridElementModel>, List<GridElementModel>, List<GridElementModel>) GetLayers() {
            return (roomGridService.GetTopLayer(), roomGridService.GetBottomLayer(), roomGridService.GetMiddleLayer());
        }

        private void ChangeCursor() {
            Cursor.SetCursor(cursorSprite.texture, Vector2.zero, CursorMode.Auto);
        }

        public void OnPointerExit(PointerEventData eventData) {
            currentHoverCell = null;
            cellPreviewManager.Hide();
        }

        private void OnCellClickHandler(CellRoomGO cellRoomGO) {
            Element cellConfig = cellRoomGO.GetConfig(layerType);
            switch (currentAction) {
                case RoomUIAction.COPY:
                    CopyCell(cellConfig);
                    break;
                case RoomUIAction.SELECT:
                    SelectCell(cellRoomGO);
                    break;
                case RoomUIAction.TRASH:
                    DeleteCell(cellRoomGO);
                    break;
            }
        }

        public void AddCellRoomGoItem(int instanceID, int index, LayerType layerType, (Image, Image, Image) images) {
            if (!cellRoomGoDictionary[layerType].ContainsKey(instanceID)) {
                cellRoomGoDictionary[layerType][instanceID] = (index, images.Item1, images.Item2, images.Item3);
            }
            Debug.Log("ADD removed from list " + cellRoomGoDictionary[layerType].Count);
        }

        public bool RemoveCellRoomGoItemByLayerAndInstanceID(int instanceID, LayerType layerType) {
            if (cellRoomGoDictionary.ContainsKey(layerType) && cellRoomGoDictionary[layerType].ContainsKey(instanceID)) {
                cellRoomGoDictionary[layerType].Remove(instanceID);
                return true;
            }
            return false;
        }

        public int GetIndexByLayerAndInstanceID(int instanceID, LayerType layerType) {
            if (cellRoomGoDictionary.ContainsKey(layerType) && cellRoomGoDictionary[layerType].ContainsKey(instanceID)) {
                if (cellRoomGoDictionary.TryGetValue(layerType, out var layerDictionary)) {
                    if (layerDictionary.TryGetValue(instanceID, out var data)) {
                        return data.Item1;
                    }
                }
            }
            return -1;
        }

        private void DeleteCell(CellRoomGO cellRoomGO) {
            if (!IsVoidCell(cellRoomGO)) {
                Vector2Int cellSize = cellRoomGO.GetConfig(layerType).GetSize();
                CellRoomGO rootCell = cellRoomGO;
                bool isdeletedCell = false;
                if (cellSize.x > 1 || cellSize.y > 1) {
                    int index = GetIndexByLayerAndInstanceID(cellRoomGO.GetRootCellRoomGOInstanceID(layerType), layerType);
                    if (index != -1) {
                        rootCell = roomGridService.GetCellByIndex(index);
                        if (rootCell) {
                            isdeletedCell = roomGridService.DeleteCell(rootCell, layerType);
                        }
                        else {
                            Debug.LogError("DeleteCell: no rootCell with this id: ");
                        }
                    }
                }
                else {
                    isdeletedCell = roomGridService.DeleteCell(cellRoomGO, layerType);
                }
                if (isdeletedCell) {
                    bool isEmptyCell = rootCell.IsLayersEmpty();
                    if (isEmptyCell) {
                        RemoveCellRoomGoItemByLayerAndInstanceID(rootCell.GetInstanceID(), layerType);
                        Debug.Log("CellRoomGO removed from list " + cellRoomGoDictionary[layerType].Count);
                    }
                    cellPreviewManager.SetPreviewByActionType(PreviewAction.HOVER, cellRoomGO.transform.position, new Vector2(1, 1), cellRoomGO.GetCellSize());
                }
            }
        }

        private void SelectCell(CellRoomGO cellRoomGO) {
            bool isCellCreated = roomGridService.CreateCell(cellRoomGO, currenSelectedObject, layerType);
            if (isCellCreated) {
                int index = roomGridService.GetCellIndexByPosition(cellRoomGO.GetPosition());
                AddCellRoomGoItem(cellRoomGO.GetInstanceID(), index, layerType, cellRoomGO.GetImages());
            }
            if (currenSelectedObject != null)
                cellPreviewManager.Forbidden();
        }

        private void CopyCell(Element cellConfig) {
            if (cellConfig != null) {
                roomUIStateManager.OnSelectObject(cellConfig);
                OnSelectButtonClick();
            }
        }

        private CellRoomGO GetRootCellByIdAndLayer(CellRoomGO cell, LayerType layerType) {
            int index = GetIndexByLayerAndInstanceID(cell.GetRootCellRoomGOInstanceID(layerType), layerType);
            return roomGridService.GetCellByIndex(index);
        }

        private void OnCellPointerEnterHandler(CellRoomGO cellRoomGO) {
            currentHoverCell = cellRoomGO;
            cellPreviewManager.Reset();
            if (cellRoomGO.IsDoorOrWall()) {
                cellPreviewManager.Hide();
                return;
            }
            bool isVoidCell = IsVoidCell(cellRoomGO);
            Vector2 cellSize = cellRoomGO.GetCellSize(); // ex: 28 x 28
            Vector3 cellRoomGOPosition = cellRoomGO.transform.position;
            Vector2Int elementSize;
            if (!isVoidCell) {
                elementSize = cellRoomGO.GetConfig(layerType).GetSize();
            }
            else {
                elementSize = new Vector2Int(1, 1);
            }
            switch (currentAction) {
                case RoomUIAction.SELECT:
                    cellPreviewManager.OnHoverSelectAction(cellRoomGO, cellSize, cellRoomGOPosition, isVoidCell, currenSelectedObject, layerType);
                    if (holdClick) {
                        OnCellClickHandler(cellRoomGO);
                    }
                    break;
                case RoomUIAction.TRASH:
                    if (elementSize.x > 1 || elementSize.y > 1) {
                        CellRoomGO rootCell = GetRootCellByIdAndLayer(cellRoomGO, layerType);
                        cellPreviewManager.OnHoverTrashAction(rootCell, cellSize, cellRoomGOPosition, layerType);
                    }
                    else {
                        cellPreviewManager.OnHoverTrashAction(cellRoomGO, cellSize, cellRoomGOPosition, layerType);
                    }
                    if (holdClick) {
                        OnCellClickHandler(cellRoomGO);
                    }
                    break;
                case RoomUIAction.COPY:
                    if (elementSize.x > 1 || elementSize.y > 1) {
                        CellRoomGO rootCell = GetRootCellByIdAndLayer(cellRoomGO, layerType);
                        cellPreviewManager.OnHoverCopyAction(rootCell, cellSize, cellRoomGOPosition, layerType);
                    }
                    else {
                        cellPreviewManager.OnHoverCopyAction(cellRoomGO, cellSize, cellRoomGOPosition, layerType);
                    }
                    break;
                default:
                    cellPreviewManager.Reset();
                    break;
            }
        }

        private void InitButtonPanel() {
            currentAction = RoomUIAction.SELECT;
            layerType = LayerType.MIDDLE;
            defaultButtonColor = selectButton.colors.normalColor;
            ChangeButtonColor(selectButton, Color.yellow);
            ChangeButtonColor(layerMiddleButton, Color.yellow);
        }

        private void ChangeButtonColor(Button button, Color color) {
            ColorBlock colorBlock = button.colors;
            colorBlock.normalColor = color;
            colorBlock.selectedColor = color;
            button.colors = colorBlock;
        }

        private void ResetLayersColor() {
            ChangeButtonColor(layerTopButton, defaultButtonColor);
            ChangeButtonColor(layerMiddleButton, defaultButtonColor);
            ChangeButtonColor(layerBottomButton, defaultButtonColor);
        }

        private void ResetButtonsColor() {
            ChangeButtonColor(selectButton, defaultButtonColor);
            ChangeButtonColor(copyButton, defaultButtonColor);
            ChangeButtonColor(trashButton, defaultButtonColor);
        }

        private void CreatePooling() {
            pool = cellPool.GetComponent<CellRoomPool>();
            PoolConfig config = pool.GetConfig();
            if (!config) {
                Debug.LogError("Error no config for cellPool on GridManager awake !");
            }
            else {
                pool.Setup(config.GetPrefab(), config.GetPoolSize());
            }
        }

        private void VerifySerialisables() {
            Dictionary<string, object> serializableFields = new Dictionary<string, object> {
                { "roomUIStateManager", roomUIStateManager },
                { "cellPool", cellPool },
                { "gridZoomMinus", gridZoomMinus },
                { "gridZoomPlus", gridZoomPlus },
                { "selectButton", selectButton },
                { "copyButton", copyButton },
                { "trashButton", trashButton },
                { "cellPreviewGO", cellPreviewGO },
                { "layerTopButton", layerTopButton },
                { "layerMiddleButton", layerMiddleButton },
                { "layerBottomButton", layerBottomButton },
                { "mainCamera", mainCamera },
                { "cursorSprite", cursorSprite }
            };
            foreach (var field in serializableFields) {
                if (field.Value == null) {
                    Debug.LogError($"ItemGridManager SerializeField {field.Key} not set !");
                }
            }
        }

        private void OnDestroy() {
            CellRoomGO.OnPointerEnterEvent -= OnCellPointerEnterHandler;
            roomUIStateManager.OnShapeChange -= OnShapeChange;
            roomUIStateManager.OnBiomeChange -= OnBiomeChange;
            roomUIStateManager.OnObjectSelected -= OnObjectSelectedHandler;
            roomUIStateManager.OnRoomLoad -= OnLoadRoomHandler;
            roomUIStateManager.OnRoomDelete -= OnDeleteRoomHandler;
            gridZoomMinus.onClick.RemoveListener(OnGridZoomMinusClick);
            gridZoomPlus.onClick.RemoveListener(OnGridZoomPlusClick);
            roomUIStateManager.OnRoomReset -= OnDeleteRoomHandler;
        }

        private void CreateListeners() {
            CellRoomGO.OnPointerEnterEvent += OnCellPointerEnterHandler;
            roomUIStateManager.OnShapeChange += OnShapeChange;
            roomUIStateManager.OnBiomeChange += OnBiomeChange;
            roomUIStateManager.OnObjectSelected += OnObjectSelectedHandler;
            roomUIStateManager.OnRoomLoad += OnLoadRoomHandler;
            roomUIStateManager.OnRoomDelete += OnDeleteRoomHandler;
            roomUIStateManager.OnRoomReset += OnDeleteRoomHandler;

            if (gridZoomMinus != null) {
                gridZoomMinus.onClick.AddListener(OnGridZoomMinusClick);
            }
            else {
                Debug.LogError("gridZoomMinus is null");
            }
            if (gridZoomPlus != null) {
                gridZoomPlus.onClick.AddListener(OnGridZoomPlusClick);
            }
            else {
                Debug.LogError("gridZoomPlus is null");
            }
            if (selectButton != null) {
                selectButton.onClick.AddListener(OnSelectButtonClick);
            }
            else {
                Debug.LogError("selectButton is null");
            }
            if (copyButton != null) {
                copyButton.onClick.AddListener(OnCopyButtonClick);
            }
            else {
                Debug.LogError("copyButton is null");
            }
            if (layerTopButton != null) {
                layerTopButton.onClick.AddListener(OnLayerTopButtonClick);
            }
            else {
                Debug.LogError("layerTopButton is null");
            }
            if (layerMiddleButton != null) {
                layerMiddleButton.onClick.AddListener(OnLayerMiddleButtonClick);
            }
            else {
                Debug.LogError("layerMiddleButton is null");
            }
            if (layerBottomButton != null) {
                layerBottomButton.onClick.AddListener(OnLayerBottomButtonClick);
            }
            else {
                Debug.LogError("layerBottomButton is null");
            }
            if (trashButton != null) {
                trashButton.onClick.AddListener(OnTrashButtonClick);
            }
            else {
                Debug.LogError("trashButton is null");
            }
        }

        private bool IsVoidCell(CellRoomGO cellRoomGO) {
            if (cellRoomGO.IsVoidCell(layerType) && !cellRoomGO.IsDesactivatedCell(layerType)) {
                return true;
            }
            return false;
        }

        private void OnDeleteRoomHandler(int id) {
            roomGridService.ResetLayers();
            currentGrid.ResetGrid();
            if (id != -1)
                TooltipManager.Instance.CallTooltip(TooltipType.INFORMATION, "Room GRID reset because current id had been deleted !");
        }

        private void OnLoadRoomHandler(RoomUIModel roomUIModel) {
            if (roomUIModel == null) {
                Debug.LogError("RoomGridManager(OnLoadRoomHandler): RoomUIModel is null copy not possible !");
                return;
            }
            roomGridService.ResetLayers();
            currentGrid.ResetGrid();
            cellRoomGoDictionary.Clear();
            InitCellDictionary();
            if (cellRoomGoDictionary.Count > 0) {
                Debug.Log("Clear List" + cellRoomGoDictionary[layerType].Count);
            }
            CreateGridView();
            CreateCellsForLayer(roomUIModel.TopLayer, LayerType.TOP);
            CreateCellsForLayer(roomUIModel.MiddleLayer, LayerType.MIDDLE);
            CreateCellsForLayer(roomUIModel.BottomLayer, LayerType.BOTTOM);
            ChangeLayerOpacity(layerType);
        }

        private void CreateCellsForLayer(List<GridElementModel> layerGridModel, LayerType layer) {
            if (layerGridModel.Count == 0) return;
            layerGridModel.ForEach(cell => {
                int x = cell.GetPosition().x;
                int y = cell.GetPosition().y;
                int index = y * gridLayout.constraintCount + x;
                GameObject cellObject = gridLayout.transform.GetChild(index).gameObject;
                CellRoomGO cellRoomGO = cellObject.GetComponent<CellRoomGO>();
                bool isCreatedCell = roomGridService.CreateCell(cellRoomGO, cell.GetElement(), layer);
                if (isCreatedCell) {
                    AddCellRoomGoItem(cellRoomGO.GetInstanceID(), index, layer, cellRoomGO.GetImages());
                }
            });
        }

        private void OnTrashButtonClick() {
            SetButtonConfiguration(RoomUIAction.TRASH, trashButton);
        }

        private void OnCopyButtonClick() {
            SetButtonConfiguration(RoomUIAction.COPY, copyButton);
        }

        private void OnSelectButtonClick() {
            SetButtonConfiguration(RoomUIAction.SELECT, selectButton);
        }

        private void SetButtonConfiguration(RoomUIAction action, Button button) {
            currentAction = action;
            ResetButtonsColor();
            ChangeButtonColor(button, selectedButtonColor);
        }

        private void SetLayerConfiguration(LayerType layerType, Button button) {
            this.layerType = layerType;
            ChangeLayerOpacity(layerType);
            ResetLayersColor();
            ChangeButtonColor(button, selectedButtonColor);
        }

        private void SetOpacityForLayer(Image image, float opacity) {
            image.color = new Color(image.color.r, image.color.g, image.color.b, opacity);
        }

        private void ChangeOpacityForLayers(LayerType layer, LayerType currentLayer) {
            if (!cellRoomGoDictionary.ContainsKey(layer)) return;
            foreach (var kvp in cellRoomGoDictionary[layer]) {
                Image layerTop = kvp.Value.Item2;
                Image layerMiddle = kvp.Value.Item3;
                Image layerBottom = kvp.Value.Item4;
                switch (currentLayer) {
                    case LayerType.TOP:
                        SetOpacityForLayer(layerTop, 1f);
                        SetOpacityForLayer(layerMiddle, 0.5f);
                        SetOpacityForLayer(layerBottom, 0.5f);
                        break;
                    case LayerType.MIDDLE:
                        SetOpacityForLayer(layerTop, 0.5f);
                        SetOpacityForLayer(layerMiddle, 1f);
                        SetOpacityForLayer(layerBottom, 0.5f);
                        break;
                    case LayerType.BOTTOM:
                        SetOpacityForLayer(layerTop, 0.5f);
                        SetOpacityForLayer(layerMiddle, 0.5f);
                        SetOpacityForLayer(layerBottom, 1f);
                        break;
                }
            }
        }

        private void ChangeLayerOpacity(LayerType layerType) {
            ChangeOpacityForLayers(LayerType.TOP, layerType);
            ChangeOpacityForLayers(LayerType.BOTTOM, layerType);
            ChangeOpacityForLayers(LayerType.MIDDLE, layerType);
        }

        private void OnLayerTopButtonClick() {
            SetLayerConfiguration(LayerType.TOP, layerTopButton);
        }

        private void OnLayerMiddleButtonClick() {
            SetLayerConfiguration(LayerType.MIDDLE, layerMiddleButton);
        }

        private void OnLayerBottomButtonClick() {
            SetLayerConfiguration(LayerType.BOTTOM, layerBottomButton);
        }

        private void OnObjectSelectedHandler(Element selectedObject) {
            currenSelectedObject = selectedObject;
            OnSelectButtonClick();
        }

        private void CreateGridView() {
            string currentBiome = roomUIStateManager.CurrentBiome;
            string currentShape = roomUIStateManager.CurrentShape;
            RoomShapeEnum? newShape = Utilities.GetEnumValueFromDropdown<RoomShapeEnum>(currentShape);
            BiomeEnum? newBiome = Utilities.GetEnumValueFromDropdown<BiomeEnum>(currentBiome);
            if (newShape.HasValue && newBiome.HasValue) {
                GenerateGrid(newShape.Value);
                currentZoom = 1;
                Zoom(currentZoom);
            }
            else {
                currentGrid.ResetGrid();
                cellRoomGoDictionary.Clear();
                InitCellDictionary();
            }
        }

        private void OnShapeChange(string shape) {
            CreateGridView();
        }

        private void OnBiomeChange(string biome) {
            CreateGridView();
        }

        private void OnGridZoomMinusClick() {
            currentZoom = currentZoom == 1 ? 1 : currentZoom - zoomIncrement;
            Zoom(currentZoom);
        }

        private void OnGridZoomPlusClick() {
            currentZoom += zoomIncrement;
            Zoom(currentZoom);
        }

        private void Zoom(float currentZoom) {
            float newWidth = currentZoom * defaultWidth;
            float newHeight = currentZoom * defaultHeight;
            gridLayout.cellSize = new Vector2(cellSize * currentZoom, cellSize * currentZoom);
            rectTransform.sizeDelta = new Vector2(newWidth, newHeight);
            if (currentGrid != null) {
                currentGrid.UsedCells.ForEach(cell => {
                    cell.ResizeCellZiseAfterZoom();
                });
            }
        }

        private void GenerateGrid(RoomShapeEnum shape) {
            if (roomByShape.ContainsKey(shape)) {
                Room room = roomByShape[shape];
                Vector2Int[] roomSections = room.GetSections(Vector2Int.zero);
                Vector2Int roomSize = room.GetSizeOfRoom();
                int cols = roomSize.x * (int)RoomSizeEnum.WIDTH;
                int rows = roomSize.y * (int)RoomSizeEnum.HEIGHT;
                gridLayout.constraintCount = cols;
                currentGrid.GenerateGrid(transform, roomSections, roomSize, rows, cols, layerType);
                ModifyGridLayoutRectTransform(cols, rows);
            }
            else {
                Debug.Log("GridManager Start, error: " + shape + " Not Exist...");
            }
        }

        void ModifyGridLayoutRectTransform(int cols, int rows) {
            int width = (cellSize * cols) + (cellSpacing * (cols - 1));
            int height = (cellSize * rows) + (cellSpacing * (rows - 1));
            defaultWidth = width;
            defaultHeight = height;
            rectTransform.sizeDelta = new Vector2(width, height);
        }

        // used to create true Room by shape and get all function and properties like sections etc.
        private void CreateRoomInstance() {
            foreach (RoomShapeEnum shape in Enum.GetValues(typeof(RoomShapeEnum))) {
                Room room = RoomFactory.GetInstance().InstantiateRoomImpl(shape);
                roomByShape.Add(shape, room);
            }
        }

    }
}