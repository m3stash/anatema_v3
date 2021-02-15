using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionState {
    public bool top;
    public bool bottom;
    public bool left;
    public bool right;
    public bool noGround;
}

public class LocalState {
    public bool seePlayer = false;
    public Vector2 playerPositon = Vector3.zero;
    public bool earPlayer = false;
    public bool onAlert = false;
    public Vector2 moveTo;
    public bool startPatrol = true;
    public Vector2 startPoint;
    public DirectionalEnum moveDirection;
    public CollisionState collisionState;
    public bool canEar;
    public bool canSee;
    public bool canPatrol;
}

public class IA : MonoBehaviour {

    private LocalState localState;
    private State currentState;

    [SerializeField] private GameObject eyes;
    [SerializeField] private EnnemyConfig config;
    [SerializeField] private HealthBar healthBar;

    private EnnemyAgresivityTypeEnum agressivityType;
    private EyesIA eyesIa;
    private Coroutine alertCoroutine;

    private void Awake() {
        bool canEar = config.EnnemyCanSee() || false;
        bool canSee = config.EnnemyCanEar() || false;

        DetectColliders colliders = GetComponent<DetectColliders>();

        localState = new LocalState {
            startPoint = transform.position,
            canEar = canEar,
            canSee = canSee,
            canPatrol = config.EnnemyCanPatrol(),
            collisionState = new CollisionState(),
        };


        colliders.Init(localState);

        if (canEar) {
            //
        }
        if (canSee) {
            eyes.SetActive(true);
            eyesIa = eyes.GetComponent<EyesIA>();
            eyesIa.Setup(config.ViewRange(), localState);
        }

        healthBar.SetHealth(config.Health());

        agressivityType = config.EnnemyAgresivityType();

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
        if (alertCoroutine == null && localState.seePlayer && !localState.onAlert) {
            switch (agressivityType) {
                case EnnemyAgresivityTypeEnum.AGRESSIVE:
                Destroy(currentState);
                localState.onAlert = true;
                localState.startPatrol = false;
                // state new AgressiveAlertState();
                break;
                case EnnemyAgresivityTypeEnum.FEARFULL:
                Destroy(currentState);
                localState.onAlert = true;
                localState.startPatrol = false;
                currentState = gameObject.AddComponent<FearFullAlertState>();
                break;
                case EnnemyAgresivityTypeEnum.PASSIVE:
                // toDo => do nothing if passive IA not Agressed ?!? 
                // state = new PassiveAlterState();
                break;
            }
            currentState.Init(localState, config);
        }
        if (alertCoroutine == null && !localState.seePlayer && localState.onAlert) {
            alertCoroutine = StartCoroutine(endAlertCountdown());
        }
        if (alertCoroutine != null && localState.seePlayer) {
            StopCoroutine(alertCoroutine);
            alertCoroutine = null;
        }
    }

    private IEnumerator endAlertCountdown() {
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
