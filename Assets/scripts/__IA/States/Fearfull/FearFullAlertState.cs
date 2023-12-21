using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FearFullAlertState : State {

    private IEnumerator onEscape;
    private Vector2 escapeTo;

    public override void ViewEnnemy(Vector3 ennemyPosition) {
        if (onEscape == null) {
            if (CanEscape(ennemyPosition)) {
                EscapeFromEnnemy(ennemyPosition);
            } else {
                if (AnalyseEscapePossibility(ennemyPosition)) {
                    escapeTo = EscapeEnnemy(ennemyPosition);
                    GoTo(escapeTo);
                    onEscape = OnEscape();
                    StartCoroutine(onEscape);
                } else {
                    GoTo(transform.position);
                }
            }
        }
    }

    private IEnumerator OnEscape() {
        while ((Vector2)transform.position != escapeTo) {
            yield return new WaitForSeconds(0.1f);
        }
        escapeTo = Vector2.zero;
        StopCoroutine(onEscape);
        onEscape = null;
    }
}