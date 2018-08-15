using UnityEngine;
using StateMachine;

//Mark Phillips
//Created 13/08/2018
//Last edited 15/08/2018

public class Seek : IState<AI>
{
    public void Enter(AI a_owner)
    {
        Debug.Log("Entering Seek");
    }

    public void Exit(AI a_owner)
    {
        Debug.Log("Exiting Seek");
    }

    public void Update(AI a_owner)
    {
        a_owner.Agent.SetDestination(a_owner.PlayerPosition);
    }
}
