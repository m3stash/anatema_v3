using UnityEngine;
using UnityEngine.UI;

public class CellGO: MonoBehaviour {

    private Image image;
    private Button button;
    private Color defaultColor;

    public void DesactivateCell() {
        image.enabled = false;
        button.interactable = false;
    }

    public void AddWall() {
        image.color = Color.gray;
        button.interactable = false;
    }

    public void AddDoor() {
        image.color = Color.yellow;
        button.interactable = false;
    }

    public void Setup() {
        if(button == null || image == null) {
            button = GetComponent<Button>();
            image = GetComponent<Image>();
            defaultColor = image.color;
        } else {
            image.enabled = true;
            button.interactable = true;
            image.color = defaultColor;
        }
        
    }

}

