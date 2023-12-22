using UnityEngine;

public class Potion : ItemConfig {
    public Potion() {
        category = ItemType.POTION;
        itemSize = new Vector2Int(1, 1);
    }
}