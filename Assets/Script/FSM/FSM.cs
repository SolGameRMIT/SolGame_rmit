using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class FSM
{
    public IState currentStateBase;//  Current state base [ener,update,exit]
    public StateType currentStateType;// Current state type
    //Create dictionary with enumeration type StateType as Key
    public Dictionary<StateType, IState> stateDic = new Dictionary<StateType, IState>();
    public GameObject character;

    public FSM()
    {
    
    }


    public void AddState(StateType stateType,IState stateBase)
    {
        if (stateDic.ContainsKey(stateType)) return;
        stateDic.Add(stateType, stateBase);
    }

    public void OnUpdate()
    {
        currentStateBase.OnUpdate();
    }


    public virtual void TransitionState(StateType type)
    {
        if (currentStateBase == stateDic[type]) return;
        currentStateBase?.OnExit();
        currentStateBase = stateDic[type];
        currentStateType = type;
        currentStateBase.OnEnter();
    }

}