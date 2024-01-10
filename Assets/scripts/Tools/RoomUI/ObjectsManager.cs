using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoomUI {
    public class ObjectsManager : MonoBehaviour {
        [SerializeField] private RoomUIStateManager roomUIStateManager;
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
        private ObjectType selectedTab;
        private RoomUIManager roomUIManager;
        private Dictionary<ObjectType, Dictionary<Type, Dictionary<Enum, List<ObjectConfig>>>> objectConfigsDictionnary = new Dictionary<ObjectType, Dictionary<Type, Dictionary<Enum, List<ObjectConfig>>>>();

        void Awake() {
            VerifySerialisables();
            CreateListeners();
            CreatePooling();
            InitGrid();
            CreateTabs();
            InitGridTabs();
            LoadObjects();
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
            if (roomUIStateManager == null) {
                Debug.LogError("ItemGridManager SerializeField roomStateManager not set !");
            }
        }

        private void OnDestroy() {
            roomUIStateManager.OnBiomeChange -= DropdownBiomeChanged;
        }

        private void CreateListeners() {
            roomUIStateManager.OnBiomeChange += DropdownBiomeChanged;
        }

        private void DropdownBiomeChanged(string biome) {
            BiomeEnum? newBiome = Utilities.GetEnumValueFromDropdown<BiomeEnum>(biome);
            int nbItems = 10;
            if (newBiome.HasValue) {
                int nbrRows = (int)Math.Ceiling((decimal)nbItems / constraintCount);
                int height = nbrRows * (cellSize + (cellSpacing * 2)) + (padding.bottom + padding.top);
                rectTransform.sizeDelta = new Vector2(gridWidth, height);
                // TODO changer le biome des objects configs !!!
            } else {
                ResetPool();
            }
        }

        private void CreateTab(ObjectType type, bool isFirst) {
            Sprite sprite = tabsCategoryConfig.GetItemByCategory(type);
            TabCellGO tab = tabCellPool.GetOne();
            usedTabCells.Add(tab);
            tab.transform.SetParent(gridTabs.transform);
            tab.Setup(isFirst, sprite, type);
            GameObject cellGo = tab.gameObject;
            tab.OnTabClicked += OnTabClickHandler;
            cellGo.SetActive(true);
        }

        private void OnTabClickHandler(ObjectType type) {
            Debug.Log("SELECTED TYPE: "+ type);
            // ToDO vider les cellules avant d'en charger des nouvelles
            CreateDictionnary(type);
        }

        private void CreateTabs() {
            bool isFirstElement = true;
            foreach (ObjectType type in Enum.GetValues(typeof(ObjectType))) {
                if(type != ObjectType.EQUIPMENT) {
                    if (isFirstElement) {
                        selectedTab = type;
                        CreateTab(type, true);
                        isFirstElement = false;
                    } else {
                        CreateTab(type, false);
                    }
                }
            }
        }

        void LoadObjects() {
            foreach (ObjectType type in Enum.GetValues(typeof(ObjectType))) {
                switch (type) {
                    case ObjectType.ITEM:
                    case ObjectType.ENTITY:
                    case ObjectType.BLOCK:
                    case ObjectType.PEDESTRAL:
                    case ObjectType.DECORATION:
                        CreateDictionnary(type);
                        break;
                    case ObjectType.EQUIPMENT:
                        break;
                    default:
                        Debug.LogError("LoadObjects: type not found: " + type);
                        break;
                }
            };
        }

        private void CreateDictionnary(ObjectType type) {
            string cat = type.ToString().ToLower();
            if (objectConfigsDictionnary.ContainsKey(type)) { return; }
            ObjectConfig[] objectConfigs = Resources.LoadAll<ObjectConfig>(GlobalConfig.Instance.ScriptablePath + cat);
            objectConfigsDictionnary[type] = new Dictionary<Type, Dictionary<Enum, List<ObjectConfig>>>();
            foreach (var config in objectConfigs) {
                Type category = config.Category();
                Enum categoryValue = config.CategoryValue<Enum>();
                if (!objectConfigsDictionnary[type].ContainsKey(category)) {
                    objectConfigsDictionnary[type][category] = new Dictionary<Enum, List<ObjectConfig>>();
                }
                if (!objectConfigsDictionnary[type][category].ContainsKey(categoryValue)) {
                    objectConfigsDictionnary[type][category][categoryValue] = new List<ObjectConfig>();
                }
                objectConfigsDictionnary[type][category][categoryValue].Add(config);
                CreateCell(config);
            }
        }

        public void CreateCell(ObjectConfig config) {
            CellGO cell = cellPool.GetOne();
            usedCells.Add(cell);
            cell.transform.SetParent(grid.transform);
            cell.Setup(config);
            GameObject cellGo = cell.gameObject;
            cellGo.SetActive(true);
        }

        public void ResetPool() {
            cellPool.ReleaseMany(usedCells);
            tabCellPool.ReleaseMany(usedTabCells);
        }
    }
}