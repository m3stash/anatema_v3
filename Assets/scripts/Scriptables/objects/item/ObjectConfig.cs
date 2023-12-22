using UnityEngine;

public class ObjectConfig : ScriptableObject {
    [Header("Settings")]
    protected ItemType category;
    [SerializeField] private string displayName;
    [SerializeField] private Sprite icon;
    [SerializeField] private string description;
    [SerializeField] private int id;
}