using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CellRoomGO: MonoBehaviour {

    private Image image;
    private Button button;
    private Color defaultColor;
    private ObjectConfig config;
    [SerializeField] private GameObject cell;
    [SerializeField] private Sprite defaultIcon;
    private Image icon;
    private RectTransform rectTransform;
    private RectTransform childRectTransform;

    private float lastWith = 0;

    public delegate void CellClickEvent(CellRoomGO cellRoomGO);
    public static event CellClickEvent OnClick;

    void Awake() {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnCellClick);
        image = GetComponent<Image>();
        defaultColor = image.color;
        icon = cell.GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        childRectTransform = cell.GetComponent<RectTransform>();
    }

    private void OnCellClick() {
        OnClick?.Invoke(this);
    }

    public ObjectConfig GetConfig() {
        return config;
    }

    private void SetComponentValues(ObjectConfig config){
        this.config = config;
        Sprite cellIcon = config.GetSprite();
        icon.sprite = cellIcon;
    }

    void OnDestroy() {
        button.onClick.RemoveListener(OnCellClick);
    }

    public void DesactivateCell() {
        image.enabled = false;
        button.interactable = false;
        ResetCell();
    }

    public void AddWall() {
        image.color = Color.gray;
        button.interactable = false;
        icon.sprite = null;
        cell.SetActive(false);
    }

    public void AddDoor() {
        image.color = Color.yellow;
        button.interactable = false;
        icon.sprite = null;
        cell.SetActive(false);
    }

    public void ResetCell(){
        config = null;
        icon.sprite = null;
        icon.color = Color.white;
    }

    /*private void LateUpdate() {
        float width = rectTransform.sizeDelta.x;
        if(width == lastWith)return;
        resizeCellSize(width);
    }*/

    private void ResizeCellSize() {
        float width = rectTransform.sizeDelta.x;
        float height = rectTransform.sizeDelta.y;
        childRectTransform.sizeDelta = new Vector2(width * 0.75f, height * 0.75f);
    }

    public void Setup(ObjectConfig config) {
        StartCoroutine(AdjustSizeAfterFrame());
        DefaultCellConfiguration();
        if (config) {
            SetComponentValues(config);
        }
    }

    private void DefaultCellConfiguration(){
        cell.SetActive(true);
        image.enabled = true;
        button.interactable = true;
        image.color = defaultColor;
    }

    public void ResizeCellZiseAfterZoom(){
        StartCoroutine(AdjustSizeAfterFrame());
    }

    // used to solve the problem of recovering the size of the RectTransform, which is erroneous because it is driven by the gridLayout
    private IEnumerator AdjustSizeAfterFrame() {
        yield return new WaitForEndOfFrame();
        ResizeCellSize();
    }

}

