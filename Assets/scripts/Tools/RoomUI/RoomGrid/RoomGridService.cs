using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RoomUI {
    public class RoomGridService {

        private GridLayoutGroup gridLayout;
        private List<GridElementModel> topLayer = new List<GridElementModel>();
        private List<GridElementModel> middleLayer = new List<GridElementModel>();
        private List<GridElementModel> bottomLayer = new List<GridElementModel>();

        public RoomGridService(GridLayoutGroup gridLayout) {
            this.gridLayout = gridLayout;
        }

        public List<GridElementModel> GetBottomLayer() {
            return bottomLayer;
        }

        public List<GridElementModel> GetTopLayer() {
            return topLayer;
        }

        public List<GridElementModel> GetMiddleLayer() {
            return middleLayer;
        }

        public void ResetLayers() {
            bottomLayer = new List<GridElementModel>();
            topLayer = new List<GridElementModel>();
            middleLayer = new List<GridElementModel>();
        }

        public List<CellRoomGO> GetCellsAtPosition(CellRoomGO cellRoomGO, Vector2Int selectedElementSize) {
            List<CellRoomGO> cells = new List<CellRoomGO>();
            Vector2Int size = selectedElementSize;
            if (IsBigCell(selectedElementSize)) {
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
                            gridLayout.transform.GetChild(targetChildIndex).GetInstanceID();
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

        // public CellRoomGO GetCellByGridLayoutIndex(int instanceID) {
        //     Transform gridTransform = gridLayout.transform;
        //     for (int i = 0; i < gridTransform.childCount; i++) {
        //         if (instanceID == gridTransform.GetChild(i).GetInstanceID()) {
        //             return gridTransform.GetChild(i).GetComponent<CellRoomGO>();
        //         }
        //     }
        //     return null;
        // }

        public CellRoomGO GetCellByIndex(int index) {
            Transform child = gridLayout.transform.GetChild(index);
            if (child) {
                return child.GetComponent<CellRoomGO>();
            }
            return null;
        }

        public int GetCellIndexByPosition(Vector2Int pos) {
            return pos.y * gridLayout.constraintCount + pos.x;
        }

        public bool IsBigCell(Vector2Int size) {
            return size.x > 1 || size.y > 1;
        }

        public bool CreateCell(CellRoomGO cellRoomGO, Element selectedElement, LayerType layer) {
            if (selectedElement == null || cellRoomGO.GetConfig(layer) != null) return false;
            Vector2Int size = selectedElement.GetSize();
            List<CellRoomGO> cells = GetCellsAtPosition(cellRoomGO, size);
            if (cells.Exists(cell => cell.GetConfig(layer) != null || cell.IsDoorOrWall() || cell.IsDesactivatedCell(layer))) {
                return false;
            }
            if (IsBigCell(size)) {
                DesactivateAllCellsAndGetTopLeftCell(cells, cellRoomGO, selectedElement, layer);
            }
            AddCellInUsedCell(selectedElement, cellRoomGO.GetPosition(), layer);
            cellRoomGO.Setup(selectedElement, layer, gridLayout.spacing, cellRoomGO.GetPosition());
            return true;
        }

        public void DesactivateAllCellsAndGetTopLeftCell(List<CellRoomGO> cells, CellRoomGO cellRoomGO, Element selectedElement, LayerType layerType) {
            cells.ForEach(cell => {
                cell.SetupBigCell(cellRoomGO.GetInstanceID(), selectedElement, layerType);
            });
        }

        public void AddCellInUsedCell(Element element, Vector2Int position, LayerType layerType) {
            if (layerType == LayerType.BOTTOM) {
                bottomLayer.Add(new GridElementModel(element.GetId(), position));
            }
            if (layerType == LayerType.MIDDLE) {
                middleLayer.Add(new GridElementModel(element.GetId(), position));
            }
            if (layerType == LayerType.TOP) {
                topLayer.Add(new GridElementModel(element.GetId(), position));
            }
        }

        public bool DeleteCell(CellRoomGO cellRoomGO, LayerType layerType) {
            Element config = cellRoomGO.GetConfig(layerType);
            if (config == null && !cellRoomGO.IsDesactivatedCell(layerType)) return false;
            bool isDeletedCell = RemoveCellInUsedCell(cellRoomGO, layerType);
            if (isDeletedCell) {
                List<CellRoomGO> cells = GetCellsAtPosition(cellRoomGO, config.GetSize());
                cells.ForEach(cell => {
                    cell.ResetLayerCell(layerType);
                });
                return true;
            }
            return false;
        }

        private bool RemoveCellInUsedCell(CellRoomGO cellRoomGO, LayerType layerType) {
            if (layerType == LayerType.BOTTOM) {
                return DeleteElement(bottomLayer, layerType, cellRoomGO);
            }
            if (layerType == LayerType.MIDDLE) {
                return DeleteElement(middleLayer, layerType, cellRoomGO);
            }
            if (layerType == LayerType.TOP) {
                return DeleteElement(topLayer, layerType, cellRoomGO);
            }
            return false;
        }

        private bool DeleteElement(List<GridElementModel> layer, LayerType layerType, CellRoomGO cellRoomGO) {
            int index = layer.FindIndex(cellConfig =>
                cellConfig.GetId() == cellRoomGO.GetConfig(layerType).GetId() &&
                cellConfig.GetPosition() == cellRoomGO.GetPosition());
            if (index != -1) {
                layer.RemoveAt(index);
                return true;
            }
            return false;
        }

    }
}
