using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyesIA : MonoBehaviour {

    private int viewRange;
    [SerializeField] private LayerMask layer;
    private bool seePlayer;
    private LocalState state;

    public void Setup(int viewRange, LocalState state) {
        this.state = state;
        this.viewRange = viewRange;
        StartCoroutine(See());
    }

    private bool ManageSeeState(Collider2D currentHit) {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, (currentHit.transform.position - transform.position), viewRange, layer);
        Debug.DrawRay(transform.position, (currentHit.transform.position - transform.position), Color.yellow);
        if (hit && hit.transform.CompareTag("Player")) {
            state.seePlayer = true;
            state.playerPositon = new Vector3(hit.transform.position.x, hit.transform.position.x, hit.transform.position.z);
            return true;
        }
        return false;
    }

    private IEnumerator See() {
        while (true) {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, viewRange);
            int i = 0;
            bool findPlayer = false;
            while (i < hits.Length) {
                if (hits[i].transform.CompareTag("Player")) {
                    float hitPosX = hits[i].transform.position.x;
                    if (!state.onAlert && (state.flipDirection == DirectionalEnum.L && hitPosX < transform.position.x || state.flipDirection == DirectionalEnum.R && hitPosX > transform.position.x)) {
                        findPlayer = ManageSeeState(hits[i]);
                    }
                    if (state.onAlert) {
                        findPlayer = ManageSeeState(hits[i]);
                    }
                }
                i++;
            }
            if (!findPlayer) {
                state.seePlayer = false;
                state.playerPositon = Vector3.zero;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void OnDisable() {
        StopAllCoroutines();
    }

}