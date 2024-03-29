﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace RoomUI {
    public class TabGridManager : MonoBehaviour {
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
        private string currentCategory;
        private Dictionary<string, Dictionary<string, Dictionary<string, List<Element>>>> ElementsDictionnary = new Dictionary<string, Dictionary<string, Dictionary<string, List<Element>>>>();
        private ElementTable elementTable;
        private SpriteLoader spriteLoader;
        private List<string> categories;

        void OnDestroy() {
            RemoveListeners();
        }

        public void Setup(ElementTable elementTable, List<string> categories, SpriteLoader spriteLoader) {
            this.elementTable = elementTable;
            this.spriteLoader = spriteLoader;
            this.categories = categories;
            VerifySerialisables();
            CreateListeners();
            CreatePooling();
            InitGrid();
            CreateTabs();
            InitGridTabs();
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
            // remove pooling for tabs is not necessary !
            tabCellPool = tabCellPoolGO.GetComponent<TabCellPool>();
            PoolConfig config = tabCellPool.GetConfig();
            if (!config) {
                Debug.LogError("Error no config for cellPool on GridManager awake !");
            }
            else {
                tabCellPool.Setup(config.GetPrefab(), config.GetPoolSize());
            }
        }

        private void CreateObjectCellPooling() {
            cellPool = cellPoolGO.GetComponent<CellPool>();
            PoolConfig config = cellPool.GetConfig();
            if (!config) {
                Debug.LogError("Error no config for cellPool on GridManager awake !");
            }
            else {
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
                int height = nbrRows * (cellSize + (cellSpacing * 2)) + padding.bottom + padding.top;
                rectTransform.sizeDelta = new Vector2(gridWidth, height);
            }
            else {
                //ResetPool();
                // ToDo : refresh object categories !!
            }
        }

        private void OnTabClickHandler(string category) {
            if (currentCategory == category) {
                return;
            }
            currentCategory = category;
            cellPool.ReleaseMany(usedCells);
            LoadElements(currentCategory);
        }

        private void OnCellClickHandler(Element config) {
            roomUIStateManager.OnSelectObject(config);
        }

        private void CreateTabs() {
            bool isFirstElement = true;
            foreach (string category in categories) {
                if (category != ElementCategoryType.EQUIPMENT.ToString()) {
                    if (isFirstElement) {
                        currentCategory = category;
                        CreateTab(category, true);
                        isFirstElement = false;
                        LoadElements(category);
                    }
                    else {
                        CreateTab(category, false);
                    }
                }
            }
        }

        private void CreateTab(string type, bool isFirst) {
            Sprite sprite = tabsCategoryConfig.GetItemByCategory(type);
            TabCellGO tab = tabCellPool.GetOne();
            usedTabCells.Add(tab);
            tab.transform.SetParent(gridTabs.transform);
            // Be careful to call setup after setActive to avoid setting components to null !
            tab.Setup(isFirst, sprite, type);
            GameObject cellGo = tab.gameObject;
            cellGo.SetActive(true);
        }

        void LoadElements(string category) {
            switch (category) {
                case "EQUIPMENT":
                    break;
                default:
                    CreateDictionnaryAndCellByElementCategoryType(category);
                    break;
            }
        }

        private void CreateDictionnaryAndCellByElementCategoryType(string category) {
            if (!ElementsDictionnary.ContainsKey(category)) {
                List<Element> elements = elementTable.GetElementsByCategory(category);
                if (elements == null) {
                    Debug.Log($"Error: No Element For This category {category} !");
                }
                else {
                    BuildElementsDictionary(category, elements);
                }
            }
            else {
                CreateCellWithExistingDictionnary(category);
            }
        }

        private void CreateCellWithExistingDictionnary(string category) {
            foreach (var subCategory in ElementsDictionnary[category].Keys) {
                foreach (var categoryValue in ElementsDictionnary[category][subCategory].Keys) {
                    foreach (var config in ElementsDictionnary[category][subCategory][categoryValue]) {
                        CreateCell(config, category);
                    }
                }
            }
        }

        private void BuildElementsDictionary(string category, List<Element> elements) {
            ElementsDictionnary[category] = new Dictionary<string, Dictionary<string, List<Element>>>();
            foreach (var element in elements) {
                string subCategory = element.GetSubCategory();
                string groupType = element.GetGroupType();
                EnsureNestedDictionariesExist(category, subCategory, groupType);
                ElementsDictionnary[category][subCategory][groupType].Add(element);
                CreateCell(element, category);
            }
        }

        private void EnsureNestedDictionariesExist(string category, string subCategory, string groupType) {
            if (!ElementsDictionnary[category].ContainsKey(subCategory)) {
                ElementsDictionnary[category][subCategory] = new Dictionary<string, List<Element>>();
            }
            if (!ElementsDictionnary[category][subCategory].ContainsKey(groupType)) {
                ElementsDictionnary[category][subCategory][groupType] = new List<Element>();
            }
        }
        public void CreateCell(Element element, string category) {
            Sprite sprite = spriteLoader.GetSprite(category, element.GetSpriteName());
            element.SetSprite(sprite);
            CellGO cell = cellPool.GetOne();
            usedCells.Add(cell);
            cell.transform.SetParent(grid.transform);
            cell.Setup(element);
            GameObject cellGo = cell.gameObject;
            cellGo.SetActive(true);
        }

        public void ResetPool() {
            cellPool.ReleaseMany(usedCells);
            tabCellPool.ReleaseMany(usedTabCells);
        }
    }
}