using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Michael Corben
//Created 13/08/2018
//Last edited 08/10/2018

public class RicochetObstacle : MonoBehaviour, IDamagable {
    
    private Vector3 m_projectilePosition;

    public void TakeDamage(int a_damage) {

    }
    public void TakeHit(int a_damage, RaycastHit a_hit) {
        TakeDamage(a_damage);
    }

    //When hit by a projectile this will redirect it 
    //Along the normal of the point of collision imitating a ricochet
    public void TakeImpact(int a_damage, RaycastHit a_hit, Projectile a_projectile) {
        Vector3 incomingVec = a_hit.point - a_projectile.transform.position;
        Vector3 reflectVec = Vector3.Reflect(incomingVec, a_hit.normal);

        a_projectile.transform.forward = (reflectVec);
        a_projectile.transform.position = a_hit.point;

        TakeHit(a_damage, a_hit);
    }
}
