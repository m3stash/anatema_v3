using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoomUI {
    public class ElementManager : MonoBehaviour {
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
        private ElementCategoryType selectedTab;
        private RoomUIManager roomUIManager;
        private Dictionary<ElementCategoryType, Dictionary<Type, Dictionary<Enum, List<Element>>>> ElementsDictionnary = new Dictionary<ElementCategoryType, Dictionary<Type, Dictionary<Enum, List<Element>>>>();

        void Awake() {
            VerifySerialisables();
            CreateListeners();
            CreatePooling();
            InitGrid();
            CreateTabs();
            InitGridTabs();
            LoadObjects();
        }

        void OnDestroy() {
            RemoveListeners();
        }

        public void Setup(RoomUIManager roomUIManager) {
            this.roomUIManager = roomUIManager;
        }

        private void RemoveListeners() {
            CellGO.OnClick -= OnCellClickHandler;
            TabCellGO.OnClick -= OnTabClickHandler;
            roomUIStateManager.OnBiomeChange -= DropdownBiomeChanged;
        }

        private void CreateListeners() {
            CellGO.OnClick += OnCellClickHandler;
            TabCellGO.OnClick += OnTabClickHandler;
            roomUIStateManager.OnBiomeChange += DropdownBiomeChanged;
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

        private void DropdownBiomeChanged(string biome) {
            BiomeEnum? newBiome = Utilities.GetEnumValueFromDropdown<BiomeEnum>(biome);
            int nbItems = 10;
            if (newBiome.HasValue) {
                int nbrRows = (int)Math.Ceiling((decimal)nbItems / constraintCount);
                int height = nbrRows * (cellSize + (cellSpacing * 2)) + (padding.bottom + padding.top);
                rectTransform.sizeDelta = new Vector2(gridWidth, height);
            } else {
                //ResetPool();
                // ToDo : refresh object categories !!
            }
        }

        private void CreateTab(ElementCategoryType type, bool isFirst) {
            Sprite sprite = tabsCategoryConfig.GetItemByCategory(type);
            TabCellGO tab = tabCellPool.GetOne();
            usedTabCells.Add(tab);
            tab.transform.SetParent(gridTabs.transform);
            // Be careful to call setup after setActive to avoid setting components to null !
            tab.Setup(isFirst, sprite, type);
            GameObject cellGo = tab.gameObject;
            cellGo.SetActive(true);
        }

        private void OnTabClickHandler(ElementCategoryType type) {
            cellPool.ReleaseMany(usedCells);
            CreateDictionnaryAndCellByElementCategoryType(type);
        }

        private void OnCellClickHandler(Element config) {
            roomUIStateManager.OnSelectObject(config);
        }

        private void CreateTabs() {
            bool isFirstElement = true;
            foreach (ElementCategoryType type in Enum.GetValues(typeof(ElementCategoryType))) {
                if(type != ElementCategoryType.EQUIPMENT) {
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
            foreach (ElementCategoryType type in Enum.GetValues(typeof(ElementCategoryType))) {
                switch (type) {
                    case ElementCategoryType.ITEM:
                    case ElementCategoryType.ENTITY:
                    case ElementCategoryType.BLOCK:
                    case ElementCategoryType.PEDESTRAL:
                    case ElementCategoryType.DECORATION:
                        CreateDictionnaryAndCellByElementCategoryType(type);
                        break;
                    case ElementCategoryType.EQUIPMENT:
                        break;
                    default:
                        Debug.LogError("LoadObjects: type not found: " + type);
                        break;
                }
            };
        }

        private void CreateDictionnaryAndCellByElementCategoryType(ElementCategoryType type) {
            string cat = type.ToString().ToLower();
            if (!ElementsDictionnary.ContainsKey(type)) {
                Element[] Elements = LoadElements(cat);
                if(Elements == null) {
                    // Debug.Log("Error: Elements is null !");
                }else{
                    BuildElementsDictionary(type, Elements);
                }
            }else{
                CreateCellWithExistingDictionnary(type);
            }
        }

        private void CreateCellWithExistingDictionnary(ElementCategoryType type) {
            foreach (var category in ElementsDictionnary[type].Keys) {
                foreach (var categoryValue in ElementsDictionnary[type][category].Keys) {
                    foreach (var config in ElementsDictionnary[type][category][categoryValue]) {
                        CreateCell(config);
                    }
                }
            }
        }

        private Element[] LoadElements(string category) {
            // toDo ICI
            return null;
            // return Resources.LoadAll<Element>(GlobalConfig.Instance.ScriptablePath + category);
        }

        private void BuildElementsDictionary(ElementCategoryType type, Element[] Elements) {
            if (!ElementsDictionnary.ContainsKey(type)) {
                ElementsDictionnary[type] = new Dictionary<Type, Dictionary<Enum, List<Element>>>();
            }
            foreach (var config in Elements) {
                Type category = config.GetSubCategory();
                Enum categoryValue = config.GetSubCategoryType<Enum>();
                EnsureNestedDictionariesExist(type, category, categoryValue);

                ElementsDictionnary[type][category][categoryValue].Add(config);
                CreateCell(config);
            }
        }

        private void EnsureNestedDictionariesExist(ElementCategoryType type, Type category, Enum categoryValue) {
            if (!ElementsDictionnary[type].ContainsKey(category)) {
                ElementsDictionnary[type][category] = new Dictionary<Enum, List<Element>>();
            }
            if (!ElementsDictionnary[type][category].ContainsKey(categoryValue)) {
                ElementsDictionnary[type][category][categoryValue] = new List<Element>();
            }
        }
        public void CreateCell(Element config) {
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