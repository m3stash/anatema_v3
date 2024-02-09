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
        [SerializeField] private Camera mainCamera;

        [SerializeField] private Sprite cursorSprite;
        private CellRoomPool pool;
        private GridLayoutGroup gridLayout;
        private Dictionary<RoomShapeEnum, Room> roomByShape = new Dictionary<RoomShapeEnum, Room>();
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
        private Element currenSelectedObject;
        private string[,] roomGridPlane;
        private CellPreviewManager cellPreviewManager;
        private RoomGridService roomGridService;

        private void Awake() {
            VerifySerialisables();
            CreateListeners();
            InitButtonPanel();
            CreatePooling();
            GridLayoutGroup grid = InitGrid();
            roomGridService = new RoomGridService(grid);
            cellPreviewManager = new CellPreviewManager(cellPreviewGO, roomGridService);
            currentGrid = new CreateRoomGrid(pool);
            CreateRoomInstance();
            ChangeCursor();
        }

        private void ChangeCursor(){
            Cursor.SetCursor(cursorSprite.texture, Vector2.zero, CursorMode.Auto);
        }

        public void OnPointerExit(PointerEventData eventData) {
            cellPreviewManager.Hide();
        }

        private void OnCellClickHandler(CellRoomGO cellRoomGO) {
            Element cellConfig = cellRoomGO.GetConfig();
            switch(currentAction){
                case RoomUIAction.COPY:
                    CopyCell(cellConfig);
                break;
                case RoomUIAction.SELECT:
                    SelectCell(cellConfig, cellRoomGO);
                break;
                case RoomUIAction.TRASH:
                    DeleteCell(cellRoomGO);
                break;
            }
        }

        private void DeleteCell(CellRoomGO cellRoomGO){
            if(!IsVoidCell(cellRoomGO)){
                roomGridService.DeleteCell(cellRoomGO);
                cellPreviewManager.OnClickTrashAction(cellRoomGO);
            }
        }

        private void SelectCell(Element cellConfig, CellRoomGO cellRoomGO){
            bool isExistingCell = roomGridService.CreateCell(cellConfig, cellRoomGO, currenSelectedObject);
            if(isExistingCell){
                cellPreviewManager.Forbidden();
            }
        }

        private void CopyCell(Element cellConfig){
            if(cellConfig != null){
                roomUIStateManager.OnSelectObject(cellConfig);
                OnSelectButtonClick();
            }
        }

        private void OnCellPointerEnterHandler(CellRoomGO cellRoomGO) {
            cellPreviewManager.Reset();
            if(cellRoomGO.IsDoorOrWall()){
                cellPreviewManager.Hide();
                return;
            }
            bool isVoidCell = IsVoidCell(cellRoomGO);
            Vector2 cellSize = cellRoomGO.GetCellSize(); // ex: 28 x 28
            Vector3 cellRoomGOPosition = cellRoomGO.transform.position;
            switch(currentAction){
                case RoomUIAction.SELECT:
                    cellPreviewManager.OnHoverSelectAction(cellRoomGO, cellSize, cellRoomGOPosition, isVoidCell, currenSelectedObject);
                    break;
                case RoomUIAction.TRASH:
                    cellPreviewManager.OnHoverTrashAction(cellRoomGO, cellSize, cellRoomGOPosition);
                    break;
                case RoomUIAction.COPY:
                    cellPreviewManager.OnHoverCopyAction(cellRoomGO, cellSize, cellRoomGOPosition);
                    break;
                default:
                    cellPreviewManager.Reset();
                    break;
            }
        }

        private void InitButtonPanel() {
            currentAction = RoomUIAction.SELECT;
            defaultButtonColor = selectButton.colors.normalColor;
            ChangeButtonColor(selectButton, Color.yellow);
        }

        private void ChangeButtonColor(Button button, Color color) {
            ColorBlock colorBlock = button.colors;
            colorBlock.normalColor = color;
            colorBlock.selectedColor = color;
            button.colors = colorBlock;
        }

        private void ResetButtonsColor() {
            ChangeButtonColor(selectButton, defaultButtonColor);
            ChangeButtonColor(copyButton, defaultButtonColor);
            ChangeButtonColor(trashButton, defaultButtonColor);
        }

        private GridLayoutGroup InitGrid() {
            gridLayout = gameObject.GetComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(cellSize, cellSize);
            gridLayout.spacing = new Vector2(cellSpacing, cellSpacing);
            rectTransform = gridLayout.GetComponent<RectTransform>();
            return gridLayout;
        }

        private void CreatePooling() {
            pool = cellPool.GetComponent<CellRoomPool>();
            PoolConfig config = pool.GetConfig();
            if (!config) {
                Debug.LogError("Error no config for cellPool on GridManager awake !");
            } else {
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
                { "cellPreviewGO", cellPreviewGO }
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
            gridZoomMinus.onClick.RemoveListener(OnGridZoomMinusClick);
            gridZoomPlus.onClick.RemoveListener(OnGridZoomPlusClick);
        }

        private void CreateListeners() {
            CellRoomGO.OnPointerEnterEvent += OnCellPointerEnterHandler;
            CellRoomGO.OnClick += OnCellClickHandler;
            roomUIStateManager.OnShapeChange += OnShapeChange;
            roomUIStateManager.OnBiomeChange += OnBiomeChange;
            roomUIStateManager.OnObjectSelected += OnObjectSelectedHandler;

            if (gridZoomMinus != null) {
                gridZoomMinus.onClick.AddListener(OnGridZoomMinusClick);
            }else{
                Debug.LogError("gridZoomMinus is null");
            }
            if (gridZoomPlus != null) {
                gridZoomPlus.onClick.AddListener(OnGridZoomPlusClick);
            }else{
                Debug.LogError("gridZoomPlus is null");
            }
            if (selectButton != null) {
                selectButton.onClick.AddListener(OnSelectButtonClick);
            }else{
                Debug.LogError("selectButton is null");
            }
            if (copyButton != null) {
                copyButton.onClick.AddListener(OnCopyButtonClick);
            }else{
                Debug.LogError("copyButton is null");
            }
            if (trashButton != null) {
                trashButton.onClick.AddListener(OnTrashButtonClick);
            }else{
                Debug.LogError("trashButton is null");
            }
        }

        private bool IsVoidCell(CellRoomGO cellRoomGO) {
            if(cellRoomGO?.GetConfig() == null && !cellRoomGO.IsDesactivatedCell()){
                return true;
            }
            return false;
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

        private void OnObjectSelectedHandler(Element selectedObject) {
            currenSelectedObject = selectedObject;
            OnSelectButtonClick();
        }

        private void CreateGridView(){
            string currentBiome = roomUIStateManager.CurrentBiome;
            string currentShape = roomUIStateManager.CurrentShape;
            RoomShapeEnum? newShape = Utilities.GetEnumValueFromDropdown<RoomShapeEnum>(currentShape);
            BiomeEnum? newBiome = Utilities.GetEnumValueFromDropdown<BiomeEnum>(currentBiome);
            if(newShape.HasValue && newBiome.HasValue) {
                GenerateGrid(newShape.Value);
                currentZoom = 1;
                Zoom(currentZoom);
            }else{
                currentGrid.ResetGrid();
            }
        }

        private void OnShapeChange(string shape) {
            CreateGridView();
        }

        private void OnBiomeChange(string biome){
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
            if(currentGrid != null){
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
                currentGrid.GenerateGrid(transform, roomSections, roomSize, rows, cols);
                roomGridPlane = currentGrid.RoomGridPlane;
                ModifyGridLayoutRectTransform(cols, rows);
            } else {
                Debug.Log("GridManager Start, error: "+ shape + " Not Exist...");
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