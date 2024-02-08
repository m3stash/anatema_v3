using UnityEngine;
public class CellPreviewManager {

    private CellPreviewGO cellPreview;

    public CellPreviewManager(GameObject cellPreviewGO) {
        if(cellPreviewGO == null) throw new System.ArgumentNullException("cellPreviewGO");
        cellPreview = cellPreviewGO.GetComponent<CellPreviewGO>();
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
            default:
                Reset();
                break;
        }
    }
}
