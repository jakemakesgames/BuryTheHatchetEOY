﻿using UnityEngine;
using StateMachine;

public class Flee : IState<AI>
{ 
    public void Enter(AI a_owner)
    {
        Debug.Log("Entering Flee");
    }

    public void Exit(AI a_owner)
    {
        Debug.Log("Exiting Flee");
    }

    public void Update(AI a_owner)
    {
        Vector3 vecBetween = (a_owner.GetPlayerPos() - a_owner.transform.position);
        a_owner.GetAgent().SetDestination(a_owner.transform.position - vecBetween);
    }
}

