using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using StateMachine;

//Mark Phillips
//Created 13/08/2018
//Last edited 29/08/2018

//[RequireComponent(typeof(WeaponController))]
//[RequireComponent(typeof(NavMeshAgent))]
//[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(AudioSource))]
public class AI : BaseAI
{
    enum STATE
    {
        SEEK,
        PEEK,
        STATIONARY,
        FINDCOVER,
        RELOAD
    }
    enum DETECTION
    {
        CLEAR,
        BEHINDCOVER,
        BLOCKED,
        NONE
    }

    #region Inspector Variables
    //[Tooltip("Max health value, currently at 5 (5 hits to die).")]
    //[SerializeField]
    //private float m_maxHealth = 5;

    [Header("AI State Radii")]
    [Tooltip("Seek radius/green sphere - distance to seek to player.")]
    [SerializeField]
    private float m_seekFromCoverRadius;
    [Tooltip("Seek radius/green sphere - distance to seek to player.")]
    [SerializeField]
    private float m_alertRadius;
    [Tooltip("Flee radius/Blue sphere (small) - distance to flee from player.")]
    [SerializeField]
    private float m_fleeRadius;
    [Tooltip("Cover radius/Green sphere - finds objects on the cover layer.")]
    [SerializeField]
    private float m_coverRadius;
    [Tooltip("Attack radius/Red sphere - shoots at player.")]
    [SerializeField]
    private float m_attackRadius;
    //[Tooltip("Accounts for floating point precision - when AI knows to switch to Stationary")]
    //[SerializeField]
    //private float m_deadZone;
    [Tooltip("The distance from enemy to cover point at which to stop calculating a new position.")]
    [SerializeField]
    private float m_coverFoundThreshold = 3;
    [Tooltip("Distance behind the enemy's hand to ensure the ray casts do not start in a collider")]
    [SerializeField]
    private float m_rayDetectBufferDist;
    //[Tooltip("World height of body when dead")]
    //[SerializeField]
    //private float m_bodyDropHeight;
    [Tooltip("Minimum delay in seconds that AI will leave cover to attack")]
    [SerializeField]
    private float m_minAttackDelay;
    [Tooltip("Maximum delay in seconds that AI will leave cover to attack")]
    [SerializeField]
    private float m_maxAttackDelay;
    [Tooltip("Transform for gun's raycast")]
    [SerializeField]
    private Transform m_gunRayTransform;


    [Header("Collison Mask Layers")]
    [Tooltip("Set this to the Cover layer for cover collision detection")]
    [SerializeField]
    private LayerMask m_coverLayer;
    [Tooltip("Set this to the Environment layer for environment collision detection")]
    [SerializeField]
    private LayerMask m_environmentLayer;

    //[Header("Sounds")]
    //[SerializeField]
    //private List<AudioClip> m_deathSounds;

    //[Header("Animation")]
    //[SerializeField]
    //private Animator m_enemyAnimator;
    //[Tooltip("The number of death animations (starting at 0)")]
    //[SerializeField]
    //private int deathAnimationCount;

    //[Header("Particles")]
    //[Tooltip("Paricles that will play when the enemy is walking")]
    //[SerializeField]
    //private ParticleSystem m_walkingParticleSystem;
    #endregion

    // private float m_enemyRadius;

    //private AudioSource m_audioSource;

    //private float m_health;
    //private float m_distBetweenPlayer;
    private float m_gunDistToPlayer;
    private float m_distBetweenCover;
    //private float m_counter = 0;
    private float m_time = 0;
    //private bool m_isDead = false;
    //private bool m_hasDropped = false;
    //private bool m_hasDroppedTrigger = false;
    private bool m_finishedReload = true;
    private bool m_atCover = false;
    private bool m_noCover = false;

    private STATE m_state;

    private Vector3 m_coverPos;
    private Transform m_currCoverObj;
    private Transform m_nextCoverObj;
    //private NavMeshAgent m_agent;
    private LineRenderer m_line;
    //private GameObject m_player;
    private List<Transform> m_coverPoints;

    //private WeaponController m_weaponController;
    private Gun m_gun;
    private Melee m_melee;

    //public delegate void Dead(AI enemy);
    //public Dead OnDeath;

    private StateMachine<AI> m_stateMachine;

