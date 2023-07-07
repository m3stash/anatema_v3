using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRoom_shape {

    /*
     * Get index position of shape
     */
    public Vector2Int[] GetDirections(Vector2Int vector);

    /*
     * Get occuped Cell's for shape
     */
    public Vector2Int[] GetCellToVerify(Vector2Int vector);

    /*
     * Get GetNeighbors of cell
     */
    public Vector2Int[] GetNeighborsCells(Vector2Int vector);

}
