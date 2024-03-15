using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
namespace RoomUI {
    public class RoomGridService {

        private GridLayoutGroup gridLayout;
        private List<GridElementModel> topLayer = new List<GridElementModel>();
        private List<GridElementModel> middleLayer = new List<GridElementModel>();
        private List<GridElementModel> bottomLayer = new List<GridElementModel>();
        Dictionary<string, Sprite[]> spriteDictionary = new Dictionary<string, Sprite[]>();

        public RoomGridService(GridLayoutGroup gridLayout) {
            this.gridLayout = gridLayout;
        }

        public List<GridElementModel> GetBottomLayer() {
            return bottomLayer;
        }

        public List<GridElementModel> GetTopLayer() {
            return topLayer;
        }

        public List<GridElementModel> GetMiddleLayer() {
            return middleLayer;
        }

        public void ResetLayers() {
            bottomLayer = new List<GridElementModel>();
            topLayer = new List<GridElementModel>();
            middleLayer = new List<GridElementModel>();
        }

        public List<CellRoomGO> GetCellsAtPosition(CellRoomGO cellRoomGO, Vector2Int selectedElementSize) {
            List<CellRoomGO> cells = new List<CellRoomGO>();
            Vector2Int size = selectedElementSize;
            if (IsBigCell(selectedElementSize)) {
                Vector2Int position = cellRoomGO.GetPosition();
                int x = position.x;
                int y = position.y;
                int gridSizeX = gridLayout.constraintCount;
                for (int yOffset = 0; yOffset < size.y; yOffset++) {
                    for (int xOffset = 0; xOffset < size.x; xOffset++) {
                        int targetX = x + xOffset;
                        int targetY = y - yOffset;
                        int targetChildIndex = targetY * gridSizeX + targetX;
                        if (targetChildIndex >= 0 && targetChildIndex < gridLayout.transform.childCount) {
                            gridLayout.transform.GetChild(targetChildIndex).GetInstanceID();
                            CellRoomGO targetCell = gridLayout.transform.GetChild(targetChildIndex).GetComponent<CellRoomGO>();
                            cells.Add(targetCell);
                        }
                    }
                }
                return cells;
            }
            cells.Add(cellRoomGO);
            return cells;
        }

        public CellRoomGO GetCellByIndex(int index) {
            Transform child = gridLayout.transform.GetChild(index);
            if (child) {
                return child.GetComponent<CellRoomGO>();
            }
            return null;
        }

        public int GetCellIndexByPosition(Vector2Int pos) {
            return pos.y * gridLayout.constraintCount + pos.x;
        }

        public bool IsBigCell(Vector2Int size) {
            return size.x > 1 || size.y > 1;
        }

        public List<CellRoomGO> CreateCell(CellRoomGO cellRoomGO, Element selectedElement, LayerType layer) {
            if (selectedElement == null || cellRoomGO.GetConfig(layer) != null) return null;
            Vector2Int size = selectedElement.GetSize();
            List<CellRoomGO> cells = GetCellsAtPosition(cellRoomGO, size);
            if (cells.Exists(cell => cell.GetConfig(layer) != null || cell.IsDoorOrWall() || cell.IsDesactivatedCell(layer))) {
                return null;
            }
            if (IsBigCell(size)) {
                SetupBigCell(cells, cellRoomGO, selectedElement, layer);
            }
            AddCellInUsedCell(selectedElement, cellRoomGO.GetPosition(), layer);
            cellRoomGO.Setup(selectedElement, layer, gridLayout.spacing, cellRoomGO.GetPosition());
            return cells;
        }

