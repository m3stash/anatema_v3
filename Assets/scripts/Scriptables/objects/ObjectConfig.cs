using System;
using UnityEngine;

public abstract class ObjectConfig {

    public string DisplayName { get; set; }
    public string IconPath { get; set; }
    public string Description { get; set; }
    public Sprite Sprite { get; set; }

    public string Id { get; set; }

    public abstract ObjectType ObjectType { get; }

    public abstract Vector2Int Size { get; }

    public abstract T CategoryValue<T>();
    public abstract Type Category();
    
}