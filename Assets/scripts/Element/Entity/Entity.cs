using System;

public class Entity : Element {

    public string type;

    public Entity(
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
        string type
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
        this.type = type;
    }

    public string GetEntityType() {
        return type;
    }

}