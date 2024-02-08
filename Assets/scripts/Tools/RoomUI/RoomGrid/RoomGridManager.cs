using UnityEngine;
using RoomNs;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.ComponentModel;

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
        private CellRoomPool pool;
        private GridLayoutGroup gridLayout;
        private Dictionary<RoomShapeEnum, Room> roomByShape = new Dictionary<RoomShapeEnum, Room>();
        private RoomGrid currentGrid;
        private RectTransform rectTransform;
        private int cellSize = 28;
        private int cellSpacing = 1;
        private int defaultWidth;
        private int defaultHeight;
        private float currentZoom = 1;
        private float zoomIncrement = 0.5f;
        private bool initGrid = true;
        private Color selectedButtonColor = Color.yellow;
        private Color defaultButtonColor;

        private RoomUIAction currentAction;

        private Element currenSelectedObject;

        private string[,] roomGridPlane;

        private CellPreviewManager cellPreviewManager;

        private void Awake() {
            VerifySerialisables();
            CreateListeners();
            InitButtonPanel();
            CreatePooling();
            InitGrid();
            CreateRoomInstance();
        }

        public void OnPointerExit(PointerEventData eventData) {
            cellPreviewManager.Hide();
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

        private void InitGrid() {
            cellPreviewManager = new CellPreviewManager(cellPreviewGO);
            gridLayout = gameObject.GetComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(cellSize, cellSize);
            gridLayout.spacing = new Vector2(cellSpacing, cellSpacing);
            rectTransform = gridLayout.GetComponent<RectTransform>();
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
            roomUIStateManager.OnShapeChange -= DropdownValueChanged;
            roomUIStateManager.OnObjectSelected -= OnObjectSelectedHandler;
            gridZoomMinus.onClick.RemoveListener(OnGridZoomMinusClick);
            gridZoomPlus.onClick.RemoveListener(OnGridZoomPlusClick);
        }

        private void CreateListeners() {
            CellRoomGO.OnPointerEnterEvent += OnCellPointerEnterHandler;
            CellRoomGO.OnClick += OnCellClickHandler;
            roomUIStateManager.OnShapeChange += DropdownValueChanged;
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
                    OnHoverSelectAction(isVoidCell, cellRoomGO, cellSize, cellRoomGOPosition);
                break;
                case RoomUIAction.TRASH:
                    OnHoverTrashAction(cellRoomGO, cellSize, cellRoomGOPosition);
                break;
                default:
                    cellPreviewManager.Reset();
                    break;
            }
        }

        private void OnHoverTrashAction(CellRoomGO cellRoomGO, Vector2 cellSize, Vector3 cellRoomGOPosition) {
            if(cellRoomGO.GetConfig() == null){
                cellPreviewManager.SetPreviewByActionType(PreviewAction.HOVER, cellRoomGOPosition, new Vector2(1, 1), cellSize);
                return;
            }
            cellPreviewManager.SetPreviewByActionType(PreviewAction.TRASH, cellRoomGO.GetRootCellRoomGO().transform.position, cellRoomGO.GetConfig().GetSize(), cellSize);
        }

        private void OnHoverSelectAction(bool isVoidCell, CellRoomGO cellRoomGO, Vector2 cellSize, Vector3 cellRoomGOPosition) {
            // simple hover for void cell and no selected Object
            if(isVoidCell && currenSelectedObject == null){
                cellPreviewManager.SetPreviewByActionType(PreviewAction.HOVER, cellRoomGOPosition, new Vector2(1, 1), cellSize);
                return;
            }
            if(currenSelectedObject != null){
                Vector2Int selectedElementSize = currenSelectedObject.GetSize();
                List<CellRoomGO> cells = GetCellsAtPosition(cellRoomGO, selectedElementSize);
                if(cells.Exists(cell => cell.GetConfig() != null || cell.IsDoorOrWall() || cell.IsDesactivatedCell())){
                    cellPreviewManager.SetPreviewByActionType(PreviewAction.FORBIDDEN, cellRoomGOPosition, selectedElementSize, cellSize);
                } else {
                    cellPreviewManager.SetPreviewByActionType(PreviewAction.SHOW_SPRITE, cellRoomGOPosition, selectedElementSize, cellSize, currenSelectedObject.GetSprite());
                }
            }
        }

        private List<CellRoomGO> GetCellsAtPosition(CellRoomGO cellRoomGO, Vector2Int selectedElementSize){
            List<CellRoomGO> cells = new List<CellRoomGO>();
            Vector2Int size = selectedElementSize;
            if(IsBigCell(selectedElementSize)) {
                Vector2Int position = cellRoomGO.GetPosition();
                int x = position.x;
                int y = position.y;
                int gridSizeX = gridLayout.constraintCount;
                for (int yOffset = 0; yOffset < size.y; yOffset++) {
                    for (int xOffset = 0; xOffset < size.x; xOffset++) {
                        int targetX = x + xOffset;
                        int targetY = y - yOffset;
                        int targetChildIndex = targetY * gridSizeX + targetX;
                        if (targetChildIndex >= 0 && targetChildIndex < gridLayout.transform.childCount) {
                            CellRoomGO targetCell = gridLayout.transform.GetChild(targetChildIndex).GetComponent<CellRoomGO>();
                            cells.Add(targetCell);
                        }
                    }
                }
                return cells;
            }
            cells.Add(cellRoomGO);
            return cells;
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
            // cellPreviewManager.Hide();
            currentAction = action;
            ResetButtonsColor();
            ChangeButtonColor(button, selectedButtonColor);
        }

        private void OnCellClickHandler(CellRoomGO cellRoomGO) {
            Element elementConfig = cellRoomGO.GetConfig();
            switch(currentAction){
                case RoomUIAction.COPY:
                    CopyCell(elementConfig);
                break;
                case RoomUIAction.SELECT:
                    CreateCell(elementConfig, cellRoomGO);
                break;
                case RoomUIAction.TRASH:
                    DeleteCell(cellRoomGO);
                break;
            }
        }

        private bool IsBigCell(Vector2Int size){
            return size.x > 1 || size.y > 1;
        }

        private void CreateCell(Element elementConfig, CellRoomGO cellRoomGO){
            if(currenSelectedObject == null || elementConfig != null) return;
            List<CellRoomGO> cells = GetCellsAtPosition(cellRoomGO, currenSelectedObject.GetSize());
            if(cells.Exists(cell => cell.GetConfig() != null || cell.IsDoorOrWall() || cell.IsDesactivatedCell())){
                cellPreviewManager.Forbidden();
            } else {
                if(IsBigCell(currenSelectedObject.GetSize())){
                    SetupBigCell(cells, cellRoomGO);
                } else {
                    cellRoomGO.Setup(currenSelectedObject, gridLayout.spacing, cellRoomGO.GetPosition());
                }
                cellPreviewManager.Forbidden();
            }
        }

        private void SetupBigCell(List<CellRoomGO> cells, CellRoomGO cellRoomGO){
            CellRoomGO topLeftCell = null;
            cells.ForEach(cell => {
                /*
                * Creates the sprite in the top-left cell.
                * Unity manages its rows in such a way that each row below has a higher z'index than the one above.
                * Otherwise, the image passes over the other cells and, when hovering, if it's at the bottom left, you can no longer select the cells above.
                *
                */
                if (topLeftCell == null || cell.GetPosition().x < topLeftCell.GetPosition().x || cell.GetPosition().y < topLeftCell.GetPosition().y) {
                    topLeftCell = cell;
                }
                cell.SetupDesactivatedCell(cellRoomGO, currenSelectedObject);
            });
            topLeftCell.Setup(currenSelectedObject, gridLayout.spacing, topLeftCell.GetPosition());
        }

        private void CopyCell(Element element){
            // toDO : voir la taille de la cell pour empêcher de copier si il y a un voisin, tout doit être vide !!!!
            if(element != null){
                roomUIStateManager.OnSelectObject(element);
                OnSelectButtonClick();
            }
        }

        private void DeleteCell(CellRoomGO cellRoomGO) {
            Element config = cellRoomGO.GetConfig();
            if(config == null && !cellRoomGO.IsDesactivatedCell()) return;
            Vector2Int size = config.GetSize();
            List<CellRoomGO> cells = GetCellsAtPosition(cellRoomGO.GetRootCellRoomGO(), config.GetSize());
            cells.ForEach(cell => {
                cell.ResetCell();
            });
        }

        private void OnObjectSelectedHandler(Element selectedObject) {
            currenSelectedObject = selectedObject;
            OnSelectButtonClick();
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

        private void DropdownValueChanged(string shape) {
            currentZoom = 1;
            Zoom(currentZoom);
            RoomShapeEnum? newShape = Utilities.GetEnumValueFromDropdown<RoomShapeEnum>(shape);
            if(newShape.HasValue) {
                GenerateGrid(newShape.Value);
            } else {
                currentGrid.ResetPool();
            }
        }

        private void GenerateGrid(RoomShapeEnum shape) {
            if (!initGrid) {
                currentGrid.ResetPool();
            }
            initGrid = false;
            if (roomByShape.ContainsKey(shape)) {
                Room room = roomByShape[shape];
                Vector2Int[] roomSections = room.GetSections(Vector2Int.zero);
                Vector2Int roomSize = room.GetSizeOfRoom();
                int cols = roomSize.x * (int)RoomSizeEnum.WIDTH;
                int rows = roomSize.y * (int)RoomSizeEnum.HEIGHT;
                gridLayout.constraintCount = cols;
                currentGrid = new RoomGrid(pool, roomSections, roomSize, rows, cols);
                roomGridPlane = currentGrid.RoomGridPlane;
                currentGrid.GenerateGrid(transform);
                ModifyGridLayoutRectTransform(cols, rows);
            } else {
                Debug.Log("GridManager Start, error: "+ shape);
            }
        }

        void ModifyGridLayoutRectTransform(int cols, int rows) {
            int width = (cellSize * cols) + (cellSpacing * (cols - 1));
            int height = (cellSize * rows) + (cellSpacing * (rows - 1));
            defaultWidth = width;
            defaultHeight = height;
            rectTransform.sizeDelta = new Vector2(width, height);
        }

        private void CreateRoomInstance() {
            foreach (RoomShapeEnum shape in Enum.GetValues(typeof(RoomShapeEnum))) {
                Room room = RoomFactory.GetInstance().InstantiateRoomImpl(shape);
                roomByShape.Add(shape, room);
            }
        }

    }
}