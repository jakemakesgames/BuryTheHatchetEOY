using StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peek : IState<AI> {

    int m_direction = 0;
    public void Enter(AI a_owner)
    {
        m_direction = Random.Range(0, 100);
    }

    public void Exit(AI a_owner)
    {
    }

    public void Update(AI a_owner)
    {
        if (m_direction <= 50)
        {
            a_owner.Agent.SetDestination(a_owner.transform.position - a_owner.transform.right);
        }

        if (m_direction > 50)
        {
            a_owner.Agent.SetDestination(a_owner.transform.position + a_owner.transform.right);
        }
    }
}
