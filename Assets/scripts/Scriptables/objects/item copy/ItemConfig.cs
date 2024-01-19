/*using System;
using UnityEngine;

public abstract class ItemConfig : ObjectConfig {
    [Header("Settings")]
    [SerializeField] private bool dropable;
    [SerializeField] private bool consumable;
    [SerializeField] private bool craftable;
    [SerializeField] private int limit;
    [SerializeField] protected Vector2Int itemSize = new Vector2Int(1, 1);
    [SerializeField] private float weight;

    public override ObjectType ObjectType => ObjectType.ITEM;
    public override Vector2Int Size => itemSize;

    public override Type Category() {
        return typeof(ItemCategory);
    }

}*/