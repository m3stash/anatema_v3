using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : MonoBehaviour {

    protected DetectColliders colliders;
    protected Vector3 moveTo;
    protected LocalState localState;
    protected SpriteRenderer spriteRenderer;
    private float moveSpeed;
    private IEnumerator patrol;
    private EnnemyConfig config;

    public virtual void Init(LocalState localState, EnnemyConfig config) {
        this.config = config;
        spriteRenderer = GetComponent<SpriteRenderer>();
        this.localState = localState;
        moveSpeed = config.MoveSpeed();
    }

    /*private void FlipPosition() {
        if (localState.moveDirection != DirectionalEnum.L && transform.position.x > localState.moveTo.x) {
            localState.moveDirection = DirectionalEnum.L;
            spriteRenderer.flipX = false;
        }
        if (localState.moveDirection != DirectionalEnum.R && transform.position.x < localState.moveTo.x) {
            localState.moveDirection = DirectionalEnum.R;
            spriteRenderer.flipX = true;
        }
    }*/

    private void ManageSee() {
        if (!localState.canSee)
            return;
        if (localState.seePlayer) {
            StopPatrol();
            ViewEnnemy(localState.playerPositon);
        }

    }

    private void ManageEar() {
        if (!localState.canEar)
            return;
    }

    private void ManagePatrol() {
        if (!localState.canPatrol)
            return;
        /*
        * transform.position == localState.moveTo ne fonctionnant pas à cause d'écarts de float de 0.02f
        */
        bool isArrived = (transform.position.x - localState.moveTo.x) == 0;

        if (CanMoveFront()) {
            if (patrol != null && isArrived) {
                StopCoroutine(patrol);
                patrol = null;
            }
        } else {
            // add and function evaluate new patrol distance and set new value ?
            if (localState.moveDirection == DirectionalEnum.L) {
                localState.moveDirection = DirectionalEnum.R;
                spriteRenderer.flipX = true;
                localState.moveTo = new Vector2(transform.position.x + 12, transform.position.y);
            } else {
                localState.moveDirection = DirectionalEnum.L;
                spriteRenderer.flipX = false;
                localState.moveTo = new Vector2(transform.position.x - 12, transform.position.y);
            }
        }

        if (patrol == null) {
            patrol = Patrol();
            StartCoroutine(patrol);
        }
    }

    private void ManageMove() {
        if (!CanMoveFront()) {
            MoveEnnemy();
        }
    }

    protected void GoTo(Vector2 newPosition) {
        localState.moveTo = newPosition;
    }

    public virtual void Update() {

        // manage possibility by priority

        ManageSee();

        if (!localState.seePlayer) {
            ManageEar();
        }

        if (!localState.onAlert) {
            ManagePatrol();
        }

        if (patrol == null) {
            ManageMove();
        }

    }

    private void FixedUpdate() {

        if (localState.moveTo != Vector2.zero) {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(localState.moveTo.x, transform.position.y), moveSpeed * Time.deltaTime);
        }

    }

    void Start() {
        patrol = Patrol();
        StartCoroutine(patrol);
    }

    private void StopPatrol() {
        if (patrol != null) {
            StopCoroutine(patrol);
            patrol = null;
        }
    }

    public virtual void ViewEnnemy(Vector3 viewObjectPositon) {
        moveTo = viewObjectPositon;
        localState.moveTo = moveTo;
    }

    public virtual void MoveEnnemy() {
        if (localState.moveDirection == DirectionalEnum.L) {
            localState.moveDirection = DirectionalEnum.R;
            spriteRenderer.flipX = true;
        } else {
            localState.moveDirection = DirectionalEnum.L;
            spriteRenderer.flipX = false;
        }
    }

    public void Ear() {

    }

    protected bool CanMoveFront() {
        if (!localState.collisionState.groundInFront || localState.collisionState.right && localState.moveDirection == DirectionalEnum.R || localState.collisionState.left && localState.moveDirection == DirectionalEnum.L) {
            return false;
        }
        return true;
    }

    protected bool CanEscape(Vector2 ennemyPosition) {
        // if ennemy front of left and obstable on right
        if (transform.position.x > ennemyPosition.x && localState.moveDirection == DirectionalEnum.L && (!localState.collisionState.groundInBack || localState.collisionState.right)) {
            return false;
        }
        // if ennemy front of right and obstable on left
        if (transform.position.x < ennemyPosition.x && localState.moveDirection == DirectionalEnum.R && (!localState.collisionState.groundInBack || localState.collisionState.left)) {
            return false;
        }
        return true;
    }


    protected void EscapeFromEnnemy(Vector2 ennemyPosition) {
        bool rightOnEnnemy = transform.position.x > ennemyPosition.x;
        bool leftOnEnnemy = transform.position.x < ennemyPosition.x;
        if (rightOnEnnemy) {
            localState.moveDirection = DirectionalEnum.R;
            spriteRenderer.flipX = true;
        }
        if (leftOnEnnemy) {
            localState.moveDirection = DirectionalEnum.L;
            spriteRenderer.flipX = false;
        }
        localState.moveTo = rightOnEnnemy ? new Vector2(transform.position.x + 1, transform.position.y) : new Vector2(transform.position.x - 1, transform.position.y);
    }


    protected bool AnalyseEscapePossibility(Vector2 ennemyPosition) {
        if (Mathf.Abs(ennemyPosition.x - transform.position.x) < 2) {
            return true;
        }
        return false;
    }

    protected Vector2 EscapeEnnemy(Vector2 ennemyPosition) {
        return localState.moveTo = transform.position.x > ennemyPosition.x ? new Vector2(ennemyPosition.x - 0.5f, transform.position.y) : new Vector2(ennemyPosition.x + 0.5f, transform.position.y);
    }

    private IEnumerator Patrol() {
        if (transform.position.x == localState.startPoint.x + 3) {
            MoveEnnemy();
            localState.moveTo = new Vector2(localState.startPoint.x - 3, transform.position.y);
        } else {
            MoveEnnemy();
            localState.moveTo = new Vector2(localState.startPoint.x + 3, transform.position.y);
        }
        yield return new WaitForSeconds(3);
    }

}
