using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Michael Corben
//Based on Tutorial:https://www.youtube.com/watch?v=rZAnnyensgs&list=PLFt_AvWsXl0ctd4dgE1F8g3uec4zKNRV0&index=3
//Created 24/07/2018
//Last edited 25/07/2018

public class Gun : MonoBehaviour {

    public Transform m_muzzle;
    public Projectile m_projectile;
    public float m_msBetweenShots = 100;
    public float m_muzzleVelocity = 35;
    public float m_effectiveDistance = 15;

    private float nextShotTime;
    public void Shoot() {

        if (Time.time > nextShotTime) {
            nextShotTime = Time.time + m_msBetweenShots / 1000;
            Projectile newProjectile = Instantiate(m_projectile, m_muzzle.position, m_muzzle.rotation) as Projectile;
            newProjectile.SetSpeed(m_muzzleVelocity);
        }
    }
    
}
