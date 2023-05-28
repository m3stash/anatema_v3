using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DebugMode : MonoBehaviour {

    void Awake() {
        // Debug.Log("COUCOU");
    }

    void OnGUI() {

        GUI.Box(new Rect(10, 10, 240, 200), "Debug Menu");

        if (GUI.Button(new Rect(30, 40, 200, 25), "Create Json Room Config")) {
            RoomsJsonConfig.CreateJson();
        }

        if (GUI.Button(new Rect(30, 80, 200, 25), "Clear Json Room Config")) {
            // 
        }
    }

    /*void Update() {
        Debug.Log("Editor causes this Update");
    }*/


}