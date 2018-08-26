using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Michael Corben
//Based on Tutorial:https://www.youtube.com/watch?v=rZAnnyensgs&list=PLFt_AvWsXl0ctd4dgE1F8g3uec4zKNRV0&index=3
//Created 24/07/2018
//Last edited 25/08/2018

    [RequireComponent(typeof(AudioSource))]
public class Gun : MonoBehaviour {

    [Tooltip("The sound that will play when the gun reloads")]
    [SerializeField] private AudioClip m_reloadSound;
    [Tooltip("The sound that will play when the gun shoots")]
    [SerializeField] private AudioClip m_shootSound;
    [Tooltip("The particle that will play when the gun shoots")]
    [SerializeField] private GameObject m_shootParticleSystem;
    [SerializeField] private float m_shootParticleLifeTime = 1f;
    [Tooltip("The particle that will play after the gun shoots")]
    [SerializeField] private GameObject m_smokeParticleSystem;
    [SerializeField] private float m_smokeParticleLifeTime = 1f;

    [Tooltip("The position projectiles spawn when the gun shoots")]
    [SerializeField] private Transform m_muzzle;
    [Tooltip("The projectile to be shot from this gun")]
    public Projectile m_projectile;

    [Tooltip("Time between shots in millie seconds")]
    public float m_msBetweenShots = 100;
    public float m_reloadTimeInMilliseconds = 100;
    [Tooltip("Speed of the projectile when the gun shoots it")]
    public float m_muzzleVelocity = 15;
    [Tooltip("How long in seconds the projectile will exist in the world")]
    public float m_projectileLifeTime = 15;
    [Tooltip("The angle from a line straight out the gun either side within which the projectile may be spawned")]
    public float m_dispersionAngle = 0;

    [Tooltip("The number of projectiles that will be spawned when the gun shoots")]
    public int m_numProjectilesPerShot = 1;
    [Tooltip("Currently does nothing")]
    public int m_burstProjectiles = 1;
    [Tooltip("The ammount of ammo that the entity wielding the gun can hold outside of the clip")]
    public int m_maxAmmo = 100;
    [Tooltip("The number of times the gun can be shot before needing to reload from full")]
    public int m_clipSize = 6;
    [Tooltip("The damage each projectile does on impact")]
    public int m_damage = 1;

    [Tooltip("shows when this gun is being reloaded")]
    public bool m_isReloading = false;
    [Tooltip("Determines if mouse press or hold mouse down to shoot")]
    public bool m_isAutomatic = false;

    private LayerMask m_entityCollisionMask;
    private LayerMask m_environmentCollisionMask;
    private LayerMask m_ricochetCollisionMask;
    private AudioSource m_audioSource;
    private float m_nextShotTime;

    [SerializeField] private int m_currentAmmo;
    [SerializeField] private int m_currentClip;

    private bool m_infiniteAmmo = false;
    private bool m_isIdle = true;
    private bool m_isFull = true;

    public bool IsIdle {
        get { return m_isIdle; }
        set { m_isIdle = value; }
    }
    public bool IsFull {
        get { return m_isFull; }
        set { m_isFull = value; }
    }

    public void AddAmmo(int a_ammoToAdd) { m_currentAmmo += a_ammoToAdd; }
    public void SetEntityCollisionLayer(LayerMask a_collsionMask) {
        m_entityCollisionMask = a_collsionMask;
    }
    public void SetEnvironmentCollisionLayer(LayerMask a_collsionMask) {
        m_environmentCollisionMask = a_collsionMask;
    }
    public void SetRicochetCollisionLayer(LayerMask a_collsionMask) {
        m_ricochetCollisionMask = a_collsionMask;
    }
    public void SetInfiniteAmmo(bool a_infiniteAmmo) {
        m_infiniteAmmo = a_infiniteAmmo;
    }

    //will return false if the max clip size is smaller than the attemped assignment
    public bool SetCurrentClip(int a_currentClip) {
        if (a_currentClip > m_clipSize)
            return false;

        else
            m_currentClip = a_currentClip;
            return true;
    }

