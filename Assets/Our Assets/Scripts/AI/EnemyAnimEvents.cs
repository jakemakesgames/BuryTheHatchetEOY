using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimEvents : MonoBehaviour {

    public void HasDropTrigger()
    {
        GetComponentInParent<AI>().HasDroppedTrigger = true;
    }

    public void Shoot()
    {

    }

    public void CanShoot()
    {

    }
}
