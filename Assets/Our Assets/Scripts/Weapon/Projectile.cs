using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Michael Corben
//Based on Tutorial:https://www.youtube.com/watch?v=rZAnnyensgs&list=PLFt_AvWsXl0ctd4dgE1F8g3uec4zKNRV0&index=3
//Created 24/07/2018
//Last edited 21/08/2018


public class Projectile : MonoBehaviour {

    private float m_lifeTime = 20 /*seconds*/;
    private float m_skinWidth = 0.1f;
    private float m_speed = 10;
    private int m_damage = 1;
    private bool m_insideEntity;
    private bool m_hasEntered;
    private LayerMask m_entityCollisionMask;
    private LayerMask m_environmentCollisionMask;
    private LayerMask m_ricochetCollisionMask;

    [Tooltip("Life time of the bullet after it hits an entity")]
    [SerializeField] private float m_bulletKillTime = 1f;
    [SerializeField] private ParticleSystem m_enterEntityParticle;
    [SerializeField] private ParticleSystem m_exitEntityParticle;
    [SerializeField] private ParticleSystem m_environmentParticle;
    [SerializeField] private ParticleSystem m_ricochetParticle;
    [SerializeField] private ParticleSystem m_travelParticle;


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

    //Check for when the projectile leaves an entity it has entered to play appropiate effects
    private void GoThroughEntity(float a_distanceToMove) {
        if (m_hasEntered == false) {
            if(m_enterEntityParticle != null)
                m_enterEntityParticle.Play();
            m_hasEntered = true;
        }
        Ray ray = new Ray(transform.position, -(transform.forward));
        if (Physics.Raycast(ray, a_distanceToMove + m_skinWidth, m_entityCollisionMask)) {
            if (m_exitEntityParticle != null)
                m_exitEntityParticle.Play();
            m_lifeTime = m_bulletKillTime;
            m_hasEntered = false;
            m_insideEntity = false;
        }
    }

    //Use ray casts to check for collisions
    private void CheckCollisions(float a_distanceToMove) {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, a_distanceToMove + m_skinWidth, m_entityCollisionMask))
            OnHitObject(hit, true);

        if (Physics.Raycast(ray, out hit, a_distanceToMove + m_skinWidth, m_environmentCollisionMask))
            OnHitObject(hit, false);

        if (Physics.Raycast(ray, out hit, a_distanceToMove + m_skinWidth, m_ricochetCollisionMask))
            OnHitObject(hit, this, false);
    }

    //Events for when the projectile collides with objects in the world with appropriate layers
    private void OnHitObject(RaycastHit a_hit, bool a_hitEntity) {
        IDamagable damagableObject = a_hit.collider.GetComponent<IDamagable>();
        if (damagableObject != null)
            damagableObject.TakeHit(m_damage, a_hit);
        m_insideEntity = a_hitEntity;
        if (m_insideEntity)
            return;
        Destroy(gameObject);
        if (m_travelParticle != null) {
            if (m_travelParticle.isPlaying)
                m_travelParticle.Stop();
        }
    }
    private void OnHitObject(Collider a_c, bool a_hitEntity) {
        IDamagable damagableObject = a_c.GetComponent<IDamagable>();
        if (damagableObject != null)
            damagableObject.TakeDamage(m_damage);
        m_insideEntity = a_hitEntity;
        if (m_insideEntity)
            return;
        Destroy(gameObject);
        if (m_travelParticle != null) {
            if (m_travelParticle.isPlaying)
                m_travelParticle.Stop();
        }
    }
    private void OnHitObject(RaycastHit a_hit, Projectile a_bullet, bool a_hitEntity) {
        IDamagable damagableObject = a_hit.collider.GetComponent<IDamagable>();
        if (damagableObject != null)
            damagableObject.TakeImpact(m_damage, a_hit, a_bullet);
        m_insideEntity = a_hitEntity;
        if (m_insideEntity)
            return;
    }

    private void Start() {
        m_insideEntity = false;
        //If the projectile spawns within a collider, it will activate the appropriate collision response
        Collider[] initialEnemyCollision = Physics.OverlapSphere(transform.position, .1f, m_entityCollisionMask);
        if (initialEnemyCollision.Length > 0) {
            OnHitObject(initialEnemyCollision[0], true);
            return;
        }

        Collider[] initialEnvironmentCollision = Physics.OverlapSphere(transform.position, .1f, m_environmentCollisionMask);
        if (initialEnvironmentCollision.Length > 0) {
            OnHitObject(initialEnvironmentCollision[0], false);
            return;
        }

    Collider[] initialRicochetCollision = Physics.OverlapSphere(transform.position, .1f, m_ricochetCollisionMask);
        if (initialRicochetCollision.Length > 0) {
            OnHitObject(initialRicochetCollision[0], false);
            return;
        }
        if (m_travelParticle != null)
            m_travelParticle.Play();
    }

    //Moves the projectile also counts down until this should be destroyed
    public void Update () {
        float moveDistance = m_speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
        if (m_insideEntity)
            GoThroughEntity(moveDistance);

        if (m_lifeTime <= 0) {
            Destroy(gameObject);
            if (m_travelParticle != null) {
                if (m_travelParticle.isPlaying)
                    m_travelParticle.Stop();
            }
        }
        m_lifeTime -= Time.deltaTime;
	}

}
