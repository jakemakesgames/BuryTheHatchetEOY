using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Michael Corben
//Created 31/07/2018
//Last edited 25/08/2018

    [RequireComponent(typeof(AudioSource))]
public class Melee : MonoBehaviour {

    #region inspector variables
        [Tooltip("DOES NOT CONTROL SPEED, " +
            "Is used to tell they raycasts how far to shoot out from the blade")]
        [SerializeField] private float m_swingSpeed;
        [Tooltip("Controls how long after a swing has ended before the " +
            "weapon can swing again")]
        [SerializeField] private float m_coolDown;
        [Tooltip("Used to determine the size of the initial collision " +
        "sphere around the weapon when it is swung")]
        [SerializeField] private float m_initCollSphereSize = .1f;
        [SerializeField] private int m_damage;
        [Tooltip("What entites can be hit by this weapon")]
        [SerializeField] private LayerMask m_entityCollisionMask;
        [Tooltip("What destroyable objects can be hit by this weapon")]
        [SerializeField] private LayerMask m_destroyableCollisionMask;
        [SerializeField] private Animator m_animator;
        [Tooltip("The points on the weapon which will shoot out " +
            "a ray each to detect for collision")]
        [SerializeField] private GameObject[] m_contactPoints;
        [Tooltip("The sound made by this weapon when it is swung")]
        [SerializeField] private AudioClip m_swooshSound;
        [Tooltip("The sound made when this weapon collides with a living entity")]
        [SerializeField] private AudioClip m_hitEntitySound;
    #endregion

    #region private member variables
        private float m_coolDownTimer;
        private float m_skinWidth = 0.1f;
        private bool m_isSwinging = false;
        private bool m_isIdle = true;
        private AudioSource m_audioSource;
    #endregion

    public bool IsIdle {
        get { return m_isIdle; }
        set { m_isIdle = value; }
    }

    //Turn on raycasts in anticipation for being moved
    public void Swing() {
        if (!m_isSwinging && m_coolDownTimer <= 0) {
            m_isSwinging = true;
            IsIdle = false;
            m_audioSource.PlayOneShot(m_swooshSound);
            m_coolDownTimer = m_coolDown;

            //If the weapon starts swinging within a collider,
            //it will activate the appropriate collision response
            Collider[] initialEnemyCollision = Physics.OverlapSphere(transform.position,
                m_initCollSphereSize, m_entityCollisionMask);
            if (initialEnemyCollision.Length > 0) {
                OnHitObject(initialEnemyCollision[0]);
                return;
            }

            Collider[] initialEnvironmentCollision = Physics.OverlapSphere(transform.position, 
                m_initCollSphereSize, m_destroyableCollisionMask);
            if (initialEnvironmentCollision.Length > 0) {
                OnHitObject(initialEnvironmentCollision[0]);
                return;
            }
        }
    }

    //Tells this that the weapon is no longer being moved 
    //and to therefore stop casting rays
    public void EndSwing() {
        m_isSwinging = false;
        IsIdle = true;
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
                if (m_hitEntitySound != null) {
                    m_audioSource.PlayOneShot(m_hitEntitySound);
                }
            }
            if (Physics.Raycast(ray, out hit, a_distanceToMove + m_skinWidth, m_destroyableCollisionMask)) {
                OnHitObject(hit);
            }
        }
    }

    //Called when the raycasts hit an object on a particular layer
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

    //Store requied componenets for later use
    private void Awake() {
        m_audioSource = GetComponent<AudioSource>();
    }

    //If this weapon thinks it is swinging the update will check for collisions.
    //If it isn't swinging but has recently the update will run the cool down.
    private void Update() {
        if (m_isSwinging) {
            CheckCollisions(m_swingSpeed);
        }
        else if(m_coolDownTimer > 0) {
            m_coolDownTimer -= Time.deltaTime;
        }
    }
}
