using UnityEngine;

public class ItemConfig : ObjectConfig {
    [Header("Settings")]
    protected PotionType subCategory;
    // [SerializeField] private GameObject prefab;
    [SerializeField] private string displayName;
    [SerializeField] private Sprite icon;
    [SerializeField] private string description;
    [SerializeField] private int id;
    [SerializeField] private bool dropable;
    [SerializeField] private bool consumable;
    [SerializeField] private bool craftable;
    [SerializeField] private int limit;
    [SerializeField] protected Vector2Int itemSize = new Vector2Int(1,1);
    [SerializeField] private float weight;
}