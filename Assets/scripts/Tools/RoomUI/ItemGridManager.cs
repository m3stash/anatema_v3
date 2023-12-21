using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoomUI {
    public class ItemGridManager : MonoBehaviour {
        [SerializeField] private RoomStateManager roomStateManager;
        [SerializeField] private GameObject cellPool;
        [SerializeField] private GameObject grid;
        [SerializeField] private GameObject scrollView;
        [SerializeField] private GameObject gridTabs;
        [SerializeField] private ItemCategoryConfig itemCategoryConfig;
        private ItemPool itemPool;
        private RectTransform rectTransformScrollView;
        private RectTransform rectTransform;
        private GridLayoutGroup gridLayout;
        private List<CellGO> usedCells = new List<CellGO>();
        private CellPool pool;
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
            CreateItemCellPooling();
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


        private void CreateItemCellPooling() {
            pool = cellPool.GetComponent<CellPool>();
            PoolConfig config = pool.GetConfig();
            if (!config) {
                Debug.LogError("Error no config for cellPool on GridManager awake !");
            } else {
                pool.Setup(config.GetPrefab(), config.GetPoolSize());
            }
        }

        private void GetItemPooling(ItemPool itemPool) {
            this.itemPool = itemPool;
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

        private void CreateTabs() {
            /*foreach (ElementType type in Enum.GetValues(typeof(ElementType))) {
                //toDO revoir ça !!!
                string directory = "";
                switch (type) {
                    case ElementType.BLOCK:
                        directory = "Ennemies";
                        break;
                }
                if(directory == "") {
                    Debug.LogError("ItemGridManager: directory does not exist for type: " + type);
                } else {
                    // GameObject go = Resources.Load<GameObject>(GlobalConfig.Instance.PrefabItemsPath+ "path");
                    string path = Application.dataPath + GlobalConfig.Instance.ResourcesPath + GlobalConfig.Instance.PrefabItemsPath + directory;
                    string[] prefabs = Directory.GetFiles(path);
                }
                Sprite sprite = itemCategoryConfig.GetIconByElementType(type);
                CellGO cell = pool.GetOne();
                usedCells.Add(cell);
                cell.transform.SetParent(gridTabs.transform);
                cell.Setup();
                GameObject cellGo = cell.gameObject;
                cellGo.SetActive(true);
                // ITEM GO toDO finir ça !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //ItemCellGO item = roomUIManager.GetItemCell();
                //item.transform.SetParent(cell.transform);
            }*/
        }

        public void GenerateGrid(int nbItems) {
            for (int i = 0; i < nbItems; i++) {
                CellGO cell = pool.GetOne();
                usedCells.Add(cell);
                cell.transform.SetParent(grid.transform);
                cell.Setup();
                GameObject cellGo = cell.gameObject;
                cellGo.SetActive(true);
                // ITEM GO toDO finir ça !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                // ItemCellGO item = roomUIManager.GetItemCell();
                // item.transform.SetParent(cell.transform);
            }
        }

        public void ResetPool() {
            /*usedCells.ForEach(cell => {
                // cell.DesactivateCell();
            });*/
            pool.ReleaseMany(usedCells);
        }
    }
}