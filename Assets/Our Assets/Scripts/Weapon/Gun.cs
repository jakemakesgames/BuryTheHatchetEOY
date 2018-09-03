using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Michael Corben
//Based on Tutorial:https://www.youtube.com/watch?v=rZAnnyensgs&list=PLFt_AvWsXl0ctd4dgE1F8g3uec4zKNRV0&index=3
//Created 24/07/2018
//Last edited 03/09/2018

    [RequireComponent(typeof(AudioSource))]
public class Gun : MonoBehaviour {

    #region Inspector variables
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
    [SerializeField] private Projectile m_projectile;

    [Tooltip("Time between shots in seconds")]
    public float m_secondsBetweenShots = 0.1f;
    public float m_reloadTimeInSeconds = 0.1f;
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
    #endregion

    #region Private variables
    private AudioSource m_audioSource;

    private LayerMask m_entityCollisionMask;
    private LayerMask m_environmentCollisionMask;
    private LayerMask m_ricochetCollisionMask;
    
    private float m_timeUntilNextAction;

    private int m_currentAmmo;
    private int m_currentClip;

    private bool m_infiniteAmmo = false;
    private bool m_isIdle = true;
    private bool m_isFull = true;
    private bool m_setToReloadOne = false;
    #endregion

    #region properties
    public LayerMask EntityCollisionMask {
        get { return m_entityCollisionMask; } 
        set { m_entityCollisionMask = value; }
    }
    public LayerMask EnvironmentCollisionMask {
        get { return m_environmentCollisionMask; }
        set { m_environmentCollisionMask = value; }
    }
    public LayerMask RicochetCollisionMask {
        get { return m_ricochetCollisionMask; } 
        set { m_ricochetCollisionMask = value; }
    }

    public bool IsIdle {
        get { return m_isIdle; }
        set { m_isIdle = value; }
    }
    public bool IsFull {
        get { return m_isFull; }
        set { m_isFull = value; }
    }
    public bool SetToReloadOne {
        get { return m_setToReloadOne; }
        set { m_setToReloadOne = value; }
    }

    public int CurrentClip {
        get { return m_currentClip; } 
        set { m_currentClip = value; }
    }

    public float TimeUntilNextAction {
        get { return m_timeUntilNextAction; } 
        set { m_timeUntilNextAction = value; }
    }
    public int CurrentAmmo { 
        get { return m_currentAmmo; }
        set { m_currentAmmo = value; }
    }
    #endregion

    #region setters, getters and other variable control
    public void AddAmmo(int a_ammoToAdd) { CurrentAmmo += a_ammoToAdd; }
    public void SetEntityCollisionLayer(LayerMask a_collsionMask) {
        EntityCollisionMask = a_collsionMask;
    }
    public void SetEnvironmentCollisionLayer(LayerMask a_collsionMask) {
        EnvironmentCollisionMask = a_collsionMask;
    }
    public void SetRicochetCollisionLayer(LayerMask a_collsionMask) {
        RicochetCollisionMask = a_collsionMask;
    }
    public void SetInfiniteAmmo(bool a_infiniteAmmo) {
        m_infiniteAmmo = a_infiniteAmmo;
    }

    //will return false if the max clip size is smaller than the attemped assignment
    public bool SetCurrentClip(int a_currentClip) {
        if (a_currentClip > m_clipSize)
            return false;

        else
            CurrentClip = a_currentClip;
            return true;
    }

    //will return false if the max ammo pool is smaller than the attemped assignment
    public bool SetCurrentReserveAmmo(int a_reserveAmmo)
    {
        if (a_reserveAmmo > m_maxAmmo)
            return false;
            
        else
            CurrentAmmo = a_reserveAmmo;
            return true;
    }

    public int GetCurrentClip() { return CurrentClip; }
    public int GetCurrentAmmo() { return CurrentAmmo; }
    public int GetTotalAmmo() { return GetCurrentAmmo() + GetCurrentClip(); }
    public int GetMaxAmmo() { return m_maxAmmo + m_clipSize; }
    public Transform GetMuzzle() { return m_muzzle; }
    #endregion

