using UnityEngine;

[CreateAssetMenu(fileName = "container--config", menuName = "Object Configuration / Item / Container")]
public class Container : ItemConfig {

    public Container() {
        category = ItemType.CONTAINER;
    }
}