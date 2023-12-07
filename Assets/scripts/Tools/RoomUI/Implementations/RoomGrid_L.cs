using RoomNs;
using UnityEngine;

public class RoomGrid_L: RoomGrid {

    protected override int roomWidth => 2;
    protected override int roomHeight => 2;
    protected override Vector2Int[] roomSections => new Room_L().GetOccupiedCells(Vector2Int.zero);

    public RoomGrid_L(RoomShapeEnum shape, CellPool pool) {
        this.shape = shape;
        this.pool = pool;
    }

    protected override void ManageCells(int col, int row, CellGO cell) {
        //valuesByCoordinates.Add(Tuple.Create(row, col), currentId);

        Vector2Int currentSection = GetCurrentSection(col, row);

        if (!FindedSection(currentSection)) {
            cell.DesactivateCell();
            return;
        }

        int middleSectionWidth = (sectionWidth / 2) + 1;
        int middleSectionHeight = (sectionHeight / 2) + 1;

        // TOP
        if (row == 0) {
            AddDoorOrWall(col, middleSectionWidth, sectionWidth, cell);
        // BOTTOM
        } else if (row == rows - 1) {
            AddDoorOrWall(col, middleSectionWidth, sectionWidth, cell);
            // Neighboor Section
        } else if(row % sectionHeight == 0 || (row + 1) % sectionHeight == 0) {
            // Top
            if (currentSection.y + 1 < roomHeight && !FindedSection(new Vector2Int(currentSection.x, currentSection.y + 1))) {
                AddDoorOrWall(col, middleSectionWidth, sectionWidth, cell);
                // Bottom
            } else if (currentSection.y - 1 >= 0 && !FindedSection(new Vector2Int(currentSection.x, currentSection.y - 1))) {
                AddDoorOrWall(col, middleSectionWidth, sectionWidth, cell);
            }
        }

        // LEFT / RIGHT
        if (col == 0) {
            AddDoorOrWall(row, middleSectionHeight, sectionHeight, cell);
        } else if (col == cols - 1) {
            AddDoorOrWall(row, middleSectionHeight, sectionHeight, cell);
        } else if (col % sectionWidth == 0 || (col + 1) % sectionWidth == 0) {
            // Right
            if (currentSection.x + 1 < roomWidth && !FindedSection(new Vector2Int(currentSection.x + 1, currentSection.y))) {
                AddDoorOrWall(row, middleSectionHeight, sectionHeight, cell);
                // Left
            } else if (currentSection.x - 1 >= 0 && !FindedSection(new Vector2Int(currentSection.x - 1, currentSection.y))) {
                AddDoorOrWall(row, middleSectionHeight, sectionHeight, cell);
            }
        }

        // Check Diagonals
        if ((col % sectionWidth == 0 || (col + 1) % sectionWidth == 0) && (row % sectionHeight == 0 || (row + 1) % sectionHeight == 0)) {
            Vector2Int TR = new Vector2Int(currentSection.x + 1, currentSection.y + 1);
            if(TR.x < roomWidth && TR.y < roomHeight) {
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
            if (BR.x < roomWidth && BR.y >= 0) {
                if (!FindedSection(BR)) {
                    cell.AddWall();
                    return;
                }
            }
            Vector2Int TL = new Vector2Int(currentSection.x - 1, currentSection.y + 1);
            if (TL.x >= 0 && TL.y < roomHeight) {
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

