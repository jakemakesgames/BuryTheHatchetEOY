using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using StateMachine;

//Mark Phillips
//Created 29/08/2018
//Last edited 29/08/2018

[RequireComponent(typeof(WeaponController))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class AIMelee : MonoBehaviour, IDamagable {

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TakeDamage(int a_damage)
    {
    }

    public void TakeHit(int a_damage, RaycastHit a_hit)
    {
    }

    public void TakeImpact(int a_damage, RaycastHit a_hit, Projectile a_projectile)
    {
    }
}
