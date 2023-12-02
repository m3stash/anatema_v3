using RoomNs;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class RoomGrid_R2X2: RoomGrid {

    protected override int roomWidth => 2;
    protected override int roomHeight => 2;

    public RoomGrid_R2X2(RoomShapeEnum shape, CellPool pool) {
        this.shape = shape;
        this.pool = pool;
    }

    protected override void ManageCells(int col, int row, int cols, int rows, CellGO cell) {
        if (col > 20 && row < 15) {
            cell.DesactivateCell();
        }
        if (col == 0) {
            if (row == 7) {
                cell.AddDoor();
            } else if (row == rows - 8) {
                cell.AddDoor();
            } else {
                cell.AddWall();
            }
        }
        if (col == cols - 1) {
            if (row == rows - 8) {
                cell.AddDoor();
            } else {
                cell.AddWall();
            }
        }
        if (row == 0) {
            if (col == 10) {
                cell.AddDoor();
            } else {
                cell.AddWall();
            }
        }
        if (row == rows - 1) {
            if (col == 10) {
                cell.AddDoor();
            } else if (col == cols - 11) {
                cell.AddDoor();
            } else {
                cell.AddWall();
            }
        }
        if (row < 16 && col == 20) {
            if (row == 7) {
                cell.AddDoor();
            } else {
                cell.AddWall();
            }
        }
        if (col > 20 && row == 15) {
            if (col == cols - 11) {
                cell.AddDoor();
            } else {
                cell.AddWall();
            }
        }
    }
}

