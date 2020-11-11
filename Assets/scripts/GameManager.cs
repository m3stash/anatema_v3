using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject roomContainer;
    public LevelGenerator levelGenerator;

    public static GameManager instance;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        levelGenerator = GetComponent<LevelGenerator>();
        levelGenerator.StartGeneration(roomContainer);
    }

}
