using System.Collections.Generic;
using UnityEngine;

public class SpriteLoader {

    private string path;

    public SpriteLoader(string path) {
        this.path = path;
    }

    private Dictionary<string, Dictionary<string, Sprite>> dictionnary = new Dictionary<string, Dictionary<string, Sprite>>();

    public void LoadSprites(string currentTab) {
        if(path == null) {
            Debug.LogError("Resource Path is not set.");
            return;
        }
        if (!dictionnary.ContainsKey(currentTab)) {
            Dictionary<string, Sprite> spriteDictionary = new Dictionary<string, Sprite>();
            Sprite[] loadedSprites = Resources.LoadAll<Sprite>(path + currentTab);
            foreach (Sprite sprite in loadedSprites) {
                // case of multiple sprite : toDO find a better way to do this !
                string spriteName = sprite.name.Split('_')[0];
                spriteDictionary.Add(spriteName, sprite);
            }
            dictionnary.Add(currentTab, spriteDictionary);
        }
    }

    public Sprite GetSprite(string category, string name) {
        if (dictionnary[category].ContainsKey(name)) {
            return dictionnary[category][name];
        } else {
            Debug.LogError($"Sprite {name} not found in dictionary in category {category}.");
            return null;
        }
    }

}