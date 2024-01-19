public class Usable : ItemConfig {
    public override T CategoryValue<T>() {
        return (T)(object)ItemCategory.USABLE;
    }
}