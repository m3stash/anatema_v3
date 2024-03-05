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

        public bool IsBigCell(Vector2Int size) {
            return size.x > 1 || size.y > 1;
        }

        public CellRoomGO CreateCell(CellRoomGO cellRoomGO, Element selectedElement, LayerType layerType) {
            if (selectedElement == null || cellRoomGO.GetConfig(layerType) != null) return null;
            List<CellRoomGO> cells = GetCellsAtPosition(cellRoomGO, selectedElement.GetSize());
            if (cells.Exists(cell => cell.GetConfig(layerType) != null || cell.IsDoorOrWall() || cell.IsDesactivatedCell(layerType))) {
                return null;
            }
            else {
                CellRoomGO cell = null;
                if (IsBigCell(selectedElement.GetSize())) {
                    cell = DesactivateAllCellsAndGetTopLeftCell(cells, cellRoomGO, selectedElement, layerType);
                }
                else {
                    cell = cellRoomGO;
                }
                AddCellInUsedCell(selectedElement, cellRoomGO.GetPosition(), layerType);
                cell.Setup(selectedElement, layerType, gridLayout.spacing, cellRoomGO.GetPosition());
                return cellRoomGO;
            }
        }

        public CellRoomGO DesactivateAllCellsAndGetTopLeftCell(List<CellRoomGO> cells, CellRoomGO cellRoomGO, Element selectedElement, LayerType layerType) {
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
                cell.SetupDesactivatedCell(cellRoomGO, selectedElement, layerType);
            });
            return topLeftCell;
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

        // public CellRoomGO DeleteCell(CellRoomGO cellRoomGO, LayerType layerType) {
        //     Element config = cellRoomGO.GetConfig(layerType);
        //     if (config == null && !cellRoomGO.IsDesactivatedCell(layerType)) return null;
        //     CellRoomGO deletedCell = RemoveCellInUsedCell(cellRoomGO.GetRootCellRoomGO(layerType), layerType);
        //     if (deletedCell) {
        //         Vector2Int size = config.GetSize();
        //         List<CellRoomGO> cells = GetCellsAtPosition(cellRoomGO.GetRootCellRoomGO(layerType), config.GetSize());
        //         cells.ForEach(cell => {
        //             cell.ResetCell(layerType);
        //         });
        //         return deletedCell;
        //     }
        //     return null;
        // }

        public CellRoomGO DeleteCell(CellRoomGO cellRoomGO, LayerType layerType) {
            Element config = cellRoomGO.GetConfig(layerType);
            if (config == null && !cellRoomGO.IsDesactivatedCell(layerType)) return null;
            CellRoomGO deletedCell = RemoveCellInUsedCell(cellRoomGO.GetRootCellRoomGO(layerType), layerType);
            if (deletedCell) {
                Vector2Int size = config.GetSize();
                List<CellRoomGO> cells = GetCellsAtPosition(cellRoomGO.GetRootCellRoomGO(layerType), config.GetSize());
                cells.ForEach(cell => {
                    cell.ResetCell(layerType);
                });
                return deletedCell;
            }
            return null;
        }

        private CellRoomGO RemoveCellInUsedCell(CellRoomGO cellRoomGO, LayerType layerType) {
            if (layerType == LayerType.BOTTOM) {
                return DeleteElement(bottomLayer, layerType, cellRoomGO);
            }
            if (layerType == LayerType.MIDDLE) {
                return DeleteElement(middleLayer, layerType, cellRoomGO);
            }
            if (layerType == LayerType.TOP) {
                return DeleteElement(topLayer, layerType, cellRoomGO);
            }
            return null;
        }

        private CellRoomGO DeleteElement(List<GridElementModel> layer, LayerType layerType, CellRoomGO cellRoomGO) {
            int index = layer.FindIndex(cellConfig =>
                cellConfig.GetId() == cellRoomGO.GetConfig(layerType).GetId() &&
                cellConfig.GetPosition() == cellRoomGO.GetPosition());
            if (index != -1) {
                layer.RemoveAt(index);
                return cellRoomGO;
            }
            return null;
        }

    }
}
