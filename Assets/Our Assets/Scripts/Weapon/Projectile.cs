﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Michael Corben
//Based on Tutorial:https://www.youtube.com/watch?v=rZAnnyensgs&list=PLFt_AvWsXl0ctd4dgE1F8g3uec4zKNRV0&index=3
//Created 24/07/2018
//Last edited 08/10/2018


public class Projectile : MonoBehaviour {
    
    #region private variables
    private float m_lifeTime = 20 /*seconds*/;
    private float m_skinWidth = 0.1f;
    private float m_speed = 10;
    private float m_knockBack = 5f;
    private int m_damage = 1;
    private bool m_insideEntity;
    private bool m_hasEntered;
    private LayerMask m_ricochetCollisionMask;
    private LayerMask m_entityCollisionMask;
    private LayerMask m_environmentCollisionMask;
    private GameObject m_currentlyInside;
    #endregion

    //----------------------------
    #region inspector variables
    [Header("Required for sound")]
    [Tooltip("The prefab for the speakers that'll spawn on events")]
    [SerializeField] private SpawnedSpeaker m_spawnedSpeaker;

    [Tooltip("Life time of the bullet after it hits an entity")]
    [SerializeField] private float m_bulletKillTime = 1f;

    [Header("Preset collision effects")]
    [Tooltip("The particle that will play when the projectile first hits an entity ie; enemy or player")]
    [SerializeField] private GameObject m_enterEntityParticle;
    [Tooltip("Life time of the particle system")]
    [SerializeField] private float m_enterParticleTimer = 1f;
    [Tooltip("Distance along the objects 'z' away from impact to play the particles")]
    [SerializeField] private float m_enterParticleDist = 0.1f;
    [Tooltip("The sound that will play when the projectile first hits an entity")]
    [SerializeField] private AudioClip m_enterEntityAudioClip;

    [Tooltip("The particle that will play when the projectile exits an entities wound")]
    [SerializeField] private GameObject m_exitEntityParticle;
    [Tooltip("Life time of the particle system")]
    [SerializeField] private float m_exitParticleTimer = 1f;
    [Tooltip("Distance along the objects 'z' away from exit wound to play the particles")]
    [SerializeField] private float m_exitParticleDist = 0.1f;
    [Tooltip("The sound that will play when the projectile exits the entities wound")]
    [SerializeField] private AudioClip m_exitEntityAudioClip;

    [Tooltip("The particle that will play when the projectile ricochets off the environment")]
    [SerializeField] private GameObject m_ricochetParticle;
    [Tooltip("Life time of the particle system")]
    [SerializeField] private float m_ricochetParticleTimer = 1f;
    [Tooltip("Distance along the objects 'z' away from impact to play the particles")]
    [SerializeField] private float m_ricochetParticleDist = 0.1f;
    [Tooltip("The sound that will play when the projectile ricochets off the environment")]
    [SerializeField] private AudioClip m_ricochetAudioClip;

    [Header("Trail renderer")]
    [Tooltip("The trail renderer prefab which will follow along the projectile as it travels")]
    [SerializeField] private GameObject m_trailRenderer;
    [Tooltip("Life time of the trail renderer after the bullet is destroyed")]
    [SerializeField] private float m_trailRendererLifeTime = 5f;

    [Header("Custom Environment effects")]
    [Tooltip("For each of these lists, the information will control what happens when the projectil hits an object on the specified layer")]
    [SerializeField] private List<LayerMask> m_environmentCollisionMasks;
    [Tooltip("Particle to play on impact")]
    [SerializeField] private List<GameObject> m_environmentParticles;
    [Tooltip("Life time of particle system")]
    [SerializeField] private List<float> m_environmentParticleTimers;
    [Tooltip("Distance along the objects 'z' away from impact to play the particles")]
    [SerializeField] private List<float> m_environmentParticleDists;
    [Tooltip("Dound to play on impact")]
    [SerializeField] private List<AudioClip> m_environmentAudioClips;
    #endregion

    public float KnockBack {
        get { return m_knockBack; }
        set { m_knockBack = value; }
    }

    //----------------------------
    #region setters
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
    #endregion