    #region Properties 
    //public NavMeshAgent Agent { get { return m_agent; } }
    //public Vector3 PlayerPosition { get { return m_player.transform.position; } }
    public float CoverRadius { get { return m_coverRadius; } }
    public LayerMask CoverLayer { get { return m_coverLayer; } }
    public float CoverFoundThreshold { get { return m_coverFoundThreshold; } }
    public Gun Gun { get { return m_gun; } }
    public List<Transform> CoverPoints { get { return m_coverPoints; } }
    //public bool HasDroppedTrigger { set { m_hasDroppedTrigger = value; } }
    public bool FinishedReload { set { m_finishedReload = value; } }
    public Vector3 CoverPos { get { return m_coverPos; } set { m_coverPos = value; } }
    public bool AtCover { get { return m_atCover; } set { m_atCover = value; } }
    public Transform CurrCoverObj { get { return m_currCoverObj; } set { m_currCoverObj = value; } }
    public bool NoCover { get { return m_noCover; } set { m_noCover = value; } }
    public Transform NextCoverObj { get { return m_nextCoverObj; } set { m_nextCoverObj = value; } }
    #endregion

    protected override void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_coverPoints = new List<Transform>();
        //m_weaponController = GetComponent<WeaponController>();
        base.Awake();
    }

    protected override void Start()
    {
        m_line = GetComponent<LineRenderer>();
        //m_agent = GetComponent<NavMeshAgent>();
        //m_audioSource = GetComponent<AudioSource>();
        //m_enemyAnimator = GetComponentInChildren<Animator>();
        m_stateMachine = new StateMachine<AI>(this);
        m_stateMachine.ChangeState(new FindCover());
        m_state = STATE.FINDCOVER;
        m_time = Time.time + Random.Range(m_minAttackDelay, m_maxAttackDelay);
        //m_health = m_maxHealth;

        if (m_weaponController.GetEquippedGun() != null)
        {
            m_gun = m_weaponController.GetEquippedGun();
            m_gun.SetInfiniteAmmo(true);
        }
        else if (m_weaponController.GetEquippedMelee() != null)
            m_melee = m_weaponController.GetEquippedMelee();

        base.Start();

    }

    protected override void Update()
    {

        if (m_isDead == false)
        {
            //To Do: put this in coroutine
            Debug.Log("Current State: " + m_state);
            DrawLinePath(m_agent.path);
            //m_distBetweenPlayer = Vector3.Distance(transform.position, m_player.transform.position);
            if (CurrCoverObj != null)
            {
                m_distBetweenCover = Vector3.Distance(transform.position, CurrCoverObj.transform.position);
            }
            m_gunDistToPlayer = Vector3.Distance(m_weaponController.GetEquippedWeapon().transform.position, m_player.transform.position);

            //if (m_health <= 0)
            //{
            //    Die();
            //    return;
            //}

            if (ClearShot())
                Attack();

            if (CheckStates())
                SwitchState();

            m_stateMachine.Update();

            //UpdateAnims();
            //UpdateParticles();

        }
        //else
        //{
        //    if (m_hasDroppedTrigger)
        //    {
        //        DropDead();
        //    }
        //}
        base.Update();
    }

    //Check states and return true if there is a state change
    bool CheckStates()
    {
        STATE prevState = m_state;

        //does this work?
        if (m_distBetweenPlayer < m_attackRadius)
        {
            transform.LookAt(PlayerPosition);
            
            if(m_distBetweenCover < m_seekFromCoverRadius)
            {
                if (m_time < Time.time)
                {
                    float choice = Random.Range(0, 100);
                    if (choice <= 50)
                    {
                        if (m_state != STATE.FINDCOVER)
                        {
                            m_state = STATE.PEEK;
                            m_time = Time.time + Random.Range(m_minAttackDelay, m_maxAttackDelay);
                        }
                    }
                    if (choice > 50)
                    {
                        m_state = STATE.FINDCOVER;
                        m_time = Time.time + Random.Range(m_minAttackDelay, m_maxAttackDelay);
                    }
                }
            }
            //if(Gun.CurrentClip <= 2)
            //{
            //    m_state = STATE.PEEK;
            //}
            if (m_distBetweenCover < m_seekFromCoverRadius && AtCover) // no other cover
            {
                m_state = STATE.PEEK;
            }

            if (m_distBetweenCover <= m_seekFromCoverRadius + m_deadZone && m_distBetweenCover >= m_seekFromCoverRadius - m_deadZone)
            {
                if (m_state != STATE.FINDCOVER && prevState != STATE.FINDCOVER)
                {
                    //If player is within cover and we're at max seek from cover distance (deadzone for floating point precision)
                    m_state = STATE.STATIONARY;
                }
            }

        }
        else
        {
            m_state = STATE.FINDCOVER;
            //TO DO: WANDER STATE
        }

        if (CurrCoverObj != null)
        {
            if (m_distBetweenCover < m_seekFromCoverRadius)
            {
                CurrCoverObj.tag = "CoverTaken";
            }
            else
            {
                CurrCoverObj.tag = "CoverFree";
                CurrCoverObj = NextCoverObj;
            }
        }

        //if (m_gun.GetIsEmpty())
        //{
        //    if (NoCover == false)
        //    {
        //        m_state = STATE.FINDCOVER;
        //    }
        //    else
        //    {
        //        m_state = STATE.RELOAD;
        //    }
        //
        //}

        if (AtCover)
        {
            if (m_finishedReload == false || m_gun.GetIsEmpty())
            {
                m_state = STATE.RELOAD;
            }
            else
            {
                AtCover = false;
            }
        }
        else if (NoCover)
        {
            if (m_finishedReload == false || m_gun.GetIsEmpty())
            {
                m_state = STATE.RELOAD;
            }
        }
 
        if (m_gunDistToPlayer < m_attackRadius && m_gun.GetIsEmpty() == false)
        {
            if (m_finishedReload)
            {
                //if (ClearShot() == DETECTION.BLOCKED)
                //{
                //    m_state = STATE.FINDCOVER;
                //}
            }
        }

        if (prevState != m_state)
            return true;

        else
            return false;
    }

    void SwitchState()
    {
        switch (m_state)
        {
            case STATE.PEEK:
                m_stateMachine.ChangeState(new Peek());
                break;
            case STATE.FINDCOVER:
                m_stateMachine.ChangeState(new FindCover());
                break;
            case STATE.STATIONARY:
                m_stateMachine.ChangeState(new Stationary());
                break;
            case STATE.RELOAD:
                m_stateMachine.ChangeState(new Reload());
                break;
            case STATE.SEEK:
                m_stateMachine.ChangeState(new Seek());
                break;
            default:
                return;
        }
    }

    //Check if there is nothing blocking the gun
    bool ClearShot()
    {
        Vector3 vecBetween = (new Vector3(m_player.transform.position.x, m_gunRayTransform.position.y, m_player.transform.position.z) - m_gunRayTransform.position);
        Vector3 rayPos1 = m_gunRayTransform.position - (m_gunRayTransform.forward * m_rayDetectBufferDist);
        Vector3 rayPos2 = rayPos1 + m_gunRayTransform.right * 0.1f;
        rayPos1 -= m_gunRayTransform.right * 0.1f;

        Ray ray1 = new Ray(rayPos1, vecBetween);
        Ray ray2 = new Ray(rayPos2, vecBetween);

        Debug.DrawRay(rayPos1, vecBetween, Color.green);
        Debug.DrawRay(rayPos2, vecBetween, Color.green);

        RaycastHit hit;

        if (Physics.Raycast(ray1, out hit, vecBetween.magnitude + m_rayDetectBufferDist, m_environmentLayer))
        {
            if (hit.transform == CurrCoverObj)
                return false;    
        }
        if (Physics.Raycast(ray2, out hit, vecBetween.magnitude + m_rayDetectBufferDist, m_environmentLayer))
        {
            if (hit.transform == CurrCoverObj)
                return false;
        }

        return true;

        //RaycastHit[] hits1;
        //hits1 = Physics.RaycastAll(ray1, vecBetween.magnitude + m_rayDetectBufferDist, m_environmentLayer);
        //RaycastHit[] hits2;
        //hits2 = Physics.RaycastAll(ray2, vecBetween.magnitude + m_rayDetectBufferDist, m_environmentLayer);


        //if (hits1.Length == 0)
        //{
        //    if (hits2.Length == 0)
        //    {
        //        return DETECTION.CLEAR;
        //    }
        //    else if (hits2.Length == 1)
        //    {
        //        if (hits2[0].transform == CurrCoverObj)
        //        {
        //            return DETECTION.BEHINDCOVER;
        //        }
        //    }          
        //}
        //if (hits1.Length == 1)
        //{
        //    if (hits2.Length < 2)
        //    {
        //        if (hits1[0].transform == CurrCoverObj)
        //        {
        //            if (hits2.Length == 0)
        //            {
        //                return DETECTION.BEHINDCOVER;
        //            }
        //            else if (hits2[0].transform == CurrCoverObj)
        //            {
        //                return DETECTION.BEHINDCOVER;
        //            }
        //        }
        //    }
        //}

        //return DETECTION.BLOCKED;

        //if (hits1.Length == 0 && hits2.Length == 0)
        //{
        //    return DETECTION.CLEAR;
        //}
        //else if (hits1.Length == 1 && hits2.Length == 1)
        //{
        //    if (hits1[0].transform == CurrCoverObj && hits2[0].transform == CurrCoverObj)
        //    {
        //        return DETECTION.BEHINDCOVER;
        //    }
        //    else
        //    {
        //        return DETECTION.BLOCKED;
        //    }
        //}
        //else
        //{
        //    return DETECTION.BLOCKED;
        //}

    }
    protected override void Attack()
    {
        if (m_gunDistToPlayer < m_attackRadius && m_finishedReload)
        {
            m_weaponController.m_weaponHold.LookAt(PlayerPosition);
            if (Gun.Shoot())
            {
                m_enemyAnimator.SetTrigger("Shoot");
            }

        }
        base.Attack();
    }

    protected override void Die()
    {
        //m_agent.ResetPath();
        //m_walkingParticleSystem.Stop();
        //int randomAnim = Random.Range(0, deathAnimationCount);
        //m_enemyAnimator.SetInteger("WhichDeath", 2);
        //m_enemyAnimator.SetTrigger("Death");
        //RandomPitch();
        //if (m_deathSounds.Count != 0)
        //{
        //    m_audioSource.PlayOneShot(m_deathSounds[Random.Range(0, m_deathSounds.Count)]);
        //}
        //GetComponent<NavMeshAgent>().enabled = false;
        //m_isDead = true;
        base.Die();
    }

    //void DropDead()
    //{
    //    if (m_hasDropped == false)
    //    {
    //        m_counter += Time.deltaTime;
    //        Vector3 target = new Vector3(transform.position.x, m_bodyDropHeight, transform.position.z);
    //
    //        transform.position = Vector3.Lerp(transform.position, target, m_counter);
    //
    //        if (transform.position.y == m_bodyDropHeight)
    //        {
    //            m_hasDropped = true;
    //        }
    //    }
    //}

   // public void TakeHit(int a_damage, RaycastHit a_hit)
   // {
   //     TakeDamage(a_damage);
   // }
   //
   // public void TakeDamage(int a_damage)
   // {
   //     m_health -= a_damage;
   // }
   //
   // public void TakeImpact(int a_damage, RaycastHit a_hit, Projectile a_projectile)
   // {
   //     TakeHit(a_damage, a_hit);
   // }

   // private void UpdateAnims()
   // {
   //     float myVelocity = m_agent.velocity.magnitude;
   //     Vector3 localVel = transform.InverseTransformDirection(m_agent.velocity.normalized);
   //
   //     m_enemyAnimator.SetFloat("Velocity", myVelocity);
   //
   //     m_enemyAnimator.SetFloat("MovementDirectionRight", localVel.x);
   //     m_enemyAnimator.SetFloat("MovementDirectionForward", localVel.z);
   // }

    //private void UpdateParticles()
    //{
    //    if (m_agent.velocity != Vector3.zero)
    //    {
    //        if (m_walkingParticleSystem.isPlaying == false)
    //            m_walkingParticleSystem.Play();
    //    }
    //    else
    //    {
    //        m_walkingParticleSystem.Stop();
    //    }
    //}

    private void RandomPitch()
    {
        m_audioSource.pitch = Random.Range(0.95f, 1.05f);
    }

    void DrawLinePath(NavMeshPath path)
    {
        if (path.corners.Length < 2) //if the path has 1 or no corners, there is no need
            return;

        m_line.positionCount = path.corners.Length; //set the array of positions to the amount of corners

        for (int i = 0; i < path.corners.Length; i++)
        {
            m_line.SetPosition(i, path.corners[i]); //go through each corner and set that to the line renderer's position
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(CoverPos, m_seekFromCoverRadius);
        //Gizmos.DrawWireSphere(transform.position, m_fleeRadius);
        Gizmos.color = Color.red;
        if (m_weaponController != null)
        {
            Gizmos.DrawWireSphere(m_weaponController.m_weaponHold.transform.position, m_attackRadius);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(m_weaponController.m_weaponHold.transform.position, m_coverRadius);
        }
    }
#endif
}














