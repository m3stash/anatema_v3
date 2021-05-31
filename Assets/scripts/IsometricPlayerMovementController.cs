using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class IsometricPlayerMovementController : MonoBehaviour {

    public float movementSpeed = 1f;
    IsometricCharacterRenderer isoRenderer;
    private Vector2 moveDirection;
    private float currentSpeed;
    private bool hMove;
    private bool vMove;
    private Vector2 m_Velocity = Vector2.zero;
    private float m_MovementSmoothing = .05f;

    Rigidbody2D rg2d;
    
    void Start() {
        InputManager.gameplay.Player.Move.performed += Move;
    }
    private void OnDestroy() {
        InputManager.gameplay.Player.Move.performed -= Move;
    }

    private void Awake() {
        rg2d = GetComponent<Rigidbody2D>();
        isoRenderer = GetComponentInChildren<IsometricCharacterRenderer>();
    }

    private void Move(InputAction.CallbackContext ctx) {
        moveDirection = ctx.ReadValue<Vector2>();
    }

    private void SetVelocity() {
        Vector2 targetVelocity;
        currentSpeed = 0;
        if (hMove) {
            currentSpeed = Mathf.Abs(moveDirection.x * 25f);
            targetVelocity = new Vector2(moveDirection.x * currentSpeed, rg2d.velocity.y);
        } else {
            targetVelocity = new Vector2(0, rg2d.velocity.y);
        }
        rg2d.velocity = Vector2.SmoothDamp(rg2d.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
        if (vMove) {
            currentSpeed = Mathf.Abs(moveDirection.y * 25f);
            targetVelocity = new Vector2(rg2d.velocity.x, moveDirection.y * currentSpeed);
        } else {
            targetVelocity = new Vector2(rg2d.velocity.x, 0);
        }
        rg2d.velocity = Vector2.SmoothDamp(rg2d.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
    }

    void Update() {
        hMove = moveDirection.x > 0 || moveDirection.x < 0 ? true : false;
        vMove = moveDirection.y > 0 || moveDirection.y < 0 ? true : false;
    }

    // Update is called once per frame
    void FixedUpdate() {
        /*Vector2 currentPos = rg2d.position;
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector2 inputVector = new Vector2(horizontalInput, verticalInput);
        inputVector = Vector2.ClampMagnitude(inputVector, 1);
        Vector2 movement = inputVector * movementSpeed;
        Vector2 newPos = currentPos + movement * Time.fixedDeltaTime;
        isoRenderer.SetDirection(movement);
        rg2d.MovePosition(newPos);*/
        SetVelocity();
    }
}