using System.Collections.Generic;
using UnityEngine;
namespace RoomUI {
    public class RoomGrid {

        private CellPool pool;
        private int rows; // Height
        private int cols; // Width
        private int sectionWidth;
        private int sectionHeight;
        private Vector2Int[] roomSections;
        private Vector2Int roomSize;
        private List<CellGO> usedCells = new List<CellGO>();

        public RoomGrid(CellPool pool, Vector2Int[] roomSections, Vector2Int roomSize, int rows, int cols) {
            this.pool = pool;
            this.roomSections = roomSections;
            this.roomSize = roomSize;
            this.rows = rows;
            this.cols = cols;
            sectionWidth = cols / roomSize.x;
            sectionHeight = rows / roomSize.y;
        }

        public void ResetPool() {
            usedCells.ForEach(cell => {
                cell.DesactivateCell();
            });
            pool.ReleaseMany(usedCells);
        }

        public void GenerateGrid(Transform transform) {
            for (int row = 0; row < rows; row++) {
                for (int col = 0; col < cols; col++) {
                    CellGO cell = pool.GetOne();
                    usedCells.Add(cell);
                    cell.transform.SetParent(transform);
                    cell.Setup();
                    GameObject cellGo = cell.gameObject;
                    cellGo.SetActive(true);
                    ManageCells(col, row, cell);
                }
            }
        }

        private bool FindedSection(Vector2Int currentSection) {
            bool findedSection = false;
            foreach (Vector2Int section in roomSections) {
                if (section == currentSection) {
                    findedSection = true;
                }
            }
            return findedSection;
        }

        // use for special shapes (eg: L shape)
        private Vector2Int GetCurrentSection(int col, int row) {
            int invertedRow = rows - row - 1; // to match the position (0,0) of an int[,] and the position (0,0) of the gridLayout (row / cols)
            int sectionRow = invertedRow / sectionHeight;
            int sectionCol = col / sectionWidth;
            return new Vector2Int(sectionCol, sectionRow);
        }

        private void ManageCells(int col, int row, CellGO cell) {
            Vector2Int currentSection = GetCurrentSection(col, row);

            if (!FindedSection(currentSection)) {
                cell.DesactivateCell();
                return;
            }

            int middleSectionWidth = (sectionWidth / 2) + 1;
            bool isSectionBoundaryY = row % sectionHeight == 0 || (row + 1) % sectionHeight == 0;

            // TOP
            if (row == 0) {
                AddDoorOrWall(col, middleSectionWidth, sectionWidth, cell);
                // BOTTOM
            } else if (row == rows - 1) {
                AddDoorOrWall(col, middleSectionWidth, sectionWidth, cell);
                // Neighboor Section
            } else if (isSectionBoundaryY) {
                // NS Top
                if (currentSection.y + 1 < roomSize.y && !FindedSection(new Vector2Int(currentSection.x, currentSection.y + 1))) {
                    AddDoorOrWall(col, middleSectionWidth, sectionWidth, cell);
                    // NS Bottom
                } else if (currentSection.y - 1 >= 0 && !FindedSection(new Vector2Int(currentSection.x, currentSection.y - 1))) {
                    AddDoorOrWall(col, middleSectionWidth, sectionWidth, cell);
                }
            }

            int middleSectionHeight = (sectionHeight / 2) + 1;
            bool isSectionBoundaryX = col % sectionWidth == 0 || (col + 1) % sectionWidth == 0;

            // LEFT
            if (col == 0) {
                AddDoorOrWall(row, middleSectionHeight, sectionHeight, cell);
                // RIGHT
            } else if (col == cols - 1) {
                AddDoorOrWall(row, middleSectionHeight, sectionHeight, cell);
                // Neighboor Section
            } else if (isSectionBoundaryX) {
                // NS Right
                if (currentSection.x + 1 < roomSize.x && !FindedSection(new Vector2Int(currentSection.x + 1, currentSection.y))) {
                    AddDoorOrWall(row, middleSectionHeight, sectionHeight, cell);
                    // NS Left
                } else if (currentSection.x - 1 >= 0 && !FindedSection(new Vector2Int(currentSection.x - 1, currentSection.y))) {
                    AddDoorOrWall(row, middleSectionHeight, sectionHeight, cell);
                }
            }

            // Check Diagonals for each sections
            if (isSectionBoundaryX && isSectionBoundaryY) {
                Vector2Int TR = new Vector2Int(currentSection.x + 1, currentSection.y + 1);
                if (TR.x < roomSize.x && TR.y < roomSize.y) {
                    if (!FindedSection(TR)) {
                        cell.AddWall();
                        return;
                    }
                }
                Vector2Int BL = new Vector2Int(currentSection.x - 1, currentSection.y - 1);
                if (BL.x >= 0 && BL.y >= 0) {
                    if (!FindedSection(BL)) {
                        cell.AddWall();
                        return;
                    }
                }
                Vector2Int BR = new Vector2Int(currentSection.x + 1, currentSection.y - 1);
                if (BR.x < roomSize.x && BR.y >= 0) {
                    if (!FindedSection(BR)) {
                        cell.AddWall();
                        return;
                    }
                }
                Vector2Int TL = new Vector2Int(currentSection.x - 1, currentSection.y + 1);
                if (TL.x >= 0 && TL.y < roomSize.y) {
                    if (!FindedSection(TL)) {
                        cell.AddWall();
                        return;
                    }
                }
            }

        }

        private void AddDoorOrWall(int pos, int middleSection, int sectionSize, CellGO cell) {
            if ((pos + middleSection) % sectionSize == 0) {
                cell.AddDoor();
                return;
            }
            cell.AddWall();
            return;
        }

    }

}