        private Sprite[] GetSprites(Element element) {
            string name = element.GetSprite().name;
            if (spriteDictionary.ContainsKey(name)) {
                return spriteDictionary[name];
            }
            Sprite sprite = element.GetSprite();
            Texture2D texture = sprite.texture;
            Vector2Int size = element.GetSize();
            RectInt[] cellRects = CalculateCellRects(texture, size);
            Sprite[] sprites = new Sprite[cellRects.Length];
            texture.Apply(false);
            foreach (RectInt cellRect in cellRects) {
                int cellWidth = texture.width / size.x;
                int cellHeight = texture.height / size.y;
                Color[] pixels = texture.GetPixels(cellRect.x, cellRect.y, cellWidth, cellHeight);
                Texture2D newTexture = new Texture2D(cellWidth, cellHeight);
                newTexture.SetPixels(pixels);
                newTexture.Apply();
                Sprite splitedSprite = Sprite.Create(newTexture, new Rect(0, 0, cellWidth, cellHeight), Vector2.one * 0.5f);
                int index = System.Array.IndexOf(cellRects, cellRect);
                sprites[index] = splitedSprite;
            }
            spriteDictionary.Add(name, sprites);
            return sprites;
        }

        RectInt[] CalculateCellRects(Texture2D texture, Vector2Int size) {
            int cellWidth = texture.width / size.x;
            int cellHeight = texture.height / size.y;
            RectInt[] cellRects = new RectInt[size.x * size.y];
            int index = 0;
            for (int y = 0; y < size.y; y++) {
                for (int x = 0; x < size.x; x++) {
                    int cellX = x * cellWidth;
                    int cellY = y * cellHeight;
                    cellRects[index] = new RectInt(cellX, cellY, cellWidth, cellHeight);
                    index++;
                }
            }
            return cellRects;
        }

        public void SetupBigCell(List<CellRoomGO> cells, CellRoomGO cellRoomGO, Element selectedElement, LayerType layerType) {
            int i = 0;
            Sprite[] sprites = GetSprites(selectedElement);
            cells.ForEach(cell => {
                Sprite sprite = sprites[i];
                cell.SetupBigCell(cellRoomGO.GetInstanceID(), selectedElement, layerType, sprite);
                i++;
            });
        }

        public void AddCellInUsedCell(Element element, Vector2Int position, LayerType layerType) {
            if (layerType == LayerType.BOTTOM) {
                bottomLayer.Add(new GridElementModel(element.GetId(), position));
            }
            if (layerType == LayerType.MIDDLE) {
                middleLayer.Add(new GridElementModel(element.GetId(), position));
            }
            if (layerType == LayerType.TOP) {
                topLayer.Add(new GridElementModel(element.GetId(), position));
            }
        }

        public List<CellRoomGO> DeleteCell(CellRoomGO cellRoomGO, LayerType layerType) {
            Element config = cellRoomGO.GetConfig(layerType);
            if (config == null && !cellRoomGO.IsDesactivatedCell(layerType)) return null;
            bool isDeletedCell = RemoveCellInUsedCell(cellRoomGO, layerType);
            if (isDeletedCell) {
                List<CellRoomGO> cells = GetCellsAtPosition(cellRoomGO, config.GetSize());
                cells.ForEach(cell => {
                    cell.ResetCellLayers(layerType);
                });
                return cells;
            }
            return null;
        }

        private bool RemoveCellInUsedCell(CellRoomGO cellRoomGO, LayerType layerType) {
            if (layerType == LayerType.BOTTOM) {
                return DeleteElement(bottomLayer, layerType, cellRoomGO);
            }
            if (layerType == LayerType.MIDDLE) {
                return DeleteElement(middleLayer, layerType, cellRoomGO);
            }
            if (layerType == LayerType.TOP) {
                return DeleteElement(topLayer, layerType, cellRoomGO);
            }
            return false;
        }

        private bool DeleteElement(List<GridElementModel> layer, LayerType layerType, CellRoomGO cellRoomGO) {
            int index = layer.FindIndex(cellConfig =>
                cellConfig.GetId() == cellRoomGO.GetConfig(layerType).GetId() &&
                cellConfig.GetPosition() == cellRoomGO.GetPosition());
            if (index != -1) {
                layer.RemoveAt(index);
                return true;
            }
            return false;
        }

    }
}
