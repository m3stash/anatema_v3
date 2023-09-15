using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour {

    public static InputManager instance;
    public static Gameplay gameplay;
    
    private void Awake() {
        instance = this;

        // Init gameplay controls
        gameplay = new Gameplay();
        gameplay.Enable();
    }


    void Start() {
        gameplay.Player.Enable();
    }

    private void OnDestroy() {
        gameplay.Player.Disable();
    }
}

