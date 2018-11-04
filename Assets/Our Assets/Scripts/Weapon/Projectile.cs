using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Michael Corben
//Based on Tutorial:https://www.youtube.com/watch?v=rZAnnyensgs&list=PLFt_AvWsXl0ctd4dgE1F8g3uec4zKNRV0&index=3
//Created 24/07/2018
//Last edited 31/10/2018


public class Projectile : MonoBehaviour {

    #region private variables
    private float m_lifeTime = 20 /*seconds*/;
    private float m_skinWidth = 0.1f;
    private float m_speed = 10;
    private float m_knockBack = 5f;
    private int m_damage = 1;
    private bool m_insideEntity;
    private bool m_hasEntered;
    private bool m_targetInvincible;
    private bool m_destroy = true;
    private LayerMask m_ricochetCollisionMask;
    private LayerMask m_entityCollisionMask;
    private LayerMask m_environmentCollisionMask;
    private LayerMask m_hittableCollisionMask;
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
    [Header("Invincible Entity Effects")]
    [Tooltip("The particle that will player when this projectile hits an invincible entity")]
    [SerializeField] private GameObject m_entityInvincibleParticle;
    [Tooltip("Life time of the particle system")]
    [SerializeField] private float m_entityInvincibleParticleTimer = 1f;
    [Tooltip("The sound that will play when this projectile hits an invincible entity")]
    [SerializeField] private AudioClip m_entityInvincibleAudioClip;

    [Header("Enter Entity Effects")]
    [Tooltip("The particle that will play when the projectile first hits an entity ie; enemy or player")]
    [SerializeField] private GameObject m_enterEntityParticle;
    [Tooltip("Life time of the particle system")]
    [SerializeField] private float m_enterParticleTimer = 1f;
    [Tooltip("Distance along the objects 'z' away from impact to play the particles")]
    [SerializeField] private float m_enterParticleDist = 0.1f;
    [Tooltip("The sound that will play when the projectile first hits an entity")]
    [SerializeField] private AudioClip m_enterEntityAudioClip;

    [Header("Exit Entity Effects")]
    [Tooltip("The particle that will play when the projectile exits an entities wound")]
    [SerializeField] private GameObject m_exitEntityParticle;
    [Tooltip("Life time of the particle system")]
    [SerializeField] private float m_exitParticleTimer = 1f;
    [Tooltip("Distance along the objects 'z' away from exit wound to play the particles")]
    [SerializeField] private float m_exitParticleDist = 0.1f;
    [Tooltip("The sound that will play when the projectile exits the entities wound")]
    [SerializeField] private AudioClip m_exitEntityAudioClip;

    [Header("Ricochet Effects")]
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
    [SerializeField] private float m_trailRendererLifeTime = 2f;

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

    private GameObject m_instancedTrailRenderer;
    #endregion


    //----------------------------
    #region Properties
    public float KnockBack { get { return m_knockBack; } set { m_knockBack = value; } }

    public float Speed { get { return m_speed; } set { m_speed = value; } }

    public bool TargetInvincible { set { m_targetInvincible = value; } }

