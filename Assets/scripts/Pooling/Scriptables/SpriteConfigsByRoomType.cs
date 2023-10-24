using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteConfigsByRoomType", menuName = "Sprite Configs/Create Config by RoomType")]
public class SpriteConfigsByRoomType : ScriptableObject {

    public SpriteConfigsByRoomType() {
        Initialize();
    }

    [Serializable]
    public class BiomeRoomTypeSprites {
        public BiomeEnum biome;
        public Sprite sprites;
    }

    public List<BiomeRoomTypeSprites> standard = new List<BiomeRoomTypeSprites>();
    public List<BiomeRoomTypeSprites> starter = new List<BiomeRoomTypeSprites>();
    public Sprite boss;
    public Sprite item;
    public Sprite secret;

    public void Initialize() {
        foreach (BiomeEnum biome in System.Enum.GetValues(typeof(BiomeEnum))) {
            // Vérifie si le biome existe déjà dans la liste standard
            if (!standard.Exists(item => item.biome == biome)) {
                // Ajoute un nouvel élément à la liste standard
                standard.Add(new BiomeRoomTypeSprites() { biome = biome });
            }

            // Vérifie si le biome existe déjà dans la liste starter
            if (!starter.Exists(item => item.biome == biome)) {
                // Ajoute un nouvel élément à la liste starter
                starter.Add(new BiomeRoomTypeSprites() { biome = biome });
            }
        }
    }

    public void Refresh() {
        // Créez une liste temporaire pour stocker les biomes existants
        List<BiomeEnum> existingBiomes = new List<BiomeEnum>();

        foreach (BiomeEnum biome in Enum.GetValues(typeof(BiomeEnum))) {
            existingBiomes.Add(biome);

            // Vérifie si le biome existe déjà dans la liste standard
            if (!standard.Exists(item => item.biome == biome)) {
                // Ajoute un nouvel élément à la liste standard
                standard.Add(new BiomeRoomTypeSprites() { biome = biome });
            }

            // Vérifie si le biome existe déjà dans la liste starter
            if (!starter.Exists(item => item.biome == biome)) {
                // Ajoute un nouvel élément à la liste starter
                starter.Add(new BiomeRoomTypeSprites() { biome = biome });
            }
        }

        // Supprimez les éléments qui ne correspondent pas aux biomes existants
        standard.RemoveAll(item => !existingBiomes.Contains(item.biome));
        starter.RemoveAll(item => !existingBiomes.Contains(item.biome));
    }
}

[CustomEditor(typeof(SpriteConfigsByRoomType))]
public class SpriteConfigsByRoomTypeEditor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        SpriteConfigsByRoomType spriteConfigsByRoomType = (SpriteConfigsByRoomType)target;

        /*if (GUILayout.Button("Initialize")) {
            spriteConfigsByRoomType.Initialize();
        }*/

        if (GUILayout.Button("Refresh")) {
            spriteConfigsByRoomType.Refresh();
        }
    }
}