using System;
using System.Collections.Generic;

public interface IFloorPlanManager {
    void SetFloorPlanValue(int x, int y, int value);
    int GetFloorPlanValue(int x, int y);
    int GetFloorPlanBound();
    HashSet<(int, int)> GetOccupiedCells();
    void ResetFloorPlan();
}

