using UnityEngine;

[CreateAssetMenu(fileName = "ObjectsTabConfig", menuName = "Tools / RoomUI / Create tabs category Config")]
public class TabsCategoryConfig: ScriptableObject {
    [SerializeField] private Sprite item;
    [SerializeField] private Sprite entity;
    [SerializeField] private Sprite block;
    [SerializeField] private Sprite decoration;
    // [SerializeField] private Sprite equipment;
    [SerializeField] private Sprite pedestral;

    public Sprite GetItemByCategory(ObjectType type) {
        switch (type) {
            case ObjectType.ITEM:
                return item;
            case ObjectType.ENTITY:
                return entity;
            case ObjectType.BLOCK:
                return block;
            case ObjectType.PEDESTRAL:
                return pedestral;
            case ObjectType.DECORATION:
                return decoration;
            default:
                return null;
        }
    }
}

