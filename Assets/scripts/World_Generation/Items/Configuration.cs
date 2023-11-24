namespace Item {
    public abstract class Configuration {

        public abstract int MAX_ITEM_PER_FLOOR { get; }
        public abstract TypeEnum itemType { get; }
        public abstract int pool { get; }

        public Configuration() { }
    }
}



