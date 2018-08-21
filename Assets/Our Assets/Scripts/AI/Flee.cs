using UnityEngine;
using StateMachine;

//Mark Phillips
//Created 13/08/2018
//Last edited 15/08/2018

public class Flee : IState<AI>
{ 
    public void Enter(AI a_owner)
    {
    }

    public void Exit(AI a_owner)
    {
    }

    public void Update(AI a_owner)
    {
        Vector3 vecBetween = (a_owner.PlayerPosition - a_owner.transform.position);
        a_owner.Agent.SetDestination(a_owner.transform.position - vecBetween);
    }
}

