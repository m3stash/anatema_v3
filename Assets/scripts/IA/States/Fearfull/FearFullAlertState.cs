using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FearFullAlertState : State {

    public override void View(Vector3 viewObjectPositon, Vector3 position) {
        if (position.x > viewObjectPositon.x) {
            moveTo = viewObjectPositon + position;
        } else {
            moveTo = viewObjectPositon - position;
        }
        localState.moveTo = moveTo;
    }

}