namespace Item {
    public class Chest: Configuration {
        public override int MAX_ITEM_PER_FLOOR => 10;
        public override TypeEnum itemType => TypeEnum.CHEST;
        public override int pool => 0;
    }
}