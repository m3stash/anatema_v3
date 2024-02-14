using UnityEngine;
using Newtonsoft.Json;

public class GridElementModel {
    [JsonProperty("id")]
    private int id;
    [JsonProperty("elementId")]
    private int elementID;
    [JsonProperty("posX")]
    private int posX;
    [JsonProperty("posY")]
    private int posY;

    public GridElementModel(int id, int elementID, Vector2Int position) {
        this.id = id;
        this.elementID = elementID;
        posX = position.x;
        posY = position.y;
    }

    public int GetId() {
        return id;
    }

    public int GetElementId() {
        return elementID;
    }

    public Vector2Int GetPosition() {
        return new Vector2Int(posX, posY);
    }

}