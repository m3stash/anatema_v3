using UnityEngine;

[CreateAssetMenu(fileName = "usable--config", menuName = "Object Configuration / Item / Usable")]
public class Usable : ItemConfig {

    public Usable() {
        category = ItemType.USABLE;
    }
}