
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour {
    [SerializeField] private ItemConfig config;
    private Image image;


    public void Setup(ItemConfig config) {
        this.config = config;
    }
}
