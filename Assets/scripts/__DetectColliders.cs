using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectColliders : MonoBehaviour {

    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Vector2 leftOffset;
    [SerializeField] private Vector2 topOffset;
    [SerializeField] private Vector2 rightOffset;
    [SerializeField] private Vector2 bottomOffset;
    [SerializeField] private Vector2 frontTileOffset;
    [SerializeField] private Vector2 backTileOffset;
    private float radiusOffset = 0.05f;
    private LocalState localState;

    public void Init(LocalState localState) {
        this.localState = localState;
    }

    void Update() {
        if (localState == null)
            return;
        Vector2 position = transform.position;
        localState.collisionState.top = Physics2D.OverlapCircle(position + topOffset, radiusOffset, wallLayer);
        localState.collisionState.bottom = Physics2D.OverlapCircle(position + bottomOffset, radiusOffset, wallLayer);
        localState.collisionState.left = Physics2D.OverlapCircle(position + leftOffset, radiusOffset, wallLayer);
        localState.collisionState.right = Physics2D.OverlapCircle(position + rightOffset, radiusOffset, wallLayer);
        Vector2 newPosLeft = localState.moveDirection == DirectionalEnum.R ? new Vector2(position.x + frontTileOffset.x, position.y + frontTileOffset.y) : new Vector2(position.x - frontTileOffset.x, position.y + frontTileOffset.y);
        Vector2 newPosRight = localState.moveDirection == DirectionalEnum.R ? new Vector2(position.x - backTileOffset.x, position.y + backTileOffset.y) : new Vector2(position.x + backTileOffset.x, position.y + backTileOffset.y);
        localState.collisionState.groundInFront = Physics2D.OverlapCircle(newPosLeft, radiusOffset, wallLayer);
        localState.collisionState.groundInBack = Physics2D.OverlapCircle(newPosRight, radiusOffset, wallLayer);
    }

    private void OnDrawGizmos() {
        if (localState == null)
            return;
        Vector2 position = transform.position;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(position + topOffset, radiusOffset);
        Gizmos.DrawWireSphere(position + bottomOffset, radiusOffset);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(position + rightOffset, radiusOffset);
        Gizmos.DrawWireSphere(position + leftOffset, radiusOffset);
        Vector2 newPosLeft = localState.moveDirection == DirectionalEnum.R ? new Vector2(position.x + frontTileOffset.x, position.y + frontTileOffset.y) : new Vector2(position.x - frontTileOffset.x, position.y + frontTileOffset.y);
        Vector2 newPosRight = localState.moveDirection == DirectionalEnum.L ? new Vector2(position.x + backTileOffset.x, position.y + backTileOffset.y) : new Vector2(position.x - backTileOffset.x, position.y + backTileOffset.y);
        Gizmos.DrawWireSphere(newPosLeft, radiusOffset);
        Gizmos.DrawWireSphere(newPosRight, radiusOffset);
    }

}
