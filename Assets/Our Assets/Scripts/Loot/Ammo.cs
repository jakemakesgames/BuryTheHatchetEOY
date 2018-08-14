using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class Ammo : MonoBehaviour {

    [Tooltip("Amount of ammo to drop.")]
    [SerializeField] private int m_ammoAmount;

    private void Awake()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.tag == "Player")
        {
            collider.transform.GetComponent<WeaponController>().GetEquippedGun().AddAmmo(m_ammoAmount);
            Destroy(gameObject);
        }
    }
}
