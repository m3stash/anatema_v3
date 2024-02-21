using UnityEngine;

public class Heal : Potion {

    private int amount;
    private float duration;

    public Heal(
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
        bool craftable,
        int max,
        int amount,
        float duration
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
        groupType,
        dropable,
        consumable,
        craftable,
        max
    ) {
        this.amount = amount;
        this.duration = duration;
    }

    public int GetAmount() => amount;
    public float GetDuration() => duration;
}