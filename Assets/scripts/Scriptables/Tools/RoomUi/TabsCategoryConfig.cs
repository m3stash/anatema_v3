using UnityEngine;

[CreateAssetMenu(fileName = "TablsCategoryConfig", menuName = "RoomUi / Create tabs Configuration")]
public class ItemCategoryConfig: ScriptableObject {
    [SerializeField] private Sprite item;
    [SerializeField] private Sprite entity;
    [SerializeField] private Sprite block;
    [SerializeField] private Sprite iconCONSUMABLES;
    [SerializeField] private Sprite iconBLOCK;
}

