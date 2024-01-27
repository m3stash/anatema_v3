using UnityEngine;

[CreateAssetMenu(fileName = "ObjectsTabConfig", menuName = "Tools / RoomUI / Create tabs category Config")]
public class TabsCategoryConfig: ScriptableObject {
    [SerializeField] private Sprite item;
    [SerializeField] private Sprite entity;
    [SerializeField] private Sprite block;
    [SerializeField] private Sprite decoration;
    // [SerializeField] private Sprite equipment;
    [SerializeField] private Sprite pedestral;

    public Sprite GetItemByCategory(ElementCategoryType type) {
        switch (type) {
            case ElementCategoryType.ITEM:
                return item;
            case ElementCategoryType.ENTITY:
                return entity;
            case ElementCategoryType.BLOCK:
                return block;
            case ElementCategoryType.PEDESTRAL:
                return pedestral;
            case ElementCategoryType.DECORATION:
                return decoration;
            default:
                return null;
        }
    }
}

