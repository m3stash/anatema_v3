using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Biome", menuName = "Game/Biome")]
public class Biome : ScriptableObject {
    public BiomeEnum biome;
    public List<DirectionalSprite> doorSprites;
    public List<DirectionalSprite> chestSprites;

    public Sprite GetDoorSpriteForDirection(DirectionalEnum direction) {
        return doorSprites.FirstOrDefault(ds => ds.direction == direction)?.sprite;
    }

    public Sprite GetChestSpriteForDirection(DirectionalEnum direction) {
        return chestSprites.FirstOrDefault(ds => ds.direction == direction)?.sprite;
    }

}
