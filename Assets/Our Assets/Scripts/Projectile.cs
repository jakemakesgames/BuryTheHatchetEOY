using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Michael Corben
//Based on Tutorial:https://www.youtube.com/watch?v=rZAnnyensgs&list=PLFt_AvWsXl0ctd4dgE1F8g3uec4zKNRV0&index=3
//Created 24/07/2018
//Last edited 13/08/2018


public class Projectile : MonoBehaviour {

    private float m_lifeTime = 20 /*seconds*/;
    private float m_skinWidth = 0.1f;
    private float m_speed = 10;
    private int m_damage = 1;
    private LayerMask m_entityCollisionMask;
    private LayerMask m_environmentCollisionMask;
    private LayerMask m_ricochetCollisionMask;

    private void Start() {
        Collider[] initialEnemyCollision = Physics.OverlapSphere(transform.position, .1f, m_entityCollisionMask);
        if (initialEnemyCollision.Length > 0) {
            OnHitObject(initialEnemyCollision[0]);
        }
        Collider[] initialEnvironmentCollision = Physics.OverlapSphere(transform.position, .1f, m_environmentCollisionMask);
        if (initialEnvironmentCollision.Length > 0) {
            OnHitObject(initialEnvironmentCollision[0]);
        }
        Collider[] initialRicoCchetCollision = Physics.OverlapSphere(transform.position, .1f, m_ricochetCollisionMask);
        if (initialRicoCchetCollision.Length > 0) {
            OnHitObject(initialRicoCchetCollision[0]);
        }
    }

    public void SetSpeed(float a_speed) {
        m_speed = a_speed;
    }

    public void SetDamage(int a_damage) {
        m_damage = a_damage;
    }

    public void SetEntityCollisionLayer(LayerMask a_collsionMask) {
        m_entityCollisionMask = a_collsionMask;
    }
    public void SetTerrainCollisionLayer(LayerMask a_collsionMask) {
        m_environmentCollisionMask = a_collsionMask;
    }
    public void SetRicochetCollisionLayer(LayerMask a_collsionMask) {
        m_ricochetCollisionMask = a_collsionMask;
    }
    public void SetLifeTime(float a_lifeTime) {
        m_lifeTime = a_lifeTime;
    }

    //moves the projectile also counts down till this should be destroyedad
    public void Update () {
        float moveDistance = m_speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
        if (m_lifeTime <= 0) {
            Destroy(gameObject);
        }
        m_lifeTime -= Time.deltaTime;
	}

    //use ray casts to check for collisions
    private void CheckCollisions(float a_distanceToMove) {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, a_distanceToMove + m_skinWidth, m_entityCollisionMask)) {
            OnHitObject(hit);
        }
        if (Physics.Raycast(ray, out hit, a_distanceToMove + m_skinWidth, m_environmentCollisionMask)) {
            OnHitObject(hit);
        }
        if (Physics.Raycast(ray, out hit, a_distanceToMove + m_skinWidth, m_ricochetCollisionMask))
        {
            OnHitObject(hit, this);
        }
    }

    private void OnHitObject(RaycastHit a_hit) {
        IDamagable damagableObject = a_hit.collider.GetComponent<IDamagable>();
        if (damagableObject != null) {
            damagableObject.TakeHit(m_damage, a_hit);
        }
        Destroy(gameObject);
    }
    private void OnHitObject(Collider a_c) {
        IDamagable damagableObject = a_c.GetComponent<IDamagable>();
        if (damagableObject != null) {
            damagableObject.TakeDamage(m_damage);
        }
        Destroy(gameObject);
    }
    private void OnHitObject(RaycastHit a_hit, Projectile a_bullet) {
        IDamagable damagableObject = a_hit.collider.GetComponent<IDamagable>();
        if (damagableObject != null) {
            damagableObject.TakeImpact(m_damage, a_hit, a_bullet);
        }
    }
}
