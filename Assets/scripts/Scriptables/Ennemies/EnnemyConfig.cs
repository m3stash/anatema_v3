using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyConfig : ScriptableObject {

    [Header("Main Settings")]
    [SerializeField] private EnnemyAgresivityTypeEnum ennemyAgresivityType;
    [SerializeField] private bool canSee;
    [SerializeField] private bool canEar;
    [SerializeField] private int viewRange;
    [SerializeField] private float speed;
    [SerializeField] private int alertDuration;

    public EnnemyAgresivityTypeEnum EnnemyAgresivityType() {
        return ennemyAgresivityType;
    }

    public bool EnnemyCanSee() {
        return canSee;
    }

    public bool EnnemyCanEar() {
        return canEar;
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
