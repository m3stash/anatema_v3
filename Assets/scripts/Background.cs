using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour {

    public void SetBackground(Sprite background) {
        GetComponent<SpriteRenderer>().sprite = background;
    }
}
