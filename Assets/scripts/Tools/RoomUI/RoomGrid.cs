using RoomNs;
using UnityEngine;

public abstract class RoomGrid {

    protected RoomShapeEnum shape;
    protected readonly int roomSizeX = (int)RoomSizeEnum.WIDTH;
    protected readonly int roomSizeY = (int)RoomSizeEnum.HEIGHT;
    protected int rows;
    protected int cols;
    protected abstract int roomWidth { get; }
    protected abstract int roomHeight { get; }
    protected abstract void ManageCells(int col, int row, int cols, int rows, CellGO cell);
    protected CellPool pool;

    public void GenerateGrid() {
        CalculateRoomSize();
        for (int row = 0; row < rows; row++) {
            for (int col = 0; col < cols; col++) {
                CellGO cell = pool.GetOne();
                cell.Setup();
                GameObject cellGo = cell.gameObject;
                cellGo.SetActive(true);
                ManageCells(col, row, cols, rows, cell);
            }
        }
    }

    protected void CalculateRoomSize() {
        rows = roomSizeY * roomHeight;
        cols = roomSizeX * roomWidth;
    }
}

