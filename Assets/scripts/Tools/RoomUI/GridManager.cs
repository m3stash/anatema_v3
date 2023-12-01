using System;
using UnityEngine;
using RoomNs;
using UnityEngine.UI;

namespace RoomUI {
    public class GridManager : MonoBehaviour {
        [SerializeField] private GameObject roomCellPrefab;
        [SerializeField] private GameObject cellPool;
        private CellPool pool;
        private GridLayoutGroup gridLayout;

        private void Awake() {
            pool = cellPool.GetComponent<CellPool>();
            PoolConfig config = pool.GetConfig();
            gridLayout = gameObject.GetComponent<GridLayoutGroup>();
            if (!config) {
                Debug.LogError("Error no config for cellPool on GridManager awake !");
            } else {
                pool.Setup(roomCellPrefab, config.GetPoolSize(), transform);
            }
        }

        void Start() {
            GenerateGrid();
        }

        void GenerateGrid() {
            /*for (var i = 0; i < 1260; i++) {
                CellGO cell = pool.GetOne();
                GameObject cellGo = cell.gameObject;
                cellGo.SetActive(true);
                if (i == 12) {
                    cell.DesactivateCell();
                }
            }*/

            int rows = 30;
            int cols = 42;

            for (int row = 0; row < rows; row++) {
                for (int col = 0; col < cols; col++) {
                    CellGO cell = pool.GetOne();
                    cell.Setup();
                    GameObject cellGo = cell.gameObject;
                    cellGo.SetActive(true);
                    if (col > 20 && row < 15) {
                        cell.DesactivateCell();
                    }
                    if(col == 0) {
                        if (row == 7) {
                            cell.AddDoor();
                        } else if (row == rows - 8) {
                            cell.AddDoor();
                        } else {
                            cell.AddWall();
                        }
                    }
                    if(col == cols - 1) {
                        if (row == rows - 8) {
                            cell.AddDoor();
                        } else {
                            cell.AddWall();
                        }
                    }
                    if(row == 0) {
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
                    if(row < 16 && col == 20) {
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
        }

    }
}


