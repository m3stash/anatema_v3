using UnityEngine;
using RoomNs;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;

namespace RoomUI {
    public class GridManager : MonoBehaviour {
        [SerializeField] private GameObject roomCellPrefab;
        [SerializeField] private GameObject cellPool;
        [SerializeField] private TMP_Dropdown roomShapeDropdown;
        [SerializeField] private Button gridZoomMinus;
        [SerializeField] private Button gridZoomPlus;
        private CellPool pool;
        private GridLayoutGroup gridLayout;
        private Dictionary<RoomShapeEnum, Room> roomBySHape = new Dictionary<RoomShapeEnum, Room>();
        private RoomGrid currentGrid;
        private RectTransform rectTransform;
        private int cellSize = 28;
        private int cellSpacing = 1;
        private int defaultWidth;
        private int defaultHeight;
        private float currentZoom = 1;
        private float zoomIncrement = 0.5f;

        private void Awake() {
            pool = cellPool.GetComponent<CellPool>();
            PoolConfig config = pool.GetConfig();
            gridLayout = gameObject.GetComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(cellSize, cellSize);
            gridLayout.spacing = new Vector2(cellSpacing, cellSpacing);
            if (!config) {
                Debug.LogError("Error no config for cellPool on GridManager awake !");
            } else {
                pool.Setup(roomCellPrefab, config.GetPoolSize());
            }
            rectTransform = gridLayout.GetComponent<RectTransform>();
            PopulateDropdown();
            CreateRoomInstance();
            CreateListeners();
        }

        private void CreateListeners() {
            roomShapeDropdown.onValueChanged.AddListener(delegate {
                DropdownValueChanged(roomShapeDropdown);
            });
            if (gridZoomMinus != null) {
                gridZoomMinus.onClick.AddListener(OnGridZoomMinusClick);
            } else {
                Debug.Log("[SerializeField] gridZoomMinus not set !");
            }
            if (gridZoomPlus != null) {
                gridZoomPlus.onClick.AddListener(OnGridZoomPlus);
            } else {
                Debug.Log("[SerializeField] gridZoomPlus not set !");
            }
            
        }

        private void OnGridZoomMinusClick() {
            currentZoom = currentZoom == 1 ? 1 : currentZoom - zoomIncrement;
            Zoom(currentZoom);
        }

        private void OnGridZoomPlus() {
            currentZoom += zoomIncrement;
            Zoom(currentZoom);
        }

        private void Zoom(float currentZoom) {
            float newWidth = currentZoom * defaultWidth;
            float newHeight = currentZoom * defaultHeight;
            gridLayout.cellSize = new Vector2(cellSize * currentZoom, cellSize * currentZoom);
            rectTransform.sizeDelta = new Vector2(newWidth, newHeight);
        }

        private void DropdownValueChanged(TMP_Dropdown change) {
            string value = change.options[change.value].text;
            RoomShapeEnum shape = GetEnumFromDropdownValue(value);
            GenerateGrid(shape, false);
        }

        private RoomShapeEnum GetEnumFromDropdownValue(string value) {
            RoomShapeEnum shapeEnumValue;
            if (Enum.TryParse(value, out shapeEnumValue)) {
                return shapeEnumValue;
            }
            Debug.Log("Unkown Enum GetEnumFromDropdownValue: " + value);
            return RoomShapeEnum.R1X1;
        }

        private void PopulateDropdown() {
            List<string> options = new List<string>(Enum.GetNames(typeof(RoomNs.RoomShapeEnum)));
            roomShapeDropdown.ClearOptions();
            roomShapeDropdown.AddOptions(options);
        }

        private void Start() {
            RoomShapeEnum shape = GetEnumFromDropdownValue(roomShapeDropdown.options[0].text);
            GenerateGrid(shape, true);
        }

        private void GenerateGrid(RoomShapeEnum shape, bool isStartCall) {
            if (!isStartCall) {
                currentGrid.ResetPool();
            }
            if (roomBySHape.ContainsKey(shape)) {
                Room room = roomBySHape[shape];
                Vector2Int[] roomSections = room.GetSections(Vector2Int.zero);
                Vector2Int roomSize = room.GetSizeOfRoom();
                int cols = roomSize.x * (int)RoomSizeEnum.WIDTH;
                int rows = roomSize.y * (int)RoomSizeEnum.HEIGHT;
                gridLayout.constraintCount = cols;
                currentGrid = new RoomGrid(pool, roomSections, roomSize, rows, cols);
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
                roomBySHape.Add(shape, room);
            }
        }

    }
}