    //will return false if the max ammo pool is smaller than the attemped assignment
    public bool SetCurrentReserveAmmo(int a_reserveAmmo)
    {
        if (a_reserveAmmo > m_maxAmmo)
            return false;
            
        else
            m_currentAmmo = a_reserveAmmo;
            return true;
    }

    public int GetCurrentClip() { return m_currentClip; }
    public int GetCurrentAmmo() { return m_currentAmmo; }
    public int GetTotalAmmo() { return GetCurrentAmmo() + GetCurrentClip(); }
    public int GetMaxAmmo() { return m_maxAmmo + m_clipSize; }
    public Transform GetMuzzle() { return m_muzzle; }

    //reloads the gun and also prevents shooting for a time based on the reload time in milliseconds variable
    public void Reload() {
        if (m_isReloading == false) {
            m_nextShotTime = Time.time + m_reloadTimeInMilliseconds / 1000;
            m_isReloading = true;
            IsIdle = false;
            if (m_currentAmmo < m_clipSize) {
                m_currentClip = m_currentAmmo;
                if (m_infiniteAmmo == false)
                    m_currentAmmo = 0;
            }
            else {
                if (m_infiniteAmmo == false)
                    m_currentAmmo -= m_clipSize - m_currentClip;
                m_currentClip = m_clipSize;
                IsFull = true;
            }
            if (m_audioSource != null)
                    m_audioSource.PlayOneShot(m_reloadSound);
        }
    }

    //tells who asks is if the gun is empty
    public bool GetIsEmpty() {
        if (m_currentClip <= 0)
            return true;
        else
            return false;
        }

    //Randomises the projectiles angle based on the dispersion angle variable
    private Vector3 RandomAngle() {
        Vector3 direction = m_muzzle.forward + Random.insideUnitSphere * m_dispersionAngle;
        direction.x = 0;
        return direction;
    }

    //Creates projectiles when called if this gun has ammo left in the current
    //clip and if the next shot time has passed
    public bool Shoot() {
        if (Time.time > m_nextShotTime) {
            if (m_currentClip > 0) {
                m_nextShotTime = Time.time + m_msBetweenShots / 1000;
                for (int i = 0; i < m_numProjectilesPerShot; i++) {
                    Projectile newProjectile = Instantiate(m_projectile, m_muzzle.position, m_muzzle.rotation * Quaternion.Euler(RandomAngle())) as Projectile;
                    newProjectile.SetDamage(m_damage); 
                    newProjectile.SetSpeed(m_muzzleVelocity);
                    newProjectile.SetLifeTime(m_projectileLifeTime);
                    newProjectile.SetEntityCollisionLayer(m_entityCollisionMask);
                    newProjectile.SetTerrainCollisionLayer(m_environmentCollisionMask);
                    newProjectile.SetRicochetCollisionLayer(m_ricochetCollisionMask);
                    IsIdle = false;
                }
                if (m_shootParticleSystem != null) {
                    GameObject GO = Instantiate(m_shootParticleSystem, m_muzzle.position, transform.rotation);
                    Destroy(GO, m_shootParticleLifeTime);
                }
                if (m_smokeParticleSystem != null) {
                    GameObject GO = Instantiate(m_smokeParticleSystem, m_muzzle.position, transform.rotation);
                    Destroy(GO, m_smokeParticleLifeTime);
                }
                if (m_audioSource != null) {
                    if (m_audioSource.isPlaying == false)
                        m_audioSource.PlayOneShot(m_shootSound);
                }
                m_currentClip--;
                IsFull = false;
                return true;
            }
        }
        return false;
    }

    private void Awake() {
        m_currentAmmo = m_maxAmmo;
        m_currentClip = m_clipSize;
        m_audioSource = GetComponent<AudioSource>();
    }
    //Keeps track of whether this gun is reloading
    //and stops the ammo stores from going higher than their respective maximums
    private void Update() {
        if (Time.time > m_nextShotTime) {
            if (m_isReloading) 
                m_isReloading = false;
            IsIdle = true;
        }
        if (m_currentAmmo > m_maxAmmo)
            m_currentAmmo = m_maxAmmo;
        if (m_currentClip > m_clipSize)
            m_currentClip = m_clipSize;
    }
    
}
