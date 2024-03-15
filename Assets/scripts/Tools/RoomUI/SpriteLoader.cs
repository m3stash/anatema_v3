using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SpriteLoader {

    private string path;

    public SpriteLoader(string path) {
        this.path = path;
    }

    private Dictionary<string, Dictionary<string, Sprite>> dictionnary = new Dictionary<string, Dictionary<string, Sprite>>();

    public async Task LoadSpritesAsync(string category) {
        if (path == null) {
            Debug.LogError("Resource Path is not set.");
            return;
        }

        if (!dictionnary.ContainsKey(category)) {
            Dictionary<string, Sprite> spriteDictionary = new Dictionary<string, Sprite>();
            Sprite[] loadedSprites = Resources.LoadAll<Sprite>(path + category.ToLower());
            foreach (Sprite sprite in loadedSprites) {
                string spriteName = sprite.name.Split('_')[0];
                spriteDictionary.Add(spriteName, sprite);
            }
            dictionnary.Add(category, spriteDictionary);
        }
        await Task.FromResult(0);
    }

    public Sprite GetSprite(string category, string name) {
        if (dictionnary[category].ContainsKey(name)) {
            return dictionnary[category][name];
        }
        else {
            Debug.LogError($"Sprite {name} not found in dictionary in category {category}.");
            return null;
        }
    }

}