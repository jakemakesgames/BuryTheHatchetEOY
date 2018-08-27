using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour {

    public void ChangeToDeathCollider()
    {
        GetComponentInParent<CapsuleCollider>().radius = 0.2f;
        GetComponentInParent<CapsuleCollider>().height = 0.0f;
    }

    public void SetToKinematic()
    {
        GetComponentInParent<Rigidbody>().isKinematic = true;
    }
}
