using System;
using UnityEngine;

public class Element {

    public string Id { get; set; }
    public string DisplayName { get; set; }
    public string Category { get; set; }
    public string IconPath { get; set; }
    public string Description { get; set; }
    public ElementCategoryType ElementCategoryType { get; set; }
    public Sprite Sprite { get; set; }

    protected int SizeX { get; set; }
    protected int SizeY { get; set; }

    public Vector2Int Size => new Vector2Int(SizeX, SizeY);

    public virtual Type GetSubCategory(){
        return null;
    }

    public virtual T GetSubCategoryType<T>() {
        return default;
    }
    
}