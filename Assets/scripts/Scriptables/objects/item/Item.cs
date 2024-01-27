using System;

public class Item : Element {

    public Item() {
        ElementCategoryType = ElementCategoryType.ITEM;
    }

    public bool Dropable { get; set; }
    public bool Consumable { get; set; }
    public bool Craftable { get; set; }
    public int Max { get; set; }
    public float Weight { get; set; }

    public string SubCategory { get; set; }

    public override Type GetSubCategory() {
        return typeof(ItemType);
    }

}