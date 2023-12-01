using UnityEngine;
using UnityEngine.UI;

public class CellGO: MonoBehaviour {

    private Image image;
    private Button button;

    public void SetImage(Image image) {
        this.image = image;
    }

    public void SetButton(Button button) {
        this.button = button;
    }

    public void DesactivateCell() {
        image.enabled = false;
        button.interactable = false;
    }

    public void AddWall() {
        image.color = Color.black;
        button.interactable = false;
    }

    public void AddDoor() {
        image.color = Color.yellow;
        button.interactable = false;
    }

    public void Setup() {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
    }
}

