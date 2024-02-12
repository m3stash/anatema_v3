public class Potion : Item {

    private int max;

    public Potion(
        int elementId,
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
        int max
    ): base(
        elementId,
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
        craftable
    ) {
        this.max = max;
    }

    public int GetMax() => max;
}