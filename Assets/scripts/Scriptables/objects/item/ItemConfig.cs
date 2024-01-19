using System;
using UnityEngine;

public abstract class ItemConfig : ObjectConfig {

    public bool Dropable { get; set; }
    public bool Consumable { get; set; }
    public bool Craftable { get; set; }
    public int Limit { get; set; }
    public float Weight { get; set; }
    
    protected Vector2Int itemSize = new Vector2Int(1, 1);

    public override ObjectType ObjectType => ObjectType.ITEM;
    public override Vector2Int Size => itemSize;

    public override Type Category() {
        return typeof(ItemCategory);
    }

}