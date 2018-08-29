using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Michael Corben
//Created 29/08/2018
//Last edited 29/08/2018

public class DestructibleObject : MonoBehaviour, IDamagable {

    #region Inspector variables
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
    [Tooltip("The mesh of this object when at this spot's number of health")]
    [SerializeField] private List<Mesh> m_healthStages;
    #endregion

    private MeshFilter m_meshFilter;

    #region IDamagable methods
    public void TakeDamage(int a_damage) {
        m_health -= a_damage;
    }

    public void TakeHit(int a_damage, RaycastHit a_hit) {
        TakeDamage(a_damage);
    }

    public void TakeImpact(int a_damage, RaycastHit a_hit, Projectile a_projectile) {
        TakeHit(a_damage, a_hit);
    }
    #endregion

    private void Awake()
    {
        m_meshFilter = gameObject.GetComponent<MeshFilter>();
    }

    //Called when this takes a hit, plays a particle and checks if this should be destroyed.
    private void OnHit() {
        if (m_health <= 0) {
            GameObject GO = Instantiate(m_destructionParticle);
            Destroy(GO, m_destructionParticleLifeTime);
            Destroy(gameObject);
        }
        else 
            m_hitParticle.Play();
        if (m_healthStages[m_health - 1] != null)
            m_meshFilter.mesh = m_healthStages[m_health - 1];
    }
}
