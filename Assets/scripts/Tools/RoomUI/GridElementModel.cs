using UnityEngine;
using Newtonsoft.Json;

public class GridElementModel {
    [JsonProperty("id")]
    private int id;
    [JsonProperty("posX")]
    private int posX;
    [JsonProperty("posY")]
    private int posY;
    private Element element;

    public GridElementModel(int id, Vector2Int position) {
        this.id = id;
        posX = position.x;
        posY = position.y;
    }

    public int GetId() {
        return id;
    }

    public Element GetElement() {
        return element;
    }

    public Vector2Int GetPosition() {
        return new Vector2Int(posX, posY);
    }

    public void SetElement(Element element) {
        this.element = element;
    }

}