using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoomUI {
    public class ObjectsManager : MonoBehaviour {
        [SerializeField] private RoomStateManager roomStateManager;
        [SerializeField] private GameObject cellPoolGO;
        [SerializeField] private GameObject tabCellPoolGO;
        [SerializeField] private GameObject grid;
        [SerializeField] private GameObject scrollView;
        [SerializeField] private GameObject gridTabs;
        [SerializeField] private TabsCategoryConfig tabsCategoryConfig;
        private RectTransform rectTransformScrollView;
        private RectTransform rectTransform;
        private GridLayoutGroup gridLayout;
        private List<CellGO> usedCells = new List<CellGO>();
        private List<TabCellGO> usedTabCells = new List<TabCellGO>();
        private CellPool cellPool;
        private TabCellPool tabCellPool;
        private RectOffset padding;
        private int cellSize = 48;
        private int cellSpacing = 1;
        // private RectTransform gridTabRectTransform;
        private GridLayoutGroup gridTablGridLayout;
        private int cellTabSize = 50;
        private int cellTabSpacing = 1;
        private int constraintCount;
        private float gridWidth;
        private RoomUIManager roomUIManager;

        void Awake() {
            VerifySerialisables();
            CreateListeners();
            CreatePooling();
            InitGrid();
            CreateTabs();
            InitGridTabs();
        }

        public void Setup(RoomUIManager roomUIManager) {
            this.roomUIManager = roomUIManager;
        }

        private void InitGrid() {
            gridLayout = grid.GetComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(cellSize, cellSize);
            gridLayout.spacing = new Vector2(cellSpacing, cellSpacing);
            rectTransformScrollView = scrollView.GetComponent<RectTransform>();
            rectTransform = gridLayout.GetComponent<RectTransform>();
            padding = gridLayout.padding;
            gridWidth = rectTransformScrollView.rect.width;
            constraintCount = (int)(gridWidth / (cellSize + cellSpacing * 2));
            gridLayout.constraintCount = constraintCount;
        }

        private void InitGridTabs() {
            gridTablGridLayout = grid.GetComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(cellTabSize, cellTabSize);
            gridLayout.spacing = new Vector2(cellTabSpacing, cellTabSpacing);
        }

        private void CreatePooling() {
            CreateTabCellPooling();
            CreateObjectCellPooling();
        }

        private void CreateTabCellPooling() {
            tabCellPool = tabCellPoolGO.GetComponent<TabCellPool>();
            PoolConfig config = tabCellPool.GetConfig();
            if (!config) {
                Debug.LogError("Error no config for cellPool on GridManager awake !");
            } else {
                tabCellPool.Setup(config.GetPrefab(), config.GetPoolSize());
            }
        }


        private void CreateObjectCellPooling() {
            cellPool = cellPoolGO.GetComponent<CellPool>();
            PoolConfig config = cellPool.GetConfig();
            if (!config) {
                Debug.LogError("Error no config for cellPool on GridManager awake !");
            } else {
                cellPool.Setup(config.GetPrefab(), config.GetPoolSize());
            }
        }

        private void VerifySerialisables() {
            if (roomStateManager == null) {
                Debug.LogError("ItemGridManager SerializeField roomStateManager not set !");
            }
        }

        private void OnDestroy() {
            roomStateManager.OnBiomeChange -= DropdownBiomeChanged;
        }

        private void CreateListeners() {
            roomStateManager.OnBiomeChange += DropdownBiomeChanged;
        }

        private void DropdownBiomeChanged(string biome) {
            BiomeEnum? newBiome = Utilities.GetEnumValueFromDropdown<BiomeEnum>(biome);
            int nbItems = 10;
            if (newBiome.HasValue) {
                int nbrRows = (int)Math.Ceiling((decimal)nbItems / constraintCount);
                int height = nbrRows * (cellSize + (cellSpacing * 2)) + (padding.bottom + padding.top);
                rectTransform.sizeDelta = new Vector2(gridWidth, height);
                GenerateGrid(nbItems);
            } else {
                ResetPool();
            }
        }

        private void CreateTab(ObjectType type, bool isFirst) {
            Sprite sprite = tabsCategoryConfig.GetItemByCategory(type);
            TabCellGO tab = tabCellPool.GetOne();
            usedTabCells.Add(tab);
            tab.transform.SetParent(gridTabs.transform);
            tab.Setup(isFirst, sprite);
            GameObject cellGo = tab.gameObject;
            cellGo.SetActive(true);
        }

        private void CreateTabs() {
            bool isFirstElement = true;
            foreach (ObjectType type in Enum.GetValues(typeof(ObjectType))) {
                if(type != ObjectType.EQUIPMENT) {
                    if (isFirstElement) {
                        CreateTab(type, true);
                        isFirstElement = false;
                    } else {
                        CreateTab(type, false);
                    }
                }
            }
        }

        public void GenerateGrid(int nbItems) {
            for (int i = 0; i < nbItems; i++) {
                CellGO cell = cellPool.GetOne();
                usedCells.Add(cell);
                cell.transform.SetParent(grid.transform);
                cell.Setup();
                GameObject cellGo = cell.gameObject;
                cellGo.SetActive(true);
                // ITEM GO toDO finir ça !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                // voir à ne pas intégrer une cellule dans chaque object mais plutot un gameobject vide qui contiendrait le script et la conf associé !
                // ItemCellGO item = roomUIManager.GetItemCell();
                // item.transform.SetParent(cell.transform);
            }
        }

        public void ResetPool() {
            /*usedCells.ForEach(cell => {
                // cell.DesactivateCell();
            });*/
            cellPool.ReleaseMany(usedCells);
        }
    }
}