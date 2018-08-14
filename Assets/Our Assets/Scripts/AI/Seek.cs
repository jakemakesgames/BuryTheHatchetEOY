using UnityEngine;
using StateMachine;

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
        a_owner.transform.LookAt(a_owner.GetPlayerPos());
        a_owner.SetDestination(a_owner.GetPlayerPos());
    }
}
