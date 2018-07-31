using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour {

    [SerializeField] private GameObject[] m_contactPoints;
    [SerializeField] private GameObject m_positionOne;
    [SerializeField] private GameObject m_positionTwo;
    [SerializeField] private float m_swingSpeed;
    [SerializeField] private float m_coolDown;
    [SerializeField] private int m_damage;
    [SerializeField] private LayerMask m_entityCollisionMask;
    [SerializeField] private LayerMask m_environmentCollisionMask;

    private float m_skinWidth = 0.1f;
    private bool m_isSwinging = false;
    
    public void Swing() {
        if (!m_isSwinging) {
            m_isSwinging = true;
        }
    }

    public void Update()
    {
        if (m_isSwinging)
        {
            //do the swing animaion if the animation isn't already playing

            CheckCollisions(1f);
        }
    }

    public void SetEntityCollisionLayer(LayerMask a_collsionMask) {
        m_entityCollisionMask = a_collsionMask;
    }
    public void SetEnvironmentCollisionLayer(LayerMask a_collsionMask) {
        m_environmentCollisionMask = a_collsionMask;
    }

    //use ray casts to check for collisions
    private void CheckCollisions(float a_distanceToMove) {
        for (int i = 0; i < m_contactPoints.Length; i++) {
            Ray ray = new Ray(m_contactPoints[i].transform.position, m_contactPoints[i].transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, a_distanceToMove + m_skinWidth, m_entityCollisionMask)) {
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
