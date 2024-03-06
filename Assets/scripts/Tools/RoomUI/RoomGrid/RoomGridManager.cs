using UnityEngine;
using RoomNs;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace RoomUI {
    public class RoomGridManager : MonoBehaviour, IPointerExitHandler {
        [SerializeField] private RoomUIStateManager roomUIStateManager;
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

        private CellRoomPool pool;
        private GridLayoutGroup gridLayout;
        private Dictionary<RoomShapeEnum, Room> roomByShape = new Dictionary<RoomShapeEnum, Room>();
        private List<CellRoomGO> cellRoomGoList = new List<CellRoomGO>();
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
        private string[,] roomGridPlane;
        private CellPreviewManager cellPreviewManager;
        private RoomGridService roomGridService;

        private void Awake() {
            VerifySerialisables();
            CreateListeners();
            InitButtonPanel();
            CreatePooling();
            InitGrid();
            CreateRoomInstance();
            // ChangeCursor();
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

        private void DeleteCell(CellRoomGO cellRoomGO) {
            if (!IsVoidCell(cellRoomGO)) {
                Vector2Int cellSize = cellRoomGO.GetConfig(layerType).GetSize();
                CellRoomGO rootCell = cellRoomGO;
                bool isdeletedCell;
                if (cellSize.x > 1 || cellSize.y > 1) {
                    rootCell = cellRoomGoList.Find(cell => cell.GetInstanceID() == cellRoomGO.GetRootCellRoomGOInstanceID(layerType));
                    isdeletedCell = roomGridService.DeleteCell(rootCell, layerType);
                }
                else {
                    isdeletedCell = roomGridService.DeleteCell(cellRoomGO, layerType);
                }
                if (isdeletedCell) {
                    bool isEmptyCell = rootCell.IsLayersEmpty();
                    if (isEmptyCell) {
                        cellRoomGoList.Remove(rootCell);
                        Debug.Log("CellRoomGO removed from list " + cellRoomGoList.Count);
                    }
                }
                cellPreviewManager.SetPreviewByActionType(PreviewAction.HOVER, cellRoomGO.transform.position, new Vector2(1, 1), cellRoomGO.GetCellSize());
            }
        }

        private void SelectCell(CellRoomGO cellRoomGO) {
            bool isCellCreated = roomGridService.CreateCell(cellRoomGO, currenSelectedObject, layerType);
            if (isCellCreated) {
                AddUniqueCell(cellRoomGO);
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

        private void OnCellPointerEnterHandler(CellRoomGO cellRoomGO) {
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
                    if (elementSize.x > 1 || elementSize.y > 1) {
                        CellRoomGO rootCell = cellRoomGoList.Find(cell => cell.GetInstanceID() == cellRoomGO.GetInstanceIDByLayer(layerType));
                        // if (rootCell == null) return;
                        cellPreviewManager.OnHoverSelectAction(rootCell, cellSize, cellRoomGOPosition, isVoidCell, currenSelectedObject, layerType);
                    }
                    else {
                        cellPreviewManager.OnHoverSelectAction(cellRoomGO, cellSize, cellRoomGOPosition, isVoidCell, currenSelectedObject, layerType);
                    }
                    break;
                case RoomUIAction.TRASH:
                    if (elementSize.x > 1 || elementSize.y > 1) {
                        CellRoomGO rootCell = cellRoomGoList.Find(cell => cell.GetInstanceID() == cellRoomGO.GetInstanceIDByLayer(layerType));
                        cellPreviewManager.OnHoverTrashAction(rootCell, cellSize, cellRoomGOPosition, layerType);
                    }
                    else {
                        cellPreviewManager.OnHoverTrashAction(cellRoomGO, cellSize, cellRoomGOPosition, layerType);
                    }
                    break;
                case RoomUIAction.COPY:
                    if (elementSize.x > 1 || elementSize.y > 1) {
                        CellRoomGO rootCell = cellRoomGoList.Find(cell => cell.GetInstanceID() == cellRoomGO.GetInstanceIDByLayer(layerType));
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
            CellRoomGO.OnClick -= OnCellClickHandler;
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
            CellRoomGO.OnClick += OnCellClickHandler;
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
            if (cellRoomGO?.GetConfig(layerType) == null && !cellRoomGO.IsDesactivatedCell(layerType)) {
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

        private void AddUniqueCell(CellRoomGO cellRoomGO) {
            if (!cellRoomGoList.Contains(cellRoomGO)) {
                cellRoomGoList.Add(cellRoomGO);
                Debug.Log("CellRoomGO added to list " + cellRoomGoList.Count);
            }
        }

        private void OnLoadRoomHandler(RoomUIModel roomUIModel) {
            if (roomUIModel == null) {
                Debug.LogError("RoomGridManager(OnLoadRoomHandler): RoomUIModel is null copy not possible !");
                return;
            }

            roomGridService.ResetLayers();
            currentGrid.ResetGrid();
            ClearCellRoomGoList();
            Debug.Log("Clear List" + cellRoomGoList.Count);
            CreateGridView();

            CreateCellsForLayer(roomUIModel.TopLayer);
            CreateCellsForLayer(roomUIModel.MiddleLayer);
            CreateCellsForLayer(roomUIModel.BottomLayer);
        }

        private void CreateCellsForLayer(List<GridElementModel> layer) {
            layer.ForEach(cell => {
                int x = cell.GetPosition().x;
                int y = cell.GetPosition().y;
                int index = y * gridLayout.constraintCount + x;
                GameObject cellObject = gridLayout.transform.GetChild(index).gameObject;
                CellRoomGO cellRoomGO = cellObject.GetComponent<CellRoomGO>();
                bool isCreatedCell = roomGridService.CreateCell(cellRoomGO, cell.GetElement(), layerType);
                if (isCreatedCell) {
                    AddUniqueCell(cellRoomGO);
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

        private void ChangeLayerOpacity(LayerType layerType) {
            if (layerType == LayerType.TOP) {
                cellRoomGoList.ForEach(cellRoomGO => {
                    cellRoomGO.SetOpacity(LayerType.TOP, 1f);
                    cellRoomGO.SetOpacity(LayerType.MIDDLE, 0.5f);
                    cellRoomGO.SetOpacity(LayerType.BOTTOM, 0.5f);
                });
            }
            else if (layerType == LayerType.MIDDLE) {
                cellRoomGoList.ForEach(cellRoomGO => {
                    cellRoomGO.SetOpacity(LayerType.TOP, 0.5f);
                    cellRoomGO.SetOpacity(LayerType.MIDDLE, 1f);
                    cellRoomGO.SetOpacity(LayerType.BOTTOM, 0.5f);
                });
            }
            else if (layerType == LayerType.BOTTOM) {
                cellRoomGoList.ForEach(cellRoomGO => {
                    cellRoomGO.SetOpacity(LayerType.TOP, 0.5f);
                    cellRoomGO.SetOpacity(LayerType.MIDDLE, 0.5f);
                    cellRoomGO.SetOpacity(LayerType.BOTTOM, 1f);
                });
            }
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
                ClearCellRoomGoList();
            }
        }

        private void ClearCellRoomGoList() {
            cellRoomGoList.ForEach(cell => {
                cell.SetOpacity(LayerType.TOP, 1f);
                cell.SetOpacity(LayerType.MIDDLE, 1f);
                cell.SetOpacity(LayerType.BOTTOM, 1f);
            });
            cellRoomGoList.Clear();
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