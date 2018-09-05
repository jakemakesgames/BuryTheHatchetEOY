using UnityEngine;
using StateMachine;
using UnityEngine.AI;

public class Reload : IState<AI>
{
    public void Enter(AI a_owner)
    {
    }

    public void Exit(AI a_owner)
    {
    }

    public void Update(AI a_owner)
    {
        ReloadGun(a_owner);
    }
	
    void ReloadGun(AI a_owner)
    {
        if (a_owner.Gun.ReloadOne())
        {
            a_owner.Agent.SetDestination(a_owner.transform.position);
            a_owner.FinishedReload = false;
            //Set anim bool
        }
        else
        {
            a_owner.FinishedReload = true;
            //a_owner.AtCover = false;
        }
    }
}
