using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FearFullAlertState : State {

    private IEnumerator onEscape;
    private Vector2 escapeTo;

    public override void ViewEnnemy(Vector3 ennemyPosition) {
        if (onEscape == null) {
            if (CanEscape()) {
                Escape(ennemyPosition);
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
        /* attention ce débile ne détecte pas l'égalité dans ce genre de cas : 
        * (Vector2)transform.position = "(291.0, 133.3)"
        * escapeTo == "(291.0, 133.3)"
        * (Vector2)transform.position == escapeTo => false....
        * le débile..
        */
        while ((Vector2)transform.position != escapeTo) {
            yield return new WaitForSeconds(0.1f);
        }
        escapeTo = Vector2.zero;
        StopCoroutine(onEscape);
        onEscape = null;
    }
}