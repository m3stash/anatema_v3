using System.Drawing;
using UnityEngine;

public class Potion : Item {

    protected PotionType potionType;
    public Potion() {
        SubCategory = ItemType.POTION.ToString();
    }

    public override T GetSubCategoryType<T>() {
        return (T)(object)ItemType.POTION;
    }
}