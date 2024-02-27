using UnityEngine;

[CreateAssetMenu(fileName = "ObjectsTabConfig", menuName = "Tools / RoomUI / Create tabs category Config")]
public class TabsCategoryConfig: ScriptableObject {
    [SerializeField] private Sprite item;
    [SerializeField] private Sprite entity;
    [SerializeField] private Sprite block;
    [SerializeField] private Sprite decoration;
    // [SerializeField] private Sprite equipment;
    [SerializeField] private Sprite pedestral;

    public Sprite GetItemByCategory(string type) {
        switch (type) {
            case "ITEM":
                return item;
            case "ENTITY":
                return entity;
            case "BLOCK":
                return block;
            case "PEDESTRAL":
                return pedestral;
            case "DECORATION":
                return decoration;
            default:
                return null;
        }
    }
}

