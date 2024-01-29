using UnityEngine;

public class Element {

    public Element(
        int id, 
        string category, 
        string displayName, 
        string subCategory, 
        string description, 
        string iconPath, 
        int sizeX, 
        int sizeY, 
        string biome,
        string groupType
    ) {
        this.id = id;
        this.category = category;
        this.displayName = displayName;
        this.subCategory = subCategory;
        this.description = description;
        this.iconPath = iconPath;
        this.sizeX = sizeX;
        this.sizeY = sizeY;
        this.biome = biome;
        this.groupType = groupType;
    }

    private int id;
    private string displayName;
    private string category;
    private string subCategory;
    private string iconPath;
    private string description;
    private int sizeX;
    private int sizeY;
    private string biome;

    private string groupType;

    public int GetId() {
        return id;
    }

    public string GetDisplayName() => displayName;
    public string GetCategory() => category;
    public string GetSubCategory() => subCategory;  
    public string GetDescription() => description;
    public string GetBiome() => biome;

    public Sprite GetSprite() {
        // toDO -> faire un cache pour les sprites
        return Resources.Load<Sprite>(iconPath);
    }

    public Vector2Int GetSize() => new Vector2Int(sizeX, sizeY);
    public string GetGroupType() => groupType;
    
}