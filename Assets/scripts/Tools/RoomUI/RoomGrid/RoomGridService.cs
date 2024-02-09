using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RoomUI {
    public class RoomGridService {

        private GridLayoutGroup gridLayout;

        public RoomGridService(GridLayoutGroup gridLayout){
            this.gridLayout = gridLayout;
        }

        public List<CellRoomGO> GetCellsAtPosition(CellRoomGO cellRoomGO, Vector2Int selectedElementSize){
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

        public bool IsBigCell(Vector2Int size){
            return size.x > 1 || size.y > 1;
        }

        public bool CreateCell(Element cellConfig, CellRoomGO cellRoomGO, Element selectedElement){
            if(selectedElement == null || cellConfig != null) return false;
            List<CellRoomGO> cells = GetCellsAtPosition(cellRoomGO, selectedElement.GetSize());
            if(cells.Exists(cell => cell.GetConfig() != null || cell.IsDoorOrWall() || cell.IsDesactivatedCell())){
                return true;
            } else {
                if(IsBigCell(selectedElement.GetSize())){
                    SetupBigCell(cells, cellRoomGO, selectedElement);
                } else {
                    cellRoomGO.Setup(selectedElement, gridLayout.spacing, cellRoomGO.GetPosition());
                }
                return true;
            }
        }

        public void SetupBigCell(List<CellRoomGO> cells, CellRoomGO cellRoomGO, Element selectedElement){
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
            topLeftCell.Setup(selectedElement, gridLayout.spacing, topLeftCell.GetPosition());
        }

        public void DeleteCell(CellRoomGO cellRoomGO) {
            Element config = cellRoomGO.GetConfig();
            if(config == null && !cellRoomGO.IsDesactivatedCell()) return;
            Vector2Int size = config.GetSize();
            List<CellRoomGO> cells = GetCellsAtPosition(cellRoomGO.GetRootCellRoomGO(), config.GetSize());
            cells.ForEach(cell => {
                cell.ResetCell();
            });
        }

    }
}
