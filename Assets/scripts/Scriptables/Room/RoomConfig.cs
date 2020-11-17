using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomConfig : ScriptableObject {
    [Header("Main Settings")]

    [SerializeField] private RoomShapeEnum RoomShape;
    [SerializeField] private bool disable_door_T = false;
    [SerializeField] private bool disable_door_TL = false;
    [SerializeField] private bool disable_door_TR = false;
    [SerializeField] private bool disable_door_B = false;
    [SerializeField] private bool disable_door_BL = false;
    [SerializeField] private bool disable_door_BR = false;
    [SerializeField] private bool disable_door_L = false;
    [SerializeField] private bool disable_door_LT = false;
    [SerializeField] private bool disable_door_LB = false;
    [SerializeField] private bool disable_door_R = false;
    [SerializeField] private bool disable_door_RT = false;
    [SerializeField] private bool disable_door_RB = false;

    public bool GetDisable_door_T() {
        return disable_door_T;
    }
    public bool GetDisable_door_TL() {
        return disable_door_TL;
    }
    public bool GetDisable_door_TR() {
        return disable_door_TR;
    }
    public bool GetDisable_door_B() {
        return disable_door_B;
    }
    public bool GetDisable_door_BL() {
        return disable_door_BL;
    }
    public bool GetDisable_door_BR() {
        return disable_door_BR;
    }
    public bool GetDisable_door_L() {
        return disable_door_L;
    }
    public bool GetDisable_door_LT() {
        return disable_door_LT;
    }
    public bool GetDisable_door_LB() {
        return disable_door_LB;
    }
    public bool GetDisable_door_R() {
        return disable_door_R;
    }
    public bool GetDisable_door_RT() {
        return disable_door_RT;
    }

    public bool GetDisable_door_RB() {
        return disable_door_RB;
    }
}

