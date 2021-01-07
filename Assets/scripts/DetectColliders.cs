using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectColliders : MonoBehaviour {

    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Vector2 leftOffset;
    [SerializeField] private Vector2 topOffset;
    [SerializeField] private Vector2 rightOffset;
    [SerializeField] private Vector2 bottomOffset;
    [SerializeField] private Vector2 nextTileOffset;
    private float radiusOffset = 0.05f;
    public bool topCollider;
    public bool bottomCollider;
    public bool leftCollider;
    public bool rightCollider;
    public bool nextTileCollider;

    void Update() {
        Vector2 position = transform.position;
        topCollider = Physics2D.OverlapCircle(position + topOffset, radiusOffset, wallLayer);
        bottomCollider = Physics2D.OverlapCircle(position + bottomOffset, radiusOffset, wallLayer);
        leftCollider = Physics2D.OverlapCircle(position + leftOffset, radiusOffset, wallLayer);
        rightCollider = Physics2D.OverlapCircle(position + rightOffset, radiusOffset, wallLayer);
        nextTileCollider = Physics2D.OverlapCircle(position + nextTileOffset, radiusOffset, wallLayer);
    }

    public bool IsTopCollision() {
        return topCollider;
    }

    public bool IsBottomCollision() {
        return bottomCollider;
    }

    public bool OnRightCollision() {
        return rightCollider;
    }

    public bool OnLeftCollision() {
        return leftCollider;
    }

    public bool OnNextTileCollision() {
        return nextTileCollider;
    }


    private void OnDrawGizmos() {
        Vector2 position = transform.position;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(position + topOffset, radiusOffset);
        Gizmos.DrawWireSphere(position + bottomOffset, radiusOffset);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(position + rightOffset, radiusOffset);
        Gizmos.DrawWireSphere(position + leftOffset, radiusOffset);
        Gizmos.DrawWireSphere(position + nextTileOffset, radiusOffset);
    }

}
