﻿using UnityEngine;
using RoomNs;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace RoomUI {
    public class RoomGridManager : MonoBehaviour {
        [SerializeField] private RoomStateManager roomStateManager;
        [SerializeField] private GameObject roomCellPrefab;
        [SerializeField] private GameObject cellPool;
        [SerializeField] private Button gridZoomMinus;
        [SerializeField] private Button gridZoomPlus;
        private CellPool pool;
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

        private void Awake() {
            VerifySerialisables();
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
            CreateRoomInstance();
            CreateListeners();
        }

        private void VerifySerialisables() {
            Dictionary<string, object> serializableFields = new Dictionary<string, object> {
                { "roomStateManager", roomStateManager },
                { "roomCellPrefab", roomCellPrefab },
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
            roomStateManager.OnShapeChange -= DropdownValueChanged;
        }

        private void CreateListeners() {
            roomStateManager.OnShapeChange += DropdownValueChanged;

            if (gridZoomMinus != null) {
                gridZoomMinus.onClick.AddListener(OnGridZoomMinusClick);
            }
            if (gridZoomPlus != null) {
                gridZoomPlus.onClick.AddListener(OnGridZoomPlus);
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


