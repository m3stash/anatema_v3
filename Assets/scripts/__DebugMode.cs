using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DebugMode : MonoBehaviour {

    public Rect windowRect0 = new Rect(20, 20, 120, 50);
    public Rect windowRect1 = new Rect(20, 100, 120, 50);

    void Awake() {
        // Debug.Log("COUCOU");
    }

    void OnGUI() {

        GUI.color = Color.red;
        windowRect0 = GUI.Window(0, windowRect0, DoMyWindow, "Red Window");

        GUI.color = Color.green;
        windowRect1 = GUI.Window(1, windowRect1, DoMyWindow, "Green Window");

        GUI.Box(new Rect(10, 10, 240, 200), "Debug Menu");

        if (GUI.Button(new Rect(30, 40, 200, 25), "Create Json Room Config")) {
            RoomNs.RoomsJsonConfig.CreateJson();
        }

        if (GUI.Button(new Rect(30, 80, 200, 25), "Clear Json Room Config")) {
            // 
        }

    }

    void DoMyWindow(int windowID) {
        if (GUI.Button(new Rect(10, 20, 100, 20), "Hello World")) {
            Debug.Log("COUCOU");
            print("Got a click in window with color " + GUI.color);
        }

        // Make the windows be draggable.
        GUI.DragWindow(new Rect(0, 0, 10000, 10000));
    }

    /*void Update() {
        Debug.Log("Editor causes this Update");
    }*/


}