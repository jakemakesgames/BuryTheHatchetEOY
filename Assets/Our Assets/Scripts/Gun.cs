using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Michael Corben
//Based on Tutorial:https://www.youtube.com/watch?v=rZAnnyensgs&list=PLFt_AvWsXl0ctd4dgE1F8g3uec4zKNRV0&index=3
//Created 24/07/2018
//Last edited 31/07/2018

public class Gun : MonoBehaviour {

    public Transform m_muzzle;
    public Projectile m_projectile;

    public float m_msBetweenShots = 100;
    public float m_reloadTimeInMilliseconds = 100;
    public float m_muzzleVelocity = 15;
    public float m_bulletLifeTime = 15;
    public float m_dispersionAngle = 0;

    public int m_numProjectilesPerShot = 1;
    public int m_burstProjectiles = 1;
    public int m_maxAmmo = 100;
    public int m_clipSize = 6;
    public int m_damage = 1;

    public bool m_isReloading = false;
    public bool m_isAutomatic = false;

    private LayerMask m_entityCollisionMask;
    private LayerMask m_environmentCollisionMask;

    private float m_nextShotTime;

    [SerializeField] private int m_currentAmmo;
    [SerializeField] private int m_currentClip;

    private bool m_infiniteAmmo = false;

    public void SetEntityCollisionLayer(LayerMask a_collsionMask) {
        m_entityCollisionMask = a_collsionMask;
    }
    public void SetEnvironmentCollisionLayer(LayerMask a_collsionMask) {
        m_environmentCollisionMask = a_collsionMask;
    }
    public void SetInfiniteAmmo(bool a_infiniteAmmo) {
        m_infiniteAmmo = a_infiniteAmmo;
    }
    public int GetCurrentClip() { return m_currentClip; }
    public int GetCurrentAmmo() { return m_currentAmmo; }
    public int GetTotalAmmo() { return GetCurrentAmmo() + GetCurrentClip(); }
    public int GetMaxAmmo() { return m_maxAmmo + m_clipSize; }

    //reloads the gun and also prevents shooting for a time based on the reload time in milliseconds variable
    public void Reload() {
        m_nextShotTime = Time.time + m_reloadTimeInMilliseconds / 1000;
        m_isReloading = true;
        if (m_currentAmmo < m_clipSize) {
            m_currentClip = m_currentAmmo;
            if (!m_infiniteAmmo) {
                m_currentAmmo = 0;
            }
        }
        else
        {
            if (!m_infiniteAmmo) {
                m_currentAmmo -= m_clipSize - m_currentClip;
            }
            m_currentClip = m_clipSize;
        }
    }

    //tells who asks is if the gun is empty
    public bool GetIsEmpty() {
        if (m_currentClip <= 0) {
            return true;
        }
        else {
            return false;
        }
    }

    //Randomises the projectiles angle based on the dispersion angle variable
    private Vector3 RandomAngle() {
        Vector3 direction = m_muzzle.forward + Random.insideUnitSphere * m_dispersionAngle;
        direction.x = 0;
        return direction;
    }

    //creates projectiles will only shoot after reload time
    public void Shoot() {
        if (Time.time > m_nextShotTime) {
            if (m_currentClip > 0) {
                m_nextShotTime = Time.time + m_msBetweenShots / 1000;
                for (int i = 0; i < m_numProjectilesPerShot; i++) {
                    Projectile newProjectile = Instantiate(m_projectile, m_muzzle.position, m_muzzle.rotation * Quaternion.Euler(RandomAngle())) as Projectile;
                    newProjectile.SetDamage(m_damage);
                    newProjectile.SetSpeed(m_muzzleVelocity);
                    newProjectile.SetLifeTime(m_bulletLifeTime);
                    newProjectile.SetEntityCollisionLayer(m_entityCollisionMask);
                    newProjectile.SetTerrainCollisionLayer(m_environmentCollisionMask);
                }
                m_currentClip--;
            }
        }
    }

    private void Awake() {
        m_currentAmmo = m_maxAmmo;
        m_currentClip = m_clipSize;
    }
    private void Update() {
        if (m_isReloading) {
            if (Time.time > m_nextShotTime) {
                m_isReloading = false;
            }
        }
    }
    
}
