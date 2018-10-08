using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Michael Corben
//Based on Tutorial:https://www.youtube.com/watch?v=rZAnnyensgs&list=PLFt_AvWsXl0ctd4dgE1F8g3uec4zKNRV0&index=3
//Created 24/07/2018
//Last edited 08/10/2018

    [RequireComponent(typeof(AudioSource))]
public class Gun : MonoBehaviour {

    //----------------------------
    //Variables and access
    //----------------------------
    #region Inspector variables
    [Header("projectiles parent object, MUST BE AT 0,0,0")]
    [Tooltip("Parent object for scene clarity")]
    [SerializeField] private GameObject m_parent;

    [Header("Audio")]
    [Tooltip("The sound that will play when the gun reloads")]
    [SerializeField] private AudioClip m_reloadSound;

    [Tooltip("The sound that will play when the gun shoots")]
    [SerializeField] private AudioClip m_shootSound;

    [Header("VFX")]
    [Tooltip("The particle that will play when the gun shoots")]
    [SerializeField] private GameObject m_shootParticleSystem;
    [SerializeField] private float m_shootParticleLifeTime = 1f;

    [Tooltip("The particle that will play after the gun shoots")]
    [SerializeField] private GameObject m_smokeParticleSystem;
    [SerializeField] private float m_smokeParticleLifeTime = 1f;

    [Header("Other requirements")]
    [Tooltip("The position projectiles spawn when the gun shoots")]
    [SerializeField] private Transform m_muzzle;
    [Tooltip("The projectile to be shot from this gun")]
    [SerializeField] private Projectile m_projectile;

    [Tooltip("Time between shots in seconds")]
    [SerializeField] private float m_secondsBetweenShots = 0.1f;
    [Tooltip("Time between it takes for an individual bullet to be reloaded into this weapon")]
    [SerializeField] private float m_reloadTimeInSeconds = 0.1f;

    [Tooltip("Speed of the projectile when the gun shoots it")]
    [SerializeField] private float m_muzzleVelocity = 15;

    [Tooltip("How long in seconds the projectile will exist in the world")]
    [SerializeField] private float m_projectileLifeTime = 15;

    [Tooltip("The angle from a line straight out the gun either side within which the projectile may be spawned")]
    [SerializeField] private float m_dispersionAngle = 0;

    [Tooltip("The amount of knockback a bullet fired from this weapon will apply to an entity")]
    [SerializeField] private float m_knockBack = 5f;

    [Tooltip("The number of projectiles that will be spawned when the gun shoots")]
    [SerializeField] private int m_numProjectilesPerShot = 1;
    [Tooltip("Currently does nothing")]
    [SerializeField] private int m_burstProjectiles = 1;
    [Tooltip("The ammount of ammo that the entity wielding the gun can hold outside of the clip")]
    [SerializeField] private int m_maxAmmo = 100;
    [Tooltip("The number of times the gun can be shot before needing to reload from full")]
    [SerializeField] private int m_clipSize = 6;
    [Tooltip("The damage each projectile does on impact")]
    [SerializeField] private int m_damage = 1;

    [Tooltip("shows when this gun is being reloaded")]
    [SerializeField] private bool m_isReloading = false;
    [Tooltip("Determines if mouse press or hold mouse down to shoot")]
    [SerializeField] private bool m_isAutomatic = false;
    #endregion

    //----------------------------
    #region Private member variables
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

    //----------------------------
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

    public Transform Muzzle {
        get { return m_muzzle; }
        set { m_muzzle = value; }
    }

    public int ClipSize {
        get { return m_clipSize; } 
        set { m_clipSize = value; }
    }

    public bool IsReloading {
        get { return m_isReloading; } 
        set { m_isReloading = value; }
    }
    #endregion

    //----------------------------
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
        if (a_currentClip > ClipSize)
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
    public int GetMaxAmmo() { return m_maxAmmo + ClipSize; }
    public Transform GetMuzzle() { return Muzzle; }
    #endregion

    //----------------------------
    //Methods and functionality
    //----------------------------
    #region ammo control

    //reloads the gun and also prevents shooting for a time based on the reload time in seconds variable
    //returns true if the gun can reload or false if it cannot
    public bool Reload() {
        if (CurrentClip == ClipSize)
            return false;

        if (IsReloading == false) {
            TimeUntilNextAction = Time.time + m_reloadTimeInSeconds;
            IsReloading = true;
            IsIdle = false;
            if (CurrentAmmo < ClipSize) {
                CurrentClip = CurrentAmmo;
                if (m_infiniteAmmo == false)
                    CurrentAmmo = 0;
            }
            else {
                if (m_infiniteAmmo == false)
                    CurrentAmmo -= ClipSize - CurrentClip;
                CurrentClip = ClipSize;
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
        if (CurrentClip < ClipSize && CurrentAmmo > 0) {
            if (IsIdle) {
                if (IsReloading == false)
                    IsReloading = true;

                TimeUntilNextAction = Time.time + m_reloadTimeInSeconds;
                m_setToReloadOne = true;
                IsIdle = false;
                if (m_audioSource != null)
                    m_audioSource.PlayOneShot(m_reloadSound, 0.3f);
            }
            else if (SetToReloadOne) {
                CurrentClip++;
                if(m_infiniteAmmo == false)
                    CurrentAmmo--;
                if (CurrentClip == ClipSize)
                    IsFull = true;
                SetToReloadOne = false;
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

    //----------------------------
    #region shoot
    //Randomises the projectiles angle based on the dispersion angle variable
    private Vector3 RandomAngle() {
        Vector3 direction = Muzzle.forward + Random.insideUnitSphere * m_dispersionAngle;
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
                    Projectile newProjectile;
                    if (m_parent != null)
                        newProjectile = Instantiate(m_projectile, Muzzle.position, Muzzle.rotation * Quaternion.Euler(RandomAngle()), m_parent.transform) as Projectile;
                    else
                        newProjectile = Instantiate(m_projectile, Muzzle.position, Muzzle.rotation * Quaternion.Euler(RandomAngle())) as Projectile;

                    newProjectile.KnockBack = m_knockBack;
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
                    GameObject GO = Instantiate(m_shootParticleSystem, Muzzle.position, transform.rotation);
                    Destroy(GO, m_shootParticleLifeTime);
                }
                if (m_smokeParticleSystem != null) {
                    GameObject GO = Instantiate(m_smokeParticleSystem, Muzzle.position, transform.rotation);
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
        CurrentClip = ClipSize;
        m_audioSource = GetComponent<AudioSource>();
        if (m_parent != null)
            m_parent = Instantiate(m_parent, Vector3.zero, Quaternion.identity);
    }
    //Keeps track of whether this gun is reloading
    //and stops the ammo stores from going higher than their respective maximums
    private void Update() {
        if (Time.time > TimeUntilNextAction) {
            if (IsReloading) 
                IsReloading = false;
            if (SetToReloadOne)
                ReloadOne();
            IsIdle = true;
        }
        if (CurrentAmmo > m_maxAmmo)
            CurrentAmmo = m_maxAmmo;
        if (CurrentClip > ClipSize)
            CurrentClip = ClipSize;
    }
    
}
