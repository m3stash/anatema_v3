using System;

public class Block : Element {

    public Block(
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