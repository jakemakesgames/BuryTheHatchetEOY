﻿using UnityEngine;
using StateMachine;

//Mark Phillips
//Created 13/08/2018
//Last edited 15/08/2018

public class Stationary : IState<AI>
{
    public void Enter(AI a_owner)
    {
        Debug.Log("Entering Stationary");
    }

    public void Exit(AI a_owner)
    {
        Debug.Log("Exit Stationary");
    }

    public void Update(AI a_owner)
    {
        a_owner.GetAgent().SetDestination(a_owner.transform.position);
    }
}