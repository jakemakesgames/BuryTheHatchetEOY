using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimEvents : MonoBehaviour {

    public void HasDropTrigger()
    {
        //  GetComponentInParent<CapsuleCollider>().radius = 0.3f;
        //  GetComponentInParent<CapsuleCollider>().height = 0.0f;
        GetComponentInParent<AI>().HasDroppedTrigger = true;
    }

    public void SetToKinematic()
    {
       // GetComponentInParent<Rigidbody>().isKinematic = true;
    }
}
