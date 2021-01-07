using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyConfig : ScriptableObject {

    [Header("Capacity")]
    [SerializeField] private bool canSee;
    [SerializeField] private bool canEar;
    [SerializeField] private bool canPatrol;
    [Header("Config")]
    [SerializeField] private EnnemyCategoryEnum ennemyCategoryType;
    [SerializeField] private EnnemyAgresivityTypeEnum ennemyAgresivityType;
    [SerializeField] private int viewRange;
    [SerializeField] private float speed;
    [SerializeField] private int alertDuration;

    public bool EnnemyCanSee() {
        return canSee;
    }
    public bool EnnemyCanEar() {
        return canEar;
    }

    public bool EnnemyCanPatrol() {
        return canPatrol;
    }

    public EnnemyCategoryEnum EnnemyCategoryType() {
        return ennemyCategoryType;
    }

    public EnnemyAgresivityTypeEnum EnnemyAgresivityType() {
        return ennemyAgresivityType;
    }

    public int ViewRange() {
        return viewRange;
    }

    public float MoveSpeed() {
        return speed;
    }
    public float AlertCountdownDuration() {
        return alertDuration;
    }

}
