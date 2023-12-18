using System;
using RoomNs;
using UnityEngine;
using UnityEngine.UI;

namespace RoomUI {
    public class ItemGridManager : MonoBehaviour {
        [SerializeField] private RoomStateManager roomStateManager;
        [SerializeField] private GameObject cellPool;
        [SerializeField] private GameObject grid;
        private GridLayoutGroup gridLayout;
        private CellPool pool;
        private int cellSize = 48;
        private int cellSpacing = 1;

        void Awake() {
            VerifySerialisables();
            pool = cellPool.GetComponent<CellPool>();
            gridLayout = gameObject.GetComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(cellSize, cellSize);
            gridLayout.spacing = new Vector2(cellSpacing, cellSpacing);
            CreateTabs();
        }

        private void VerifySerialisables() {
            if (roomStateManager == null) {
                Debug.LogError("ItemGridManager SerializeField roomStateManager not set !");
            }
        }

        private void OnDestroy() {
            roomStateManager.OnShapeChange -= DropdownBiomeChanged;
        }

        private void CreateListeners() {
            roomStateManager.OnShapeChange += DropdownBiomeChanged;
        }

        private void DropdownBiomeChanged(string biome) {
            RoomShapeEnum? newBiome = Utilities.GetEnumValueFromDropdown<RoomShapeEnum>(biome);
            if (newBiome.HasValue) {
                //
            } else {
                
            }
        }

        private void CreateTabs() {
            foreach (ItemType item in Enum.GetValues(typeof(ItemType))) {
                
            }
        }

        private void CreateGrid() {

        }
    }
}