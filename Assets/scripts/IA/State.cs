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
        if (localState.flipDirection != DirectionalEnum.L && transform.position.x > localState.moveTo.x) {
            localState.flipDirection = DirectionalEnum.L;
            spriteRenderer.flipX = false;
        }
        if (localState.flipDirection != DirectionalEnum.R && transform.position.x < localState.moveTo.x) {
            localState.flipDirection = DirectionalEnum.R;
            spriteRenderer.flipX = true;
        }
    }

    private void ManageSeeState() {
        if (!localState.canSee)
            return;
        if (localState.seePlayer) {
            if (patrol != null) {
                StopCoroutine(patrol);
                patrol = null;
            }
            /*if (!localState.colliders.OnNextTileCollision()) {
                // if(can falling without die) else ...
                // or(can jump) else ...
                float newPosX;
                if (!spriteRenderer.flipX) {
                    newPosX = localState.playerPositon.x - 10;
                } else {
                    newPosX = localState.playerPositon.x + 10;
                }
                View(new Vector2(newPosX, localState.playerPositon.y), transform.position);
            } else if (localState.colliders.OnRightCollision()) {
                // if(can jump) else ...
                float newPosX;
                if (!spriteRenderer.flipX) {
                    newPosX = localState.playerPositon.x - 10;
                } else {
                    newPosX = localState.playerPositon.x + 10;
                }
                View(new Vector2(newPosX, localState.playerPositon.y), transform.position);
            } else {
                View(localState.playerPositon, transform.position);
            }*/
            View(localState.playerPositon, transform.position);
        } else if (localState.onAlert) {
            View(Vector2.zero, transform.position);
        }
    }

    private void ManagePatrolState() {
        if (!localState.canPatrol)
            return;
        // TODO améliorer ça : permet de gérer le fait d'être arrivé ou on voulait le transform.position == localState.moveTo ne fonctionnant pas à cause d'écarts de float de 0.02f
        if (patrol != null && (transform.position.x - localState.moveTo.x) == 0) {
            StopCoroutine(patrol);
            patrol = null;
        }

        if (!localState.onAlert && patrol == null) {
            patrol = Patrol();
            StartCoroutine(patrol);
        }
    }

    public virtual void Update() {

        FlipPosition();
        ManageSeeState();
        ManagePatrolState();

    }

    private void FixedUpdate() {

        if (localState.moveTo != Vector2.zero) {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(localState.moveTo.x, transform.position.y), moveSpeed * Time.deltaTime);
        }

    }


    void Start() {
        StartCoroutine(Patrol());
    }

    public virtual void View(Vector3 viewObjectPositon, Vector3 position) {
        moveTo = viewObjectPositon;
        localState.moveTo = moveTo;
    }

    public void Ear() {

    }

    public void GoTo(Vector3 goToPosition, LocalState localState) {
        localState.moveTo = goToPosition;
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
