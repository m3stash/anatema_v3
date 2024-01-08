using UnityEngine;

[CreateAssetMenu(fileName = "potion--config", menuName = "Object Configuration / Item / Potion / Heal")]
public class Heal : Potion {

    public Heal() {
        potionType = PotionType.HEAL;
    }
    
    [Header("Recovered Life Config")]
    [SerializeField] private int amount;
    [SerializeField] private float duration;
}