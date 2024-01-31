using UnityEngine;
using RoomNs;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace RoomUI {
    public class RoomGridManager : MonoBehaviour {
        [SerializeField] private RoomUIStateManager roomUIStateManager;
        [SerializeField] private GameObject cellPool;
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

        private void Awake() {
            VerifySerialisables();
            CreateListeners();
            InitButtonPanel();
            CreatePooling();
            InitGrid();
            CreateRoomInstance();
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
                { "gridZoomPlus", gridZoomPlus }
            };
            foreach (var field in serializableFields) {
                if (field.Value == null) {
                    Debug.LogError($"ItemGridManager SerializeField {field.Key} not set !");
                }
            }
        }

        private void OnDestroy() {
            CellRoomGO.OnClick -= OnCellClickHandler;
            roomUIStateManager.OnShapeChange -= DropdownValueChanged;
            roomUIStateManager.OnObjectSelected -= OnObjectSelectedHandler;
            gridZoomMinus.onClick.RemoveListener(OnGridZoomMinusClick);
            gridZoomPlus.onClick.RemoveListener(OnGridZoomPlusClick);
        }

        private void CreateListeners() {
            CellRoomGO.OnPointerEnterEvent += OnCellPointerEnterHandler;
            CellRoomGO.OnPointerExitEvent += OnCellPointerExitHandler;
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

        private void OnCellPointerExitHandler(CellRoomGO cellRoomGO) {
            /*if(cellRoomGO.GetConfig() != null){
                cellRoomGO.SetDefaultColor();
            }*/
        }

        private void OnCellPointerEnterHandler(CellRoomGO cellRoomGO) {
            if(cellRoomGO.GetConfig() != null){
                switch(currentAction){
                    case RoomUIAction.COPY:
                    break;
                    case RoomUIAction.SELECT:
                        cellRoomGO.ForbidenAction();
                    break;
                    default:
                        break;
                }    
            }
        }

        private void OnTrashButtonClick() {
            currentAction = RoomUIAction.TRASH;
            ResetButtonsColor();
            ChangeButtonColor(trashButton, selectedButtonColor);
        }

        private void OnCopyButtonClick() {
            currentAction = RoomUIAction.COPY;
            ResetButtonsColor();
            ChangeButtonColor(copyButton, selectedButtonColor);
            // roomUIStateManager.OnSelectObject(null);
        }

        private void OnSelectButtonClick() {
            currentAction = RoomUIAction.SELECT;
            ResetButtonsColor();
            ChangeButtonColor(selectButton, selectedButtonColor);
        }

        private void OnCellClickHandler(CellRoomGO cellRoomGO) {
            Element element = cellRoomGO.GetConfig();
            switch(currentAction){
                case RoomUIAction.COPY:
                    CopyCell(element);
                break;
                case RoomUIAction.SELECT:
                    CreateCell(element, cellRoomGO);
                break;
                case RoomUIAction.TRASH:
                    DeleteCell(cellRoomGO);
                break;
            }
        }

        private void CreateCell(Element element, CellRoomGO cellRoomGO){
            if(currenSelectedObject == null) return;
                if(element == currenSelectedObject){
                    cellRoomGO.ForbidenAction();
                    return;
                }
                bool findExistingCell = false;
                List<CellRoomGO> cellsToDeactivate = new List<CellRoomGO>();
                Vector2Int size = currenSelectedObject.GetSize();
                bool bigCell =size.x > 1 || size.y > 1;
                if(bigCell) {
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
                                if(targetCell.GetConfig() != null){
                                    findExistingCell = true;
                                    break;
                                }
                                // roomGridPlane[targetX, targetY] = currenSelectedObject.GetId();
                                // toDo -> ajouter une tableau [,] CellRoom ou Int ??? afin de vérifier si les cells adjacentes sont bonnes ou pas
                                // ne pas oublier de gérer le fait de taper sur des cell de type wall ou autre pour l'interdir aussi
                                cellsToDeactivate.Add(targetCell);
                            }
                        }
                    }
                }
                if(!findExistingCell){
                    if(bigCell){
                        foreach (CellRoomGO cell in cellsToDeactivate) {
                            cell.DesactivateCell();
                        }
                    }   
                    cellRoomGO.Setup(currenSelectedObject, gridLayout.spacing, cellRoomGO.GetPosition());
                }else{
                    cellRoomGO.ForbidenAction();
                }
        }

        private void CopyCell(Element element){
            // toDO : voir la taille de la cell pour empêcher de copier si il y a un voisin, tout doit être vide !!!!
            if(element != null){
                roomUIStateManager.OnSelectObject(element);
                OnSelectButtonClick();
            }
        }

        private void DeleteCell(CellRoomGO cellRoomGO) {
            if(cellRoomGO.GetConfig() == null) return;
            Vector2Int size = currenSelectedObject.GetSize();
            if(size.x > 1 || size.y > 1) {
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
                            targetCell.ActivateCell();
                        }
                    }
                }
            }
            cellRoomGO.ResetCell();
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