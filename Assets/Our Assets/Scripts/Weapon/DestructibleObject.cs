﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Michael Corben
//Created 29/08/2018
//Last edited 04/09/2018

public class DestructibleObject : MonoBehaviour, IDamagable {

    #region Inspector variables
    [Tooltip("if this object will be completely destroyed when it looses all it's health")]
    [SerializeField] private bool m_willDestroy = true;

    [Tooltip("Amount of damage taken required for this object to be destroyed")]
    [SerializeField] private int m_health = 1;

    [Tooltip("The time in seconds the particle system that plays" +
        " the destruction particles should exist for after this is destroyed")]
    [SerializeField] private float m_destructionParticleLifeTime;

    [Tooltip("The particle that'll play when this object is damaged, " +
        "will not play if this is destroyed however")]
    [SerializeField] private ParticleSystem m_hitParticle;

    [Tooltip("The particle that'll play when thsi object is destroyed")]
    [SerializeField] private GameObject m_destructionParticle;

    [Tooltip("The mesh of this object when at this spot's number minus one, of health")]
    [SerializeField] private List<Mesh> m_healthStages;
    #endregion

    private MeshFilter m_meshFilter;

    public int Health {
        get { return m_health; } 
        set { m_health = value; }
    }

    #region IDamagable methods
    public void TakeDamage(int a_damage) {
        Health -= a_damage;
        OnHit();
    }

    public void TakeHit(int a_damage, RaycastHit a_hit) {
        TakeDamage(a_damage);
    }

    public void TakeImpact(int a_damage, RaycastHit a_hit, Projectile a_projectile) {
        TakeHit(a_damage, a_hit);
    }
    #endregion

    //Called when this takes a hit, plays a particle, checks if this should be destroyed and changes the mesh.
    private void OnHit() {
        if (Health <= 0)
        {
            if (m_destructionParticle != null) {
                GameObject GO = Instantiate(m_destructionParticle);
                Destroy(GO, m_destructionParticleLifeTime);
            }
            if (m_willDestroy) {
                Destroy(gameObject);
                return;
            }
        }
        else {
            if (m_hitParticle != null)
                m_hitParticle.Play();
        }
        if (m_healthStages[Health - 1] != null)
            m_meshFilter.mesh = m_healthStages[Health - 1];
    }

    private void Awake()
    {
        m_meshFilter = gameObject.GetComponent<MeshFilter>();
    }
}
