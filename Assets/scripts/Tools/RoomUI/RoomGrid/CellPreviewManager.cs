using System.Collections.Generic;
using UnityEngine;

namespace RoomUI {
    public class CellPreviewManager {
        private CellPreviewGO cellPreview;
        private RoomGridService roomGridService;

        public CellPreviewManager(GameObject cellPreviewGO, RoomGridService roomGridService) {
            if (cellPreviewGO == null || roomGridService == null) throw new System.ArgumentNullException("cellPreviewGO || RoomGridService");
            cellPreview = cellPreviewGO.GetComponent<CellPreviewGO>();
            this.roomGridService = roomGridService;
        }

        public void ManagePreview(Vector3 position, Vector2 elementSize, Vector2 cellSize) {
            Vector2 previewSize = new Vector2(cellSize.x * elementSize.x, cellSize.y * elementSize.y);

            // Calculate scale factor as a function of resolution
            float scaleFactor = GetScaleFactor();

            // Calculate offset based on cell size and anchor point
            Vector2 offset = new Vector2(cellSize.x * (elementSize.x - 1) / 2f, cellSize.y * (elementSize.y - 1) / 2f);

            // Apply scale factor to offset
            offset *= scaleFactor;

            Vector3 previewPosition;
            if (elementSize.x > 1 || elementSize.y > 1) {
                previewPosition = new Vector3(
                    position.x + offset.x,
                    position.y + offset.y,
                    position.z
                );
            }
            else {
                previewPosition = position;
            }
            cellPreview.SetSize(previewSize);
            cellPreview.SetPosition(previewPosition);
        }

        /*
        ** Function to obtain the scale factor as a function of resolution
        */
        float GetScaleFactor() {
            // base resolution
            float baseResolutionHeight = 1080f;

            // Calculate scale factor as a function of screen height
            return Screen.height / baseResolutionHeight;
        }

        public void Hide() {
            cellPreview.HideCellPreview();
        }

        public void Reset() {
            cellPreview.ResetCell();
        }

        public void Hover() {
            cellPreview.HoverCell();
        }

        public void Forbidden() {
            cellPreview.ForbiddenAction();
        }

        public void SetSprite(Sprite sprite) {
            cellPreview.SetSprite(sprite);
        }

        public void Trash() {
            cellPreview.TrashAction();
        }

        public void Copy() {
            cellPreview.CopyAction();
        }

        public void Default() {

        }

        public void SetPreviewByActionType(PreviewAction actionType, Vector3 position, Vector2 elementSize, Vector2 cellSize, Sprite sprite = null) {
            ManagePreview(position, elementSize, cellSize);
            switch (actionType) {
                case PreviewAction.HOVER:
                    Hover();
                    break;
                case PreviewAction.FORBIDDEN:
                    Forbidden();
                    break;
                case PreviewAction.TRASH:
                    Trash();
                    break;
                case PreviewAction.SHOW_SPRITE:
                    SetSprite(sprite);
                    break;
                case PreviewAction.COPY:
                    Copy();
                    break;
                case PreviewAction.DEFAULT:
                    Hover();
                    break;
                default:
                    Reset();
                    break;
            }
        }

        public void OnHoverTrashAction(CellRoomGO cellRoomGO, Vector2 cellSize, Vector3 cellRoomGOPosition, LayerType layerType) {
            if (cellRoomGO.GetConfig(layerType) == null) {
                SetPreviewByActionType(PreviewAction.HOVER, cellRoomGOPosition, new Vector2(1, 1), cellSize);
                return;
            }
            SetPreviewByActionType(PreviewAction.TRASH, cellRoomGO.transform.position, cellRoomGO.GetConfig(layerType).GetSize(), cellSize);
        }

        public void OnClickTrashAction(CellRoomGO cellRoomGO, LayerType layerType) {
            Vector2 cellSize = cellRoomGO.GetCellSize();
            SetPreviewByActionType(PreviewAction.TRASH, cellRoomGO.transform.position, new Vector2(1, 1), cellSize);
            Hover();
        }

        public void OnHoverCopyAction(CellRoomGO cellRoomGO, Vector2 cellSize, Vector3 cellRoomGOPosition, LayerType layerType) {
            if (cellRoomGO.GetConfig(layerType) == null) {
                SetPreviewByActionType(PreviewAction.HOVER, cellRoomGOPosition, new Vector2(1, 1), cellSize);
                return;
            }
            SetPreviewByActionType(PreviewAction.COPY, cellRoomGO.transform.position, cellRoomGO.GetConfig(layerType).GetSize(), cellSize);
        }

        public void OnHoverSelectAction(CellRoomGO cellRoomGO, Vector2 cellSize, Vector3 cellRoomGOPosition, bool isVoidCell, Element currenSelectedObject, LayerType layerType) {
            if (isVoidCell && currenSelectedObject == null) {
                SetPreviewByActionType(PreviewAction.HOVER, cellRoomGOPosition, new Vector2(1, 1), cellSize);
                return;
            }
            if (currenSelectedObject != null) {
                Vector2Int selectedElementSize = currenSelectedObject.GetSize();
                List<CellRoomGO> cells = roomGridService.GetCellsAtPosition(cellRoomGO, selectedElementSize);
                if (cells.Exists(cell => cell.GetConfig(layerType) != null || cell.IsDoorOrWall() || cell.IsDesactivatedCell(layerType))) {
                    SetPreviewByActionType(PreviewAction.FORBIDDEN, cellRoomGOPosition, selectedElementSize, cellSize);
                }
                else {
                    SetPreviewByActionType(PreviewAction.SHOW_SPRITE, cellRoomGOPosition, selectedElementSize, cellSize, currenSelectedObject.GetSprite());
                }
            }
            else {
                SetPreviewByActionType(PreviewAction.DEFAULT, cellRoomGOPosition, Vector2.one, cellSize);
            }
        }

    }
}