using System;

public class Item : Element {

    private bool dropable;
    private bool consumable;
    private bool craftable;
    private int max;
    private float weight;

    public Item(
        int id,
        string category,
        string displayName,
        string subCategory,
        string description,
        string spriteName,
        int sizeX,
        int sizeY,
        string biome,
        string groupType,
        bool dropable,
        bool consumable,
        bool craftable
    ) : base(
        id,
        category,
        displayName,
        subCategory,
        description,
        spriteName,
        sizeX,
        sizeY,
        biome,
        groupType
    ) {
        this.dropable = dropable;
        this.consumable = consumable;
        this.craftable = craftable;
    }

    public bool IsDropable() => dropable;
    public bool IsConsumable() => consumable;
    public bool IsCraftable() => craftable;

}