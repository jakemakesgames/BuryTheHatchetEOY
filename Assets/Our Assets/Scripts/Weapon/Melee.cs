using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Michael Corben
//Created 31/07/2018
//Last edited 25/08/2018

public class Melee : MonoBehaviour {

    [Tooltip("The points on the weapon which will shoot out" +
        "a ray each to detect for collision")]
    [SerializeField] private GameObject[] m_contactPoints;
    [Tooltip("DOES NOT CONTROL SPEED," +
        "Is used to tell they raycasts how far to shoot out from the blade")]
    [SerializeField] private float m_swingSpeed;
    [Tooltip("Controls how long after  a swing has ended before the" +
        "weapon can swing again")]
    [SerializeField] private float m_coolDown;
    [SerializeField] private int m_damage;
    [Tooltip("What entites can be hit by this weapon")]
    [SerializeField] private LayerMask m_entityCollisionMask;
    [Tooltip("What destroyable objects can be hit by this weapon")]
    [SerializeField] private LayerMask m_destroyableCollisionMask;
    [SerializeField] private Animator m_animator;

    private float m_skinWidth = 0.1f;
    private bool m_isSwinging = false;
    private bool m_isIlde = true;

    public bool IsIlde {
        get { return m_isIlde; }
        set { m_isIlde = value; }
    }

    public void Swing() {
        if (!m_isSwinging)
        {
            //do the swing animaion

            m_isSwinging = true;
            IsIlde = false;
        }
    }

    //to be called by the animator in the animations last frame
    public void EndSwing() {
        m_isSwinging = false;
        IsIlde = true;
    }

    public void Update() {
        if (m_isSwinging) {
            CheckCollisions(m_swingSpeed);
        }
    }

    public void SetEntityCollisionLayer(LayerMask a_collsionMask) {
        m_entityCollisionMask = a_collsionMask;
    }
    public void SetDestroyableCollisionLayer(LayerMask a_collsionMask) {
        m_destroyableCollisionMask = a_collsionMask;
    }

    //use ray casts to check for collisions
    private void CheckCollisions(float a_distanceToMove) {
        for (int i = 0; i < m_contactPoints.Length; i++) {
            Ray ray = new Ray(m_contactPoints[i].transform.position, m_contactPoints[i].transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, a_distanceToMove + m_skinWidth, m_entityCollisionMask)) {
                OnHitObject(hit);
            }
            if (Physics.Raycast(ray, out hit, a_distanceToMove + m_skinWidth, m_destroyableCollisionMask)) {
                OnHitObject(hit);
            }
        }
    }
    private void OnHitObject(RaycastHit a_hit) {
        IDamagable damagableObject = a_hit.collider.GetComponent<IDamagable>();
        if (damagableObject != null) {
            damagableObject.TakeHit(m_damage, a_hit);
        }
    }
    private void OnHitObject(Collider a_c) {
        IDamagable damagableObject = a_c.GetComponent<IDamagable>();
        if (damagableObject != null) {
            damagableObject.TakeDamage(m_damage);
        }
    }
}
