public class Heal : Potion {

    public Heal() {
        potionType = PotionType.HEAL;
    }
    
    public bool Amount { get; set; }
    public float Duration { get; set; }
}