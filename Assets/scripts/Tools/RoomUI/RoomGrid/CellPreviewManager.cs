using System.Collections.Generic;
using UnityEngine;

namespace RoomUI {
    public class CellPreviewManager {

        private CellPreviewGO cellPreview;
        private RoomGridService roomGridService;

        public CellPreviewManager(GameObject cellPreviewGO, RoomGridService roomGridService) {
            if(cellPreviewGO == null || roomGridService == null) throw new System.ArgumentNullException("cellPreviewGO || RoomGridService");
            cellPreview = cellPreviewGO.GetComponent<CellPreviewGO>();
            this.roomGridService = roomGridService; 
        }

        public void ManagePreview(Vector3 position, Vector2 elementSize, Vector2 cellSize){
            Vector2 previewSize = new Vector2(cellSize.x * elementSize.x, cellSize.y * elementSize.y);
            Vector3 previewPosition = position;
            if(elementSize.x > 1 || elementSize.y > 1) {
                previewPosition =  new Vector3(position.x + previewSize.x - cellSize.x, position.y + previewSize.y - cellSize.y, position.z);
            }
            cellPreview.SetSize(previewSize);
            cellPreview.SetPosition(previewPosition);
        }

        public void Hide(){
            cellPreview.HideCellPreview();
        }

        public void Reset(){
            cellPreview.ResetCell();
        }

        public void Hover(){
            cellPreview.HoverCell();
        }
        
        public void Forbidden(){
            cellPreview.ForbiddenAction();
        }   

        public void SetSprite(Sprite sprite){
            cellPreview.SetSprite(sprite);
        }

        public void Trash(){
            cellPreview.TrashAction();
        }

        public void Copy(){
            cellPreview.CopyAction();
        }

        public void SetPreviewByActionType(PreviewAction actionType, Vector3 position, Vector2 elementSize, Vector2 cellSize, Sprite sprite = null){
            ManagePreview(position, elementSize, cellSize);
            switch(actionType) {
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
                default:
                    Reset();
                    break;
            }
        }

        public void OnHoverTrashAction(CellRoomGO cellRoomGO, Vector2 cellSize, Vector3 cellRoomGOPosition) {
            if(cellRoomGO.GetConfig() == null){
                SetPreviewByActionType(PreviewAction.HOVER, cellRoomGOPosition, new Vector2(1, 1), cellSize);
                return;
            }
            SetPreviewByActionType(PreviewAction.TRASH, cellRoomGO.GetRootCellRoomGO().transform.position, cellRoomGO.GetConfig().GetSize(), cellSize);
        }

        public void OnHoverCopyAction(CellRoomGO cellRoomGO, Vector2 cellSize, Vector3 cellRoomGOPosition){
            if(cellRoomGO.GetConfig() == null){
                SetPreviewByActionType(PreviewAction.HOVER, cellRoomGOPosition, new Vector2(1, 1), cellSize);
                return;
            }
            SetPreviewByActionType(PreviewAction.COPY, cellRoomGO.GetRootCellRoomGO().transform.position, cellRoomGO.GetConfig().GetSize(), cellSize);
        }

        public void OnHoverSelectAction(CellRoomGO cellRoomGO, Vector2 cellSize, Vector3 cellRoomGOPosition, bool isVoidCell, Element currenSelectedObject) {
            if(isVoidCell && currenSelectedObject == null){
                SetPreviewByActionType(PreviewAction.HOVER, cellRoomGOPosition, new Vector2(1, 1), cellSize);
                return;
            }
            if(currenSelectedObject != null){
                Vector2Int selectedElementSize = currenSelectedObject.GetSize();
                List<CellRoomGO> cells = roomGridService.GetCellsAtPosition(cellRoomGO, selectedElementSize);
                if(cells.Exists(cell => cell.GetConfig() != null || cell.IsDoorOrWall() || cell.IsDesactivatedCell())){
                    SetPreviewByActionType(PreviewAction.FORBIDDEN, cellRoomGOPosition, selectedElementSize, cellSize);
                } else {
                    SetPreviewByActionType(PreviewAction.SHOW_SPRITE, cellRoomGOPosition, selectedElementSize, cellSize, currenSelectedObject.GetSprite());
                }
            }
        }
    }
}