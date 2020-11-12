using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class Vcam : MonoBehaviour {

    private CinemachineConfiner confiner;

    private void Awake() {
        confiner = GetComponent<CinemachineConfiner>();
    }

    private void Update() {
        // framingTransposer.m_ScreenX += 0.3f * Time.deltaTime;
    }

    private void Start() {
 
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision) {
            // transform.position = new Vector2(50, 13);
        }
    }

}
