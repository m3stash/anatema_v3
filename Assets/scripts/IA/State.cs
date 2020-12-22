using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : MonoBehaviour {

    protected DetectColliders colliders;
    protected Vector3 moveTo;
    protected LocalState localState;
    private float moveSpeed;
    private IEnumerator patrol;
    private SpriteRenderer spriteRenderer;

    public virtual void Init(LocalState localState, EnnemyConfig config) {
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

    public virtual void Update() {

        FlipPosition();

        if (localState.seePlayer) {
            if (patrol != null) {
                StopCoroutine(patrol);
                patrol = null;
            }
            View(localState.playerPositon, transform.position);
        }

        if (!localState.seePlayer && localState.onAlert) {
            View(Vector3.zero, transform.position);
        }

        // TODO améliorer ça : permet de gérer le fait d'être arrivé ou on voulait le transform.position == localState.moveTo ne fonctionnant pas à cause d'écarts de float de 0.02f
        if (patrol != null && (transform.position - localState.moveTo).x == 0) {
            StopCoroutine(patrol);
            patrol = null;
        }

        if (!localState.onAlert && patrol == null) {
            patrol = Patrol();
            StartCoroutine(patrol);
        }

    }

    private void FixedUpdate() {

        if (localState.moveTo != Vector3.zero) {
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