    public bool InsideEntity { set { m_insideEntity = value; } }
    #endregion

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
    #region Collision Detection and First Response Methods
    private void CheckCollisions(float a_distanceToMove) {

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        #region Multilayered layer detection
        
        //Casts a ray checking all they layers it could potentially hit and then reacting accoridingly
        if (Physics.Raycast(ray, out hit, a_distanceToMove + m_skinWidth, m_hittableCollisionMask)) {

            //Stores the raycasthit's layer for checking what what hit
            LayerMask hitLayer = (1 << hit.transform.gameObject.layer);

            if (hitLayer == m_ricochetCollisionMask)
            {
                if (m_ricochetParticle != null)
                {
                    GameObject GO = Instantiate(m_ricochetParticle, hit.point, transform.rotation);
                    Destroy(GO, m_ricochetParticleTimer);
                }

                if (m_ricochetAudioClip != null && m_spawnedSpeaker != null)
                {
                    SpawnedSpeaker SS = Instantiate(m_spawnedSpeaker, hit.point, hit.transform.rotation) as SpawnedSpeaker;
                    SS.AudioSource.clip = m_ricochetAudioClip;
                    SS.AudioSource.Play();
                }
                OnHitObject(hit, this, false);
            }

            else if (hitLayer == m_entityCollisionMask)
            {
                if (m_insideEntity == false)
                {
                    OnHitObject(hit, true);
                    m_currentlyInside = hit.transform.gameObject;
                }
            }
            else if (hitLayer == m_environmentCollisionMask)
                OnHitObject(hit, false);

            //Checking the list of extra environmental collisions which will have their own effect
            else {
                for (int i = 0; i < m_environmentCollisionMasks.Count; i++) {
                    if (hitLayer == m_environmentCollisionMasks[i]) {
                        if (m_environmentParticles[i] != null) {
                            //Gross horrible way of seperating a specific layer change
                            Quaternion goRot = transform.rotation;
                            if (i != 1) {
                                GameObject GO = Instantiate(m_environmentParticles[i], hit.point, Quaternion.LookRotation(-transform.forward));
                                Destroy(GO, m_environmentParticleTimers[i]);
                            }
                            //Cactus grossness
                            else {
                                GameObject GO = Instantiate(m_environmentParticles[i], hit.point, goRot);
                                Destroy(GO, m_environmentParticleTimers[i]);
                                m_destroy = false;

                            }
                        }
                        if (m_environmentAudioClips[i] != null) {
                            SpawnedSpeaker audio = Instantiate(m_spawnedSpeaker, hit.point, transform.rotation) as SpawnedSpeaker;
                            audio.AudioSource.clip = m_environmentAudioClips[i];
                            audio.AudioSource.Play();
                        }

                        OnHitObject(hit, false);
                    }
                }
            }
        }
        /*
         */
        #endregion

        #region Original collision detection
        /*
        //The case where the projectile hits a ricochet object
        if (Physics.Raycast(ray, out hit, a_distanceToMove + m_skinWidth, m_ricochetCollisionMask)) {
            //if (hit.transform.gameObject.layer == m_ricochetCollisionMask) {
                if (m_ricochetParticle != null) {
                    GameObject GO = Instantiate(m_ricochetParticle, hit.point, transform.rotation);
                    Destroy(GO, m_ricochetParticleTimer);
                }

                if (m_ricochetAudioClip != null && m_spawnedSpeaker != null) {
                    SpawnedSpeaker SS = Instantiate(m_spawnedSpeaker, transform) as SpawnedSpeaker;
                    SS.AudioSource.clip = m_ricochetAudioClip;
                    SS.AudioSource.Play();
                }
                OnHitObject(hit, this, false);
            //}
        }

        //The case where the projectile hits an entity
        if (m_insideEntity == false) {
            if (Physics.Raycast(ray, out hit, a_distanceToMove + m_skinWidth, m_entityCollisionMask)) {
                //if (hit.transform.gameObject.layer == m_entityCollisionMask) {
                    OnHitObject(hit, true);
                    m_currentlyInside = hit.transform.gameObject;
                //}
            }
        }


        //Checking for the case where the projectile hits an environment object with custom effects
        if (Physics.Raycast(ray, out hit, a_distanceToMove + m_skinWidth, m_environmentCollisionMask))
            OnHitObject(hit, false);

        for (int i = 0; i < m_environmentCollisionMasks.Count; i++) {
            if (Physics.Raycast(ray, out hit, a_distanceToMove + m_skinWidth, m_environmentCollisionMasks[i])) {

                //if (hit.transform.gameObject.layer == m_environmentCollisionMasks[i]) {
                //Gross horrible way of seperating a specific layer change
                if (m_environmentParticles[i] != null) {
                    Quaternion goRot = transform.rotation;
                    if (i != 1) { 
                        GameObject GO = Instantiate(m_environmentParticles[i], hit.point, Quaternion.LookRotation(-transform.forward));
                        Destroy(GO, m_environmentParticleTimers[i]);
                    }
                    //Cactus grossness
                    else {
                        GameObject GO = Instantiate(m_environmentParticles[i], hit.point, goRot);
                        Destroy(GO, m_environmentParticleTimers[i]);
                        m_destroy = false;
                    }
                }

                if (m_environmentAudioClips[i] != null) {
                    SpawnedSpeaker audio = Instantiate(m_spawnedSpeaker, hit.point, transform.rotation) as SpawnedSpeaker;
                    audio.AudioSource.clip = m_environmentAudioClips[i];
                    audio.AudioSource.Play();
                }
                OnHitObject(hit, false);
                //}
            }
        }
        /*
        */
        #endregion

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
                if (m_exitEntityParticle != null) {
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
        if (m_targetInvincible) {
            if (m_entityInvincibleParticle != null) {
                GameObject GO = Instantiate(m_entityInvincibleParticle, a_hit.point, transform.rotation);
                Destroy(GO, m_entityInvincibleParticleTimer);
            }

            if (m_entityInvincibleAudioClip != null && m_spawnedSpeaker != null) {
                SpawnedSpeaker SS = Instantiate(m_spawnedSpeaker, a_hit.point, transform.rotation) as SpawnedSpeaker;
                SS.AudioSource.clip = m_entityInvincibleAudioClip;
                SS.AudioSource.Play();
            }
            if (m_destroy == false) {
                m_destroy = true;
                return;
            }
            Destroy(gameObject);
            if (m_trailRenderer != null)
                Destroy(m_instancedTrailRenderer, m_trailRendererLifeTime);

            return;
        }
        m_insideEntity = a_hitEntity;
        if (m_insideEntity)
            return;
        if (m_destroy == false) {
            m_destroy = true;
            return;
        }
        Destroy(gameObject);
        if (m_trailRenderer != null)
            Destroy(m_instancedTrailRenderer, m_trailRendererLifeTime);
    }

    private void OnHitObject(Collider a_c, bool a_hitEntity) {
        IDamagable damagableObject = a_c.GetComponent<IDamagable>();
        if (damagableObject != null)
            damagableObject.TakeDamage(m_damage);
        m_insideEntity = a_hitEntity;
        if (m_insideEntity)
            return;
        Destroy(gameObject);
        Destroy(m_instancedTrailRenderer, m_trailRendererLifeTime);
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

        //Merge all layers into a new layer set for checking with a ray
        m_hittableCollisionMask = m_ricochetCollisionMask | m_entityCollisionMask | m_environmentCollisionMask;
        for (int i = 0; i < m_environmentCollisionMasks.Count; i++) {
            m_hittableCollisionMask = m_hittableCollisionMask | m_environmentCollisionMasks[i];
        }
        #region one layer at a time
        /*
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
        /*
         */
        #endregion
        #region multilayer
        //If the projectile spawns within a collider, it will activate the appropriate collision response
        Collider[] initialCollision = Physics.OverlapSphere(transform.position, .1f, m_hittableCollisionMask);
        
        if (initialCollision.Length > 0) {
            LayerMask hitLayer = (1 << initialCollision[0].transform.gameObject.layer);
            if (hitLayer == m_ricochetCollisionMask) {
                if (m_ricochetParticle != null) {
                    GameObject GO = Instantiate(m_ricochetParticle, transform.position, transform.rotation);
                    Destroy(GO, m_ricochetParticleTimer);
                }

                if (m_ricochetAudioClip != null && m_spawnedSpeaker != null) {
                    SpawnedSpeaker SS = Instantiate(m_spawnedSpeaker, transform.position, transform.rotation) as SpawnedSpeaker;
                    SS.AudioSource.clip = m_ricochetAudioClip;
                    SS.AudioSource.Play();
                }
                OnHitObject(initialCollision[0], false); 
            }

            else if (hitLayer == m_entityCollisionMask) {
                if (m_insideEntity == false) {
                    OnHitObject(initialCollision[0], true);
                    m_currentlyInside = initialCollision[0].transform.gameObject;
                }
            }
            else if (hitLayer == m_environmentCollisionMask) 
                OnHitObject(initialCollision[0], false);

            else {
                for (int i = 0; i < m_environmentCollisionMasks.Count; i++) {
                    if (hitLayer == m_environmentCollisionMasks[i]) {

                        if (m_environmentParticles[i] != null) {
                            //Gross horrible way of seperating a specific layer change
                            Quaternion goRot = transform.rotation;
                            if (i != 1) {
                                GameObject GO = Instantiate(m_environmentParticles[i], transform.position, Quaternion.LookRotation(-transform.forward));
                                Destroy(GO, m_environmentParticleTimers[i]);
                            }
                            //Cactus grossness
                            else {
                                GameObject GO = Instantiate(m_environmentParticles[i], transform.position, goRot);
                                Destroy(GO, m_environmentParticleTimers[i]);
                                m_destroy = false;

                            }
                        }
                        if (m_environmentAudioClips[i] != null) {
                            SpawnedSpeaker audio = Instantiate(m_spawnedSpeaker, transform.position, transform.rotation) as SpawnedSpeaker;
                            audio.AudioSource.clip = m_environmentAudioClips[i];
                            audio.AudioSource.Play();
                        }
                        OnHitObject(initialCollision[0], false);
                    }
                }
            }
        }
        /*
         */
        #endregion

        if (m_trailRenderer != null)
            m_instancedTrailRenderer = Instantiate(m_trailRenderer, transform.position, transform.rotation) as GameObject;

        Quaternion rot = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        transform.rotation = rot;
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
        if (m_instancedTrailRenderer != null) {
            m_instancedTrailRenderer.transform.position = transform.position;
            m_instancedTrailRenderer.transform.rotation = transform.rotation;
        }

    }

    //Displays a line out the projectile showing what it'll hit next calculation
    private void OnDrawGizmosSelected() {
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * m_speed * Time.deltaTime);
    }
}
