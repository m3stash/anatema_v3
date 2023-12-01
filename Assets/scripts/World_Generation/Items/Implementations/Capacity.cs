namespace Item {
    public class Capacity : Configuration {
        public override int MAX_ITEM_PER_FLOOR => 1;
        public override TypeEnum itemType => TypeEnum.CAPACITY;
        public override int pool => 0;
    }

}

