using UnityEngine;
using UnityEngine.UI;
public class CellPreview : MonoBehaviour {
    private Image image;
    private Sprite defaultSprite;
    private RectTransform rectTransform;
    [SerializeField] private Sprite forbidden;
    [SerializeField] private Sprite trash;

    private void Awake() {
        image = GetComponent<Image>();
        defaultSprite = image.sprite;
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetSprite(Sprite sprite) {
        image.sprite = sprite;
    }

    public void SetPosition(Vector3 position) {
        transform.position = position;
    }

    public void SetSize(Vector2 size) {
        rectTransform.sizeDelta = size;
    }

    public void ForbiddenAction(){
        image.sprite = forbidden;
        Color currentColor = image.color;
        currentColor.a = 0.7f;
        image.color = currentColor;
    }

    public void TrashAction(){
        image.sprite = trash;
        Color currentColor = image.color;
        currentColor.a = 0.7f;
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
