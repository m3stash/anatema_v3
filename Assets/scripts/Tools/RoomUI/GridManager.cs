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
                pool.Setup(roomCellPrefab, config.GetPoolSize(), transform); // toDo faire le Setup une seule fois et non pas à chaque instanciaton d'un grid différent !!!
            }
        }

        private void Start() {
            GenerateGrid(RoomShapeEnum.R2X2);
        }

        public void GenerateGrid(RoomShapeEnum shape) {
            switch (shape) {
                case RoomShapeEnum.R1X1:
                    break;
                case RoomShapeEnum.R1X2:
                    break;
                case RoomShapeEnum.R2X1:
                    break;
                case RoomShapeEnum.R2X2:
                    RoomGrid grid = new RoomGrid_R2X2(RoomShapeEnum.R2X2, pool);
                    grid.GenerateGrid();
                    break;
                default:
                    Debug.LogError("GridManager: GenerateGrid, error shape not included : " + shape);
                    break;
            }
        }

    }
}


