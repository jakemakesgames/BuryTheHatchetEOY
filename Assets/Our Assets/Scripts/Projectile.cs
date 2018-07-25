using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Michael Corben
//Based on Tutorial:https://www.youtube.com/watch?v=rZAnnyensgs&list=PLFt_AvWsXl0ctd4dgE1F8g3uec4zKNRV0&index=3
//Created 24/07/2018
//Last edited 25/07/2018


public class Projectile : MonoBehaviour {

    private float m_lifeTime = 20 /*seconds*/;
    private float m_speed = 10;
    private int m_damage = 1;
    private LayerMask collisionMask;

    public void SetSpeed(float a_speed) {
        m_speed = a_speed;
    }

    public void SetDamage(int a_damage) {
        m_damage = a_damage;
    }

    public void SetCollisionLayer(LayerMask a_collsionMask) {
        collisionMask = a_collsionMask;
    }

    public void Update () {
        float moveDistance = m_speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
	}

    private void CheckCollisions(float a_distanceToMove) {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, a_distanceToMove, collisionMask)) {
            OnHitObject(hit);
        }
    }

    private void OnHitObject(RaycastHit a_hit) {
        IDamagable damagableObject = a_hit.collider.GetComponent<IDamagable>();
        if (damagableObject != null) {
            damagableObject.TakeHit(m_damage, a_hit);
        }
        Destroy(gameObject);
    }
}
