using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;

public class Wander : IState<AI>
{
    public void Enter(AI _owner)
    {
        Debug.Log("Entering Wander");
    }

    public void Exit(AI _owner)
    {
        Debug.Log("Exiting Wander");
    }

    public void Update(AI _owner)
    {

    }
}
