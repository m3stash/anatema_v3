using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FearFullAlertState : State {

    private bool goToOpposite = false;
    private Vector2 newDirection;

    public override void View(Vector3 viewObjectPositon, Vector3 position) {
        if (localState.seePlayer) {
            if (goToOpposite) {
                if (transform.position.x == newDirection.x) {
                    goToOpposite = false;
                }
                return;
            }
            if (!localState.colliders.OnNextTileCollision()) {
                // if(can falling without die) else ...
                // or(can jump) else ...
                float newPosX;
                if (transform.position.x > localState.playerPositon.x) {
                    newPosX = localState.playerPositon.x - 1;
                } else {
                    newPosX = localState.playerPositon.x + 1;
                }
                goToOpposite = true;
                newDirection = new Vector2(newPosX, transform.position.y);
                localState.moveTo = newDirection;
                // if ennemy move to right and detect wall 
            } else if (localState.colliders.OnRightCollision()) {
                // if(can jump) else ...
                goToOpposite = true;
                newDirection = new Vector2(localState.playerPositon.x - 1, transform.position.y);
                localState.moveTo = newDirection;
                // if ennemy move to left and detect wall 
            } else if (localState.colliders.OnLeftCollision()) {
                goToOpposite = true;
                newDirection = new Vector2(localState.playerPositon.x + 1, transform.position.y);
                localState.moveTo = newDirection;
            } else {
                float newPosX;
                if (transform.position.x > localState.playerPositon.x) {
                    newPosX = localState.playerPositon.x + 10;
                } else {
                    newPosX = localState.playerPositon.x - 10;
                }
                localState.moveTo = new Vector2(newPosX, transform.position.y);
            }
        } else {
            if (position.x > viewObjectPositon.x) {
                moveTo = viewObjectPositon + position;
            } else {
                moveTo = viewObjectPositon - position;
            }
            localState.moveTo = moveTo;
        }


        /*if (position.x > viewObjectPositon.x) {
            moveTo = viewObjectPositon + position;
        } else {
            moveTo = viewObjectPositon - position;
        }
        localState.moveTo = moveTo;*/
    }

}