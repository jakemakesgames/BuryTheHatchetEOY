using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class Health : MonoBehaviour {

    [Tooltip("Amount of health to drop.")]
    [SerializeField] private int m_healthAmount;
    private void Awake()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.tag == "Player")
        {
            collider.transform.GetComponent<Player>().Heal(m_healthAmount);
            Destroy(gameObject);
        }
    }

}
