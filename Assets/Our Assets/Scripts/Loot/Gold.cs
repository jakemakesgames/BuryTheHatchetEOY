using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class Gold : MonoBehaviour {

    [SerializeField] private int m_goldDropMin;
    [SerializeField] private int m_goldDropMax;
    private int goldAmount;
    private bool goldFlag = false;

    public void SetGoldFlag(bool a_goldFlag) { goldFlag = a_goldFlag; }
    // Use this for initialization

    private void Awake()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.tag == "Player")
        {
            if (goldFlag)
            {
                goldAmount = m_goldDropMin;
            }
            else
            {
                goldAmount = m_goldDropMax;
            }

            collider.transform.GetComponent<Player>().SetMoney(goldAmount);
            Destroy(gameObject);
        }
    }
}
