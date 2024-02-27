using System;
using System.Collections.Generic;
using UnityEngine;

public interface IFloorPlanManager {
    void SetFloorPlanValue(int x, int y, int value);
    int GetFloorPlanValue(int x, int y);
    int GetFloorPlanBound();
    HashSet<(int, int)> GetSections();
    void ResetFloorPlan();
    bool CheckIsOutOfBound(Vector2Int vector, int floorplanBound);
    bool CheckIsInBounds(int x, int y, int bound);
    int[][] GetDirection();
}

