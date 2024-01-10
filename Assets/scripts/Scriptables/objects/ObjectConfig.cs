using System;
using UnityEngine;

public abstract class ObjectConfig : ScriptableObject {
    [Header("Settings")]
    [SerializeField] private string displayName;
    [SerializeField] private Sprite icon;
    [SerializeField] private string description;
    [SerializeField] private int id;

    public abstract ObjectType ObjectType { get; }

    public abstract T CategoryValue<T>();
    public abstract Type Category();

    public Sprite GetSprite() {
        return icon;
    }

    public string GetName() {
        return displayName;
    }
    public string GetDescription() {
        return description;
    }
}