    //Use ray casts to check for collisions
    #region Collision Detection Methods
    private void CheckCollisions(float a_distanceToMove) {

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        //The case where the projectile hits a ricochet object
        if (Physics.Raycast(ray, out hit, a_distanceToMove + m_skinWidth, m_ricochetCollisionMask)) {
            if (m_ricochetParticle != null) {
                GameObject GO = Instantiate(m_ricochetParticle, transform.position - transform.forward * m_ricochetParticleDist, transform.rotation);
                Destroy(GO, m_ricochetParticleTimer);
            }

            if (m_ricochetAudioClip != null && m_spawnedSpeaker != null) {
                SpawnedSpeaker SS = Instantiate(m_spawnedSpeaker, transform) as SpawnedSpeaker;
                SS.AudioSource.clip = m_ricochetAudioClip;
                SS.AudioSource.Play();
            }
            OnHitObject(hit, this, false);
        }

        //The case where the projectile hits an entity
        if (m_insideEntity == false) {
            if (Physics.Raycast(ray, out hit, a_distanceToMove + m_skinWidth, m_entityCollisionMask)) {
                OnHitObject(hit, true);
                m_currentlyInside = hit.transform.gameObject;
            }
        }


        //Checking for the case where the projectile hits an environment object with custom effects
        if (Physics.Raycast(ray, out hit, a_distanceToMove + m_skinWidth, m_environmentCollisionMask))
            OnHitObject(hit, false);

        for (int i = 0; i < m_environmentCollisionMasks.Count; i++) {
            if (Physics.Raycast(ray, out hit, a_distanceToMove + m_skinWidth, m_environmentCollisionMasks[i])) {
                OnHitObject(hit, false);
                if (m_environmentParticles[i] != null) {
                    GameObject GO = Instantiate(m_environmentParticles[i], transform.position - transform.forward * m_environmentParticleDists[i], transform.rotation);
                    Destroy(GO, m_environmentParticleTimers[i]);
                }
                if (m_environmentAudioClips[i] != null) {
                    SpawnedSpeaker audio = Instantiate(m_spawnedSpeaker, transform.position, transform.rotation) as SpawnedSpeaker;
                    audio.AudioSource.clip = m_environmentAudioClips[i];
                    audio.AudioSource.Play();
                }
            }
        }
    }

    //Check for when the projectile leaves an entity it has entered to play appropiate effects
    private void GoThroughEntity(float a_distanceToMove)
    {
        if (m_hasEntered == false)
        {
            if (m_enterEntityParticle != null)
            {
                Ray enterRay = new Ray(transform.position, transform.forward);
                RaycastHit enterRayHit;
                Physics.Raycast(enterRay, out enterRayHit, a_distanceToMove + m_skinWidth, m_entityCollisionMask);
                if (m_exitEntityParticle != null)
                {
                    GameObject GO = Instantiate(m_enterEntityParticle, enterRayHit.point + transform.forward * m_enterParticleDist, transform.rotation);
                    Destroy(GO, m_enterParticleTimer);
                }
            }
            m_hasEntered = true;
        }
        Ray ray = new Ray(transform.position, -(transform.forward));
        RaycastHit exitRayHit;

        //checking when the projectile, which has entered an entity, has left that entity
        if (Physics.Raycast(ray, out exitRayHit, a_distanceToMove + m_skinWidth, m_entityCollisionMask))
        {
            if (exitRayHit.transform.gameObject == m_currentlyInside)
            {
                if (m_exitEntityParticle != null)
                {
                    GameObject GO = Instantiate(m_exitEntityParticle, exitRayHit.point - transform.forward * m_exitParticleDist, transform.rotation);
                    Destroy(GO, m_exitParticleTimer);
                }

                m_lifeTime = m_bulletKillTime;
                m_hasEntered = false;
                m_insideEntity = false;
                m_currentlyInside = null;
            }
        }
    }

    #endregion

    //Events for when the projectile collides with objects in the world with appropriate layers
    #region Collision Response Methods
    private void OnHitObject(RaycastHit a_hit, bool a_hitEntity) {
        IDamagable damagableObject = a_hit.collider.GetComponent<IDamagable>();
        if (damagableObject != null)
            damagableObject.TakeImpact(m_damage, a_hit, this);
        m_insideEntity = a_hitEntity;
        if (m_insideEntity)
            return;
        Destroy(gameObject);
        if (m_trailRenderer != null)
            Destroy(m_trailRenderer, m_trailRendererLifeTime);
    }

    private void OnHitObject(Collider a_c, bool a_hitEntity) {
        IDamagable damagableObject = a_c.GetComponent<IDamagable>();
        if (damagableObject != null)
            damagableObject.TakeDamage(m_damage);
        m_insideEntity = a_hitEntity;
        if (m_insideEntity)
            return;
        Destroy(gameObject);
        Destroy(m_trailRenderer, m_trailRendererLifeTime);
    }
    
    private void OnHitObject(RaycastHit a_hit, Projectile a_bullet, bool a_hitEntity) {
        IDamagable damagableObject = a_hit.collider.GetComponent<IDamagable>();
        if (damagableObject != null)
            damagableObject.TakeImpact(m_damage, a_hit, a_bullet);
        m_insideEntity = a_hitEntity;
        if (m_insideEntity)
            return;
    }
    #endregion

    //Create the trail renderer and check for initial collisions
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

        if (m_trailRenderer != null)
            m_trailRenderer = Instantiate(m_trailRenderer, transform.position, transform.rotation);

    }

    //Moves the projectile along its forward
    //also counts down until this should be destroyed
    public void Update () {
        float moveDistance = m_speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);

        if (m_insideEntity)
            GoThroughEntity(moveDistance);

        if (m_lifeTime <= 0)
            Destroy(gameObject);

        m_lifeTime -= Time.deltaTime;
        if (m_trailRenderer != null) {
            m_trailRenderer.transform.position = transform.position;
            m_trailRenderer.transform.rotation = transform.rotation;
        }

    }

    //Displays a line out the projectile showing what it'll hit next calculation
    private void OnDrawGizmosSelected() {
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * m_speed * Time.deltaTime);
    }
}
