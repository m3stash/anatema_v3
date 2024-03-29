﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {

    private Vector2 moveDirection;
    private Vector3 localScale;
    private Rigidbody2D rg2d;
    private const int defaultGravityScale = 3;
    private float currentSpeed;
    private bool hMove;
    private bool vMove;
    private Vector2 m_Velocity = Vector2.zero;
    private float speed = 0;
    private float m_MovementSmoothing = .05f;

    // Start is called before the first frame update
    void Start() {
        localScale = GetComponent<Transform>().localScale;
        rg2d = GetComponent<Rigidbody2D>();
        InputManager.gameplay.Player.Move.performed += Move;
        InputManager.gameplay.Player.Jump.performed += Jump;
    }
    private void OnDestroy() {
        InputManager.gameplay.Player.Jump.performed -= Jump;
        InputManager.gameplay.Player.Move.performed -= Move;
    }
    private void Move(InputAction.CallbackContext ctx) {
        moveDirection = ctx.ReadValue<Vector2>();
    }

    private void Jump(InputAction.CallbackContext ctx) {
        rg2d.gravityScale = defaultGravityScale;
        rg2d.velocity = new Vector2(rg2d.velocity.x, 18);
        // animator.SetTrigger("JumpTrigger");
    }

    private void SetVelocity() {
        Vector2 targetVelocity = Vector2.zero;
        currentSpeed = 0;
        if (hMove) {
            currentSpeed = Mathf.Abs(moveDirection.x * speed);
            targetVelocity = new Vector2(moveDirection.x * currentSpeed, rg2d.velocity.y);
        }
        if (vMove) {
            currentSpeed = Mathf.Abs(moveDirection.y * speed);
            targetVelocity = new Vector2(0, rg2d.velocity.y);
        }
        rg2d.velocity = Vector2.SmoothDamp(rg2d.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
    }

    // Update is called once per frame
    void Update() {
        hMove = moveDirection.x > 0 || moveDirection.x < 0 ? true : false;
        vMove = moveDirection.y > 0 || moveDirection.y < 0 ? true : false;
    }

    private void FixedUpdate() {
        SetVelocity();
    }
}
