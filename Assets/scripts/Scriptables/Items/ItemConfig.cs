using UnityEngine;

[CreateAssetMenu(fileName = "ItemConfig", menuName = "Item Config/Create Item Config")]
public class ItemConfig : ScriptableObject {
    [Header("Main Settings")]

    [SerializeField] private int id;
    [SerializeField] private GameObject prefab;
    [SerializeField] private string displayName;
    [SerializeField] private string description;
    [SerializeField] private ItemType itemType;
    [SerializeField] private Sprite sprite;
}
