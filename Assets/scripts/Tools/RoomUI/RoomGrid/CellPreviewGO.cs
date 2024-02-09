using UnityEngine;
using UnityEngine.UI;
public class CellPreviewGO : MonoBehaviour {
    private Image image;
    private RectTransform rectTransformImage;
    private RectTransform rectTransformHover;
    [SerializeField] private GameObject imageGO;
    [SerializeField] private GameObject hoverGO;
    [SerializeField] private Sprite forbidden;
    [SerializeField] private Sprite trash;
    [SerializeField] private Sprite copy;

    private void Awake() {
        if(imageGO != null || hoverGO != null){
            image = imageGO.GetComponent<Image>();
            rectTransformImage = imageGO.GetComponent<RectTransform>();
            rectTransformHover = hoverGO.GetComponent<RectTransform>();
            SetPosition(new Vector3(-10000, -10000, 0));
        }else{
            Debug.LogError("CellPreviewGO: imageGO or hoverGO is not set");
        }
    }

    public void SetSprite(Sprite sprite) {
        hoverGO.SetActive(true);
        image.sprite = sprite;
    }

    public void SetPosition(Vector3 position) {
        transform.position = position;
    }

    public void SetSize(Vector2 size) {
        rectTransformImage.sizeDelta = size;
        rectTransformHover.sizeDelta = size;
    }

    public void ForbiddenAction(){
        SetImage(forbidden);
    }

    public void TrashAction(){
        SetImage(trash);
    }

    public void CopyAction(){
        SetImage(copy);
    }

    private void SetImage(Sprite sprite){
        hoverGO.SetActive(true);
        image.sprite = sprite;
        Color currentColor = image.color;
        currentColor.a = 0.85f;
        image.color = currentColor;
    }

    public void ResetCell(){
        image.color = Color.white;
    }

    public void HoverCell(){
        image.sprite = null;
        image.color = Color.green;
    }

    public void HideCellPreview() {
        SetPosition(new Vector3(-10000, -10000, 0));
    }
    
}
