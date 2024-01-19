public class Container : ItemConfig {
    public override T CategoryValue<T>() {
        return (T)(object)ItemCategory.CONTAINER;
    }
}