using System;
using System.Collections.Generic;
using RoomNs;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteConfig", menuName = "Sprite Config/Create Sprite Config")]
public class SpriteConfig : ScriptableObject {

    private void OnEnable()
    {
        Initialize();
    }

    [Serializable]
    public class BiomeRoomTypeSprites {
        public RoomTypeEnum roomType;
        public Sprite topSprite;
        public Sprite leftSprite;
    }

    [Serializable]
    public class BiomeRoomTypes {
        public BiomeEnum biome;
        public List<BiomeRoomTypeSprites> biomeRoomTypeSprites;
    }

    public List<BiomeRoomTypes> biomeRoomTypesList;

    private void Initialize() {
        foreach (BiomeEnum biome in Enum.GetValues(typeof(BiomeEnum))) {
            if (!biomeRoomTypesList.Exists(item => item.biome == biome)) {
                BiomeRoomTypes biomeRoomTypes = new BiomeRoomTypes {
                    biome = biome,
                    biomeRoomTypeSprites = new List<BiomeRoomTypeSprites>()
                };

                foreach (RoomTypeEnum roomType in Enum.GetValues(typeof(RoomTypeEnum))) {
                    BiomeRoomTypeSprites roomTypeSprites = new BiomeRoomTypeSprites {
                        roomType = roomType,
                        topSprite = null,
                        leftSprite = null
                    };

                    biomeRoomTypes.biomeRoomTypeSprites.Add(roomTypeSprites);
                }

                biomeRoomTypesList.Add(biomeRoomTypes);
            }
        }
        DeleteUnusedBiomes();
        UpdateRoomTypes();
    }

    private void UpdateRoomTypes() {
        List<RoomTypeEnum> existingRoomTypes = new List<RoomTypeEnum>();

        foreach (RoomTypeEnum roomType in Enum.GetValues(typeof(RoomTypeEnum))) {
            existingRoomTypes.Add(roomType);
        }

        foreach (var biomeRoomTypes in biomeRoomTypesList) {
            foreach (var roomType in AllRoomTypeEnums()) {
                if (!biomeRoomTypes.biomeRoomTypeSprites.Exists(item => item.roomType == roomType)) {
                    BiomeRoomTypeSprites roomTypeSprites = new BiomeRoomTypeSprites {
                        roomType = roomType,
                        topSprite = null,
                        leftSprite = null
                    };
                    biomeRoomTypes.biomeRoomTypeSprites.Add(roomTypeSprites);
                }
            }
            biomeRoomTypes.biomeRoomTypeSprites.RemoveAll(item => !existingRoomTypes.Contains(item.roomType));
        }
    }

    private IEnumerable<RoomTypeEnum> AllRoomTypeEnums() {
        foreach (RoomTypeEnum roomType in Enum.GetValues(typeof(RoomTypeEnum))) {
            yield return roomType;
        }
    }

    private void DeleteUnusedBiomes() {
        List<BiomeEnum> existingBiomes = new List<BiomeEnum>();

        foreach (BiomeEnum biome in Enum.GetValues(typeof(BiomeEnum))) {
            existingBiomes.Add(biome);
        }

        // Supprime les BiomeRoomTypes qui n'existent plus
        biomeRoomTypesList.RemoveAll(biomeRoomTypes => !existingBiomes.Contains(biomeRoomTypes.biome));
    }

    public void Refresh() {
        /*Debug.Log(biomeRoomTypesList.Count);
        Initialize();
        DeleteUnusedBiomes();*/
    }

}


/*[CustomEditor(typeof(SpriteConfig))]
public class SpriteConfigsEditor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        SpriteConfig spriteConfigsByRoomType = (SpriteConfig)target;

        if (GUILayout.Button("Refresh")) {
            spriteConfigsByRoomType.Refresh();
        }
    }
}*/
