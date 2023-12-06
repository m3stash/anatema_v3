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

        // TOP / BOTTOM
        if (row == 0) {
            cell.AddWall();
            return;
        } else if (row == rows - 1) {
            cell.AddWall();
            return;
        } else if(row % sectionHeight == 0 || (row + 1) % sectionHeight == 0) {
            // Top
            if (currentSection.y + 1 < roomHeight && !FindedSection(new Vector2Int(currentSection.x, currentSection.y + 1))) {
                cell.AddWall();
                return;
            // Bottom
            } else if (currentSection.y - 1 >= 0 && !FindedSection(new Vector2Int(currentSection.x, currentSection.y - 1))) {
                cell.AddWall();
                return;
            }
        }

        // LEFT / RIGHT
        if (col == 0) {
            cell.AddWall();
            return;
        } else if (col == cols - 1) {
            cell.AddWall();
            return;
        } else if (col % sectionWidth == 0 || (col + 1) % sectionWidth == 0) {
            // Right
            if (currentSection.x + 1 < roomWidth && !FindedSection(new Vector2Int(currentSection.x + 1, currentSection.y))) {
                cell.AddWall();
                return;
            // Left
            } else if (currentSection.x - 1 >= 0 && !FindedSection(new Vector2Int(currentSection.x - 1, currentSection.y))) {
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

