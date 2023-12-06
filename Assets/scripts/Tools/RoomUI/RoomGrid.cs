using RoomNs;
using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class RoomGrid {

    protected RoomShapeEnum shape;
    protected readonly int roomSizeX = (int)RoomSizeEnum.WIDTH;
    protected readonly int roomSizeY = (int)RoomSizeEnum.HEIGHT;
    protected int rows; // Height
    protected int cols; // Width
    protected int sectionWidth;
    protected int sectionHeight; // Width
    protected abstract Vector2Int[] roomSections { get; }
    protected abstract int roomWidth { get; }
    protected abstract int roomHeight { get; }
    protected abstract void ManageCells(int col, int row, CellGO cell);
    protected Dictionary<Tuple<int, int>, bool> excludeValues = new Dictionary<Tuple<int, int>, bool>();
    protected CellPool pool;

    public void GenerateGrid() {
        CalculateRoomSize();

        /*for (int row = 0; row < rows; row++) {
            for (int col = 0; col < cols; col++) {
                int invertedRow = rows - row - 1; // to match the position (0,0) of an int[,] and the position (0,0) of the gridLayout (row / cols)
                int sectionRow = invertedRow / sectionHeight;
                int sectionCol = col / sectionWidth;
                if (!FindedSection(new Vector2Int(sectionCol, sectionRow))) {
                    excludeValues.Add(Tuple.Create(row, col), true);
                }
            }
        }*/
        // TODO chercher un putain de moyen de faire ça 
        for (int row = 0; row < rows; row++) {
            for (int col = 0; col < cols; col++) {
                CellGO cell = pool.GetOne();
                cell.Setup();
                GameObject cellGo = cell.gameObject;
                cellGo.SetActive(true);
                ManageCells(col, row, cell);
            }
        }
    }

    protected bool FindedSection(Vector2Int currentSection) {
        bool findedSection = false;
        foreach (Vector2Int section in roomSections) {
            if (section == currentSection) {
                findedSection = true;
            }
        }
        return findedSection;
    }

    // use for special shapes (eg: L shape)
    protected Vector2Int GetCurrentSection(int col, int row) {
        int invertedRow = rows - row - 1; // to match the position (0,0) of an int[,] and the position (0,0) of the gridLayout (row / cols)
        int sectionRow = invertedRow / sectionHeight;
        int sectionCol = col / sectionWidth;
        return new Vector2Int(sectionCol, sectionRow);
    }

    protected void CalculateRoomSize() {
        rows = roomSizeY * roomHeight;
        cols = roomSizeX * roomWidth;
        sectionWidth = cols / 2;
        sectionHeight = rows / 2;

    }
}

