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
            if ((col + middleSectionWidth) % sectionWidth == 0) {
                cell.AddDoor();
                return;
            }
            cell.AddWall();
            return;
        // BOTTOM
        } else if (row == rows - 1) {
            if ((col + middleSectionWidth) % sectionWidth == 0) {
                cell.AddDoor();
                return;
            }
            cell.AddWall();
            return;
        // Neighboor Section
        } else if(row % sectionHeight == 0 || (row + 1) % sectionHeight == 0) {
            // Top
            if (currentSection.y + 1 < roomHeight && !FindedSection(new Vector2Int(currentSection.x, currentSection.y + 1))) {
                if ((col + middleSectionWidth) % sectionWidth == 0) {
                    cell.AddDoor();
                    return;
                }
                cell.AddWall();
                return;
            // Bottom
            } else if (currentSection.y - 1 >= 0 && !FindedSection(new Vector2Int(currentSection.x, currentSection.y - 1))) {
                if ((col + middleSectionWidth) % sectionWidth == 0) {
                    cell.AddDoor();
                    return;
                }
                cell.AddWall();
                return;
            }
        }

        // LEFT / RIGHT
        if (col == 0) {
            if ((row + middleSectionHeight) % sectionHeight == 0) {
                cell.AddDoor();
                return;
            }
            cell.AddWall();
            return;
        } else if (col == cols - 1) {
            if ((row + middleSectionHeight) % sectionHeight == 0) {
                cell.AddDoor();
                return;
            }
            cell.AddWall();
            return;
        } else if (col % sectionWidth == 0 || (col + 1) % sectionWidth == 0) {
            // Right
            if (currentSection.x + 1 < roomWidth && !FindedSection(new Vector2Int(currentSection.x + 1, currentSection.y))) {
                if ((row + middleSectionHeight) % sectionHeight == 0) {
                    cell.AddDoor();
                    return;
                }
                cell.AddWall();
                return;
            // Left
            } else if (currentSection.x - 1 >= 0 && !FindedSection(new Vector2Int(currentSection.x - 1, currentSection.y))) {
                if ((row + middleSectionHeight) % sectionHeight == 0) {
                    cell.AddDoor();
                    return;
                }
                cell.AddWall();
                return;
            }
        }

        // Check Diagonals
        if ((col % sectionWidth == 0 || (col + 1) % sectionWidth == 0) && (row % sectionHeight == 0 || (row + 1) % sectionHeight == 0)) {
            if (!FindedSection(new Vector2Int(currentSection.x + 1, currentSection.y + 1))) {
                cell.AddWall();
                return;
            }
            if (!FindedSection(new Vector2Int(currentSection.x - 1, currentSection.y - 1))) {
                cell.AddWall();
                return;
            }
            if (!FindedSection(new Vector2Int(currentSection.x + 1, currentSection.y - 1))) {
                cell.AddWall();
                return;
            }
            if (!FindedSection(new Vector2Int(currentSection.x - 1, currentSection.y + 1))) {
                cell.AddWall();
                return;
            }
        }

    }
}