    #region ammo control
    //reloads the gun and also prevents shooting for a time based on the reload time in seconds variable
    //returns true if the gun can reload or false if it cannot
    public bool Reload() {
        if (CurrentClip == m_clipSize)
        {
            return false;
        }
        if (m_isReloading == false) {
            TimeUntilNextAction = Time.time + m_reloadTimeInSeconds;
            m_isReloading = true;
            IsIdle = false;
            if (CurrentAmmo < m_clipSize) {
                CurrentClip = CurrentAmmo;
                if (m_infiniteAmmo == false)
                    CurrentAmmo = 0;
            }
            else {
                if (m_infiniteAmmo == false)
                    CurrentAmmo -= m_clipSize - CurrentClip;
                CurrentClip = m_clipSize;
                IsFull = true;
            }
            if (m_audioSource != null)
                    m_audioSource.PlayOneShot(m_reloadSound, 0.3f);
            return true;
        }
        return false;
    }

    //returns true if the gun can reload or false if it cannot
    public bool ReloadOne() {
        if (CurrentClip <= m_clipSize && CurrentAmmo > 0) {
            if (IsIdle) {
                if (m_isReloading == false)
                    m_isReloading = true;

                TimeUntilNextAction = Time.time + m_reloadTimeInSeconds;
                m_setToReloadOne = true;
                IsIdle = false;
                if (m_audioSource != null)
                    m_audioSource.PlayOneShot(m_reloadSound, 0.3f);
            }
            else if (SetToReloadOne) {
                CurrentClip++;
                CurrentAmmo--;
                if (CurrentClip == m_clipSize)
                    IsFull = true;
            }
            return true;
        }
        return false;
    }

    //tells who asks is if the gun is empty
    public bool GetIsEmpty() {
        if (CurrentClip <= 0)
            return true;
        else
            return false;
        }
    #endregion

    #region shoot
    //Randomises the projectiles angle based on the dispersion angle variable
    private Vector3 RandomAngle() {
        Vector3 direction = m_muzzle.forward + Random.insideUnitSphere * m_dispersionAngle;
        direction.x = 0;
        return direction;
    }

    //Creates projectiles when called if this gun has ammo left in the current
    //clip and if the next shot time has passed
    public bool Shoot() {
        if (Time.time > TimeUntilNextAction) {
            if (CurrentClip > 0) {
                TimeUntilNextAction = Time.time + m_secondsBetweenShots;
                for (int i = 0; i < m_numProjectilesPerShot; i++) {
                    //Projectile setup
                    Projectile newProjectile = Instantiate(m_projectile, m_muzzle.position, m_muzzle.rotation * Quaternion.Euler(RandomAngle())) as Projectile;
                    newProjectile.SetDamage(m_damage); 
                    newProjectile.SetSpeed(m_muzzleVelocity);
                    newProjectile.SetLifeTime(m_projectileLifeTime);
                    newProjectile.SetEntityCollisionLayer(EntityCollisionMask);
                    newProjectile.SetTerrainCollisionLayer(EnvironmentCollisionMask);
                    newProjectile.SetRicochetCollisionLayer(RicochetCollisionMask);
                    IsIdle = false;
                }
                //Play particles and sounds
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
                        m_audioSource.PlayOneShot(m_shootSound, 0.3f);
                }
                CurrentClip--;
                IsFull = false;
                return true;
            }
        }
        return false;
    }
    #endregion

    private void Awake() {
        CurrentAmmo = m_maxAmmo;
        CurrentClip = m_clipSize;
        m_audioSource = GetComponent<AudioSource>();
    }
    //Keeps track of whether this gun is reloading
    //and stops the ammo stores from going higher than their respective maximums
    private void Update() {
        if (Time.time > TimeUntilNextAction) {
            if (m_isReloading) 
                m_isReloading = false;
            if (SetToReloadOne)
                ReloadOne();
            IsIdle = true;
        }
        if (CurrentAmmo > m_maxAmmo)
            CurrentAmmo = m_maxAmmo;
        if (CurrentClip > m_clipSize)
            CurrentClip = m_clipSize;
    }
    
}
