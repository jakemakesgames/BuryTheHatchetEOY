using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Michael Corben
//Based on Tutorial:https://www.youtube.com/watch?v=rZAnnyensgs&list=PLFt_AvWsXl0ctd4dgE1F8g3uec4zKNRV0&index=3
//Created 24/07/2018
//Last edited 22/08/2018


public class Projectile : MonoBehaviour {

    private float m_lifeTime = 20 /*seconds*/;
    private float m_skinWidth = 0.1f;
    private float m_speed = 10;
    private int m_damage = 1;
    private bool m_insideEntity;
    private bool m_hasEntered;
    private LayerMask m_ricochetCollisionMask;
    private LayerMask m_entityCollisionMask;
    private LayerMask m_environmentCollisionMask;


    [Tooltip("Life time of the bullet after it hits an entity")]
    [SerializeField] private float m_bulletKillTime = 1f;

    [Header("Preset collision effects")]
    [Tooltip("The particle that will play when the projectile first hits an entity ie; enemy or player")]
    [SerializeField] private GameObject m_enterEntityParticle;
    [Tooltip("Life time of the particle system")]
    [SerializeField] private float m_enterParticleTimer = 1f;
    [Tooltip("distance along the objects 'z' away from impact to play the particles")]
    [SerializeField] private float m_enterParticleDist = 0.1f;
    [Tooltip("The sound that will play when the projectile first hits an entity")]
    [SerializeField] private AudioClip m_enterEntityAudioClip;

    [Tooltip("The particle that will play when the projectile exits an entities wound")]
    [SerializeField] private GameObject m_exitEntityParticle;
    [Tooltip("Life time of the particle system")]
    [SerializeField] private float m_exitParticleTimer = 1f;
    [Tooltip("distance along the objects 'z' away from exit wound to play the particles")]
    [SerializeField] private float m_exitParticleDist = 0.1f;
    [Tooltip("The sound that will play when the projectile exits the entities wound")]
    [SerializeField] private AudioClip m_exitEntityAudioClip;

    [Tooltip("The particle that will play when the projectile ricochets off the environment")]
    [SerializeField] private GameObject m_ricochetParticle;
    [Tooltip("Life time of the particle system")]
    [SerializeField] private float m_ricochetParticleTimer = 1f;
    [Tooltip("distance along the objects 'z' away from impact to play the particles")]
    [SerializeField] private float m_ricochetParticleDist = 0.1f;
    [Tooltip("The sound that will play when the projectile ricochets off the environment")]
    [SerializeField] private AudioClip m_ricochetAudioClip;

    [Tooltip("Life time of the bullet after it hits an entity")]
    [SerializeField] private GameObject m_travelParticle;

    [Header("Custom Environment effects")]
    [Tooltip("For each of these lists, the information will control what happens when the projectil hits an object on the specified layer")]
    [SerializeField] private List<LayerMask> m_environmentCollisionMasks;
    [Tooltip("Particle to play on impact")]
    [SerializeField] private List<GameObject> m_environmentParticles;
    [Tooltip("Life time of particle system")]
    [SerializeField] private List<float> m_environmentParticleTimers;
    [Tooltip("distance along the objects 'z' away from impact to play the particles")]
    [SerializeField] private List<float> m_environmentParticleDists;
    [Tooltip("sound to play on impact")]
    [SerializeField] private List<AudioClip> m_environmentAudioClips;


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
            if (m_enterEntityParticle != null) {
                GameObject GO = Instantiate(m_enterEntityParticle, transform.position + transform.forward * m_enterParticleDist, transform.rotation);
                Destroy(GO, m_enterParticleTimer);
            }
            m_hasEntered = true;
        }
        Ray ray = new Ray(transform.position, -(transform.forward));
        if (Physics.Raycast(ray, a_distanceToMove + m_skinWidth, m_entityCollisionMask)) {
            if (m_exitEntityParticle != null) {
                GameObject GO = Instantiate(m_exitEntityParticle, transform.position - transform.forward * m_exitParticleDist, transform.rotation);
                Destroy(GO, m_exitParticleTimer);
            }
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

        for (int i = 0; i < m_environmentParticles.Count; i++) {
            if (Physics.Raycast(ray, out hit, a_distanceToMove + m_skinWidth, m_environmentCollisionMasks[i])) {
                OnHitObject(hit, false);
                if (m_environmentParticles[i] != null) {
                    GameObject GO = Instantiate(m_environmentParticles[i], transform.position - transform.forward * m_environmentParticleDists[i], transform.rotation);
                    Destroy(GO, m_environmentParticleTimers[i]);
                }
            }
        }
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
    }
    private void OnHitObject(Collider a_c, bool a_hitEntity) {
        IDamagable damagableObject = a_c.GetComponent<IDamagable>();
        if (damagableObject != null)
            damagableObject.TakeDamage(m_damage);
        m_insideEntity = a_hitEntity;
        if (m_insideEntity)
            return;
        Destroy(gameObject);
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
            m_travelParticle = Instantiate(m_travelParticle, transform);
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
        }
        m_lifeTime -= Time.deltaTime;
	}

}
