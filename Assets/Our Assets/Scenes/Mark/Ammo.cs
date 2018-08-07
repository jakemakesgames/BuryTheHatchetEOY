using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Ammo : MonoBehaviour {

    [Tooltip("Amount of ammo to drop.")]
    [SerializeField] private int m_ammoAmount;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
