using System;
using System.Collections.Generic;
using System.Linq;
using RoomNs;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteConfig", menuName = "Sprite Config/Create Sprite Config")]
public class SpriteConfig : ScriptableObject {

    [Serializable]
    private class BiomeRoomTypes {
        public string biomeName;
        public List<BiomeRoomTypeSprites> biomeRoomTypeSprites;
    }

    [Serializable]
    private class BiomeRoomTypeSprites {
        public string roomType;
        public Sprite topSprite;
        public Sprite leftSprite;
    }

    [SerializeField]
    private List<BiomeRoomTypes> biomeRoomTypesList;

    [SerializeField]
    BiomeRoomTypeSprites SecretConfig = new BiomeRoomTypeSprites() {
        roomType = RoomTypeEnum.SECRET.ToString(),
        leftSprite = null,
        topSprite = null
    };

    private void Reset() {
        biomeRoomTypesList = new List<BiomeRoomTypes>();
        Initialize();
    }

    private void OnEnable() {
        Update();
    }

    private List<RoomTypeEnum> GetExistingRoomTypes() {
        return Enum.GetValues(typeof(RoomTypeEnum))
                   .Cast<RoomTypeEnum>()
                   .Where(roomType => roomType == RoomTypeEnum.STANDARD || roomType == RoomTypeEnum.STARTER)
                   .ToList();
    }

    private List<BiomeEnum> GetExistingBiomes() {
        return Enum.GetValues(typeof(BiomeEnum))
                   .Cast<BiomeEnum>()
                   .ToList();
    }

    private void Initialize() {
        foreach (BiomeEnum biome in GetExistingBiomes()) {
            biomeRoomTypesList.Add(CreateBiome(biome.ToString()));
        }
    }

    private void DeleteOldBiomes() {
        List<string> toDelete = new List<string>();
        foreach (BiomeRoomTypes biomeRoomType in biomeRoomTypesList) {
            bool isCurrentBiomeExist = GetExistingBiomes().Exists(biome => biomeRoomType.biomeName == biome.ToString());
            if (!isCurrentBiomeExist) {
                toDelete.Add(biomeRoomType.biomeName);
            }
        }

        foreach (string biomeName in toDelete) {
            BiomeRoomTypes findElt = biomeRoomTypesList.Find(item => biomeName == item.biomeName);
            if (findElt != null) {
                biomeRoomTypesList.Remove(findElt);
            }
        }
    }

    private void DeleteOldBiomeRoomTypeSprites(BiomeRoomTypes biomeRoomTypes) {
        List<string> biomeRoomTypeSpriteToDelete = new List<string>();
        foreach (BiomeRoomTypeSprites biomeRoomTypeSprite in biomeRoomTypes.biomeRoomTypeSprites) {
            bool isCurrentRoomTypeExist = GetExistingRoomTypes().Exists(roomType => biomeRoomTypeSprite.roomType == roomType.ToString());
            if (!isCurrentRoomTypeExist) {
                biomeRoomTypeSpriteToDelete.Add(biomeRoomTypeSprite.roomType);
            }
        }

        foreach (string biomeRoomName in biomeRoomTypeSpriteToDelete) {
            BiomeRoomTypeSprites findElt = biomeRoomTypes.biomeRoomTypeSprites.Find(item => biomeRoomName == item.roomType);
            if (findElt != null) {
                biomeRoomTypes.biomeRoomTypeSprites.Remove(findElt);
            }
        }
    }

    private void Update() {
        List<BiomeEnum> existingBiomes = GetExistingBiomes();

        // delete old biomes
        DeleteOldBiomes();
        
        foreach (BiomeEnum biome in existingBiomes) {
            string biomeToSgtring = biome.ToString();
            BiomeRoomTypes biomeRoomTypes = biomeRoomTypesList.Find(item => item.biomeName == biomeToSgtring);
            // add new biome if not present
            if (biomeRoomTypes == null) {
                biomeRoomTypesList.Add(CreateBiome(biomeToSgtring));
            } else {
                // delete old
                DeleteOldBiomeRoomTypeSprites(biomeRoomTypes);
                // or add if not exist
                CreateRoomTypeEnum(biomeRoomTypes);
            }
        }

    }

    private BiomeRoomTypes CreateBiome(string biome) {
        BiomeRoomTypes biomeRoomTypes = new BiomeRoomTypes {
            biomeName = biome,
            biomeRoomTypeSprites = new List<BiomeRoomTypeSprites>()
        };
        CreateRoomTypeEnum(biomeRoomTypes);
        return biomeRoomTypes;
    }

    private void CreateRoomTypeEnum(BiomeRoomTypes biomeRoomTypes) {
        foreach (RoomTypeEnum roomType in GetExistingRoomTypes()) {
            string roomTypeStr = roomType.ToString();
            if (!biomeRoomTypes.biomeRoomTypeSprites.Exists(item => item.roomType == roomTypeStr)) {
                BiomeRoomTypeSprites roomTypeSprites = new BiomeRoomTypeSprites() {
                    roomType = roomTypeStr,
                    topSprite = null,
                    leftSprite = null
                };
                biomeRoomTypes.biomeRoomTypeSprites.Add(roomTypeSprites);
            }
        }
    }

}