using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalState {
    public bool seePlayer = false;
    public Vector3 playerPositon = Vector3.zero;
    public bool earPlayer = false;
    public bool onAlert = false;
    public Vector3 moveTo;
    public bool startPatrol = true;
    public Vector3 startPoint;
    public DirectionalEnum flipDirection;
}

public class IA : MonoBehaviour {

    private LocalState localState;
    private State currentState;

    [SerializeField] private GameObject eyes;
    [SerializeField] private EnnemyConfig config;

    private DetectColliders colliders;
    private EnnemyAgresivityTypeEnum agressivityType;
    private EyesIA eyesIa;
    private Coroutine endAlertCoroutine;

    private void Awake() {
        colliders = GetComponent<DetectColliders>();
        agressivityType = config.EnnemyAgresivityType();
        localState = new LocalState {
            startPoint = transform.position
        };
        if (config.EnnemyCanEar()) {

        }
        if (config.EnnemyCanSee()) {
            eyes.SetActive(true);
            eyesIa = eyes.GetComponent<EyesIA>();
            eyesIa.Setup(config.ViewRange(), localState);
        }
        switch (agressivityType) {
            case EnnemyAgresivityTypeEnum.AGRESSIVE:
            // state = new AgressiveCalmState();
            break;
            case EnnemyAgresivityTypeEnum.FEARFULL:
            currentState = gameObject.AddComponent<FearFullCalmState>();
            break;
            case EnnemyAgresivityTypeEnum.PASSIVE:
            // state = new PassiveCalmState();
            break;
        }
        currentState.Init(localState, config);
    }

    private void Update() {
        if (endAlertCoroutine == null && localState.seePlayer && !localState.onAlert) {
            localState.onAlert = true;
            localState.startPatrol = false;
            Destroy(currentState);
            switch (agressivityType) {
                case EnnemyAgresivityTypeEnum.AGRESSIVE:
                // state new AgressiveAlertState();
                break;
                case EnnemyAgresivityTypeEnum.FEARFULL:
                currentState = gameObject.AddComponent<FearFullAlertState>();
                break;
                case EnnemyAgresivityTypeEnum.PASSIVE:
                // state = new PassiveAlterState();
                break;
            }
            currentState.Init(localState, config);
        }
        if (endAlertCoroutine == null && !localState.seePlayer && localState.onAlert) {
            endAlertCoroutine = StartCoroutine(EndAlertCountdown());
        }
        if (localState.seePlayer) {
            if (endAlertCoroutine != null) {
                StopCoroutine(endAlertCoroutine);
                endAlertCoroutine = null;
            }
        }
    }

    private IEnumerator EndAlertCountdown() {
        float duration = config.AlertCountdownDuration();
        float totalTime = 0;
        while (totalTime <= duration) {
            totalTime += Time.deltaTime;
            yield return null;
        }
        localState.onAlert = false;
        Destroy(currentState);
        switch (agressivityType) {
            case EnnemyAgresivityTypeEnum.AGRESSIVE:
            //  new AgressiveCalmState();
            break;
            case EnnemyAgresivityTypeEnum.FEARFULL:
            currentState = gameObject.AddComponent<FearFullCalmState>();
            break;
            case EnnemyAgresivityTypeEnum.PASSIVE:
            // new PassiveCalmState();
            break;
        }
        currentState.Init(localState, config);
    }

}
