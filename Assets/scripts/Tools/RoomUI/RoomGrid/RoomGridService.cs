using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RoomUI {
    public class RoomGridService {

        private GridLayoutGroup gridLayout;
        private List<GridElementModel> topLayer = new List<GridElementModel>();
        private List<GridElementModel> groundLayer = new List<GridElementModel>();

        public RoomGridService(GridLayoutGroup gridLayout) {
            this.gridLayout = gridLayout;
        }

        public List<GridElementModel> GetGroundLayer() {
            return groundLayer;
        }

        public List<GridElementModel> GetTopLayer() {
            return topLayer;
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

        public bool CreateCell(Element cellConfig, CellRoomGO cellRoomGO, Element selectedElement) {
            if (selectedElement == null || cellConfig != null) return false;
            List<CellRoomGO> cells = GetCellsAtPosition(cellRoomGO, selectedElement.GetSize());
            if (cells.Exists(cell => cell.GetConfig() != null || cell.IsDoorOrWall() || cell.IsDesactivatedCell())) {
                return true;
            }
            else {
                CellRoomGO cell = cellRoomGO;
                if (IsBigCell(selectedElement.GetSize())) {
                    cell = SetupBigCell(cells, cellRoomGO, selectedElement);
                }
                AddCellInUsedCell(selectedElement, cell.GetPosition());
                cell.Setup(selectedElement, gridLayout.spacing, cell.GetPosition());
                return true;
            }
        }

        public CellRoomGO SetupBigCell(List<CellRoomGO> cells, CellRoomGO cellRoomGO, Element selectedElement) {
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
                cell.SetupDesactivatedCell(cellRoomGO, selectedElement);
            });
            return topLeftCell;
        }

        public void AddCellInUsedCell(Element element, Vector2Int position) {
            topLayer.Add(new GridElementModel(element.GetId(), element.GeElementId(), position));
        }

        public void DeleteCell(CellRoomGO cellRoomGO) {
            Element config = cellRoomGO.GetConfig();
            if (config == null && !cellRoomGO.IsDesactivatedCell()) return;
            RemoveCellInUsedCell(cellRoomGO.GetRootCellRoomGO());
            Vector2Int size = config.GetSize();
            List<CellRoomGO> cells = GetCellsAtPosition(cellRoomGO.GetRootCellRoomGO(), config.GetSize());
            cells.ForEach(cell => {
                cell.ResetCell();
            });
        }

        private void RemoveCellInUsedCell(CellRoomGO cellRoomGO) {
            topLayer.RemoveAt(topLayer.FindIndex(cellConfig =>
                cellConfig.GetId() == cellRoomGO.GetConfig().GetId() &&
                cellConfig.GetElementId() == cellRoomGO.GetConfig().GeElementId() &&
                cellConfig.GetPosition() == cellRoomGO.GetPosition()));
        }

        public List<GridElementModel> GetUsedCells() {
            return topLayer;
        }

    }
}
