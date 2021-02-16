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

    private void FlipPosition() {
        if (localState.moveDirection != DirectionalEnum.L && transform.position.x > localState.moveTo.x) {
            localState.moveDirection = DirectionalEnum.L;
            spriteRenderer.flipX = false;
        }
        if (localState.moveDirection != DirectionalEnum.R && transform.position.x < localState.moveTo.x) {
            localState.moveDirection = DirectionalEnum.R;
            spriteRenderer.flipX = true;
        }
    }

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

    private void ManagePatrolState() {
        if (!localState.canPatrol)
            return;
        /*
        * transform.position == localState.moveTo ne fonctionnant pas à cause d'écarts de float de 0.02f
        */
        if (patrol != null && (transform.position.x - localState.moveTo.x) == 0) {
            StopCoroutine(patrol);
            patrol = null;
        }

        if (!localState.onAlert && patrol == null) {
            patrol = Patrol();
            StartCoroutine(patrol);
        }
    }

    protected bool CanEscape() {
        if (!localState.collisionState.noGround || localState.collisionState.right && localState.moveDirection == DirectionalEnum.R || localState.collisionState.left && localState.moveDirection == DirectionalEnum.L) {
            return false;
        }
        return true;
    }

    protected void Escape(Vector2 ennemyPosition) {
        localState.moveTo = transform.position.x > ennemyPosition.x ? new Vector2(ennemyPosition.x + 10, transform.position.y) : new Vector2(ennemyPosition.x - 10, transform.position.y);
    }

    protected void GoTo(Vector2 newPosition) {
        localState.moveTo = newPosition;
    }

    protected bool AnalyseEscapePossibility(Vector2 ennemyPosition) {
        if(Mathf.Abs(ennemyPosition.x - transform.position.x) < 2) {
            return true;
        }
        return false;
    }

    protected Vector2 EscapeEnnemy(Vector2 ennemyPosition) {
        return localState.moveTo = transform.position.x > ennemyPosition.x ? new Vector2(ennemyPosition.x - 1, transform.position.y) : new Vector2(ennemyPosition.x + 1, transform.position.y);
    }

    public virtual void Update() {

        FlipPosition();
        ManageSee();
        ManageEar();
        ManagePatrolState();

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

    public void Ear() {

    }

    private IEnumerator Patrol() {
        if (transform.position.x == localState.startPoint.x + 3) {
            localState.moveTo = new Vector2(localState.startPoint.x - 3, transform.position.y);
        } else {
            localState.moveTo = new Vector2(localState.startPoint.x + 3, transform.position.y);
        }
        yield return new WaitForSeconds(3);
    }

}
