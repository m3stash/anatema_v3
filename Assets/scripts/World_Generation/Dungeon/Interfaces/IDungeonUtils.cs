using System;
using UnityEngine;

public interface IDungeonUtils {
    public bool CheckIsOutOfBound(Vector2Int vector, int floorplanBound);
    public bool CheckIsInBounds(int x, int y, int bound);
    public int[][] GetDirection();
}

