using System;

public class Entity : Element {

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
        string groupType
    ): base(
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
    }

}