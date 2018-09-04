//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AI;
//using StateMachine;

////Mark Phillips
////Created 13/08/2018
////Last edited 29/08/2018

//[RequireComponent(typeof(WeaponController))]
//[RequireComponent(typeof(NavMeshAgent))]
//[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(AudioSource))]
//public class AI : MonoBehaviour, IDamagable
//{
//    enum STATE
//    {
//        WANDER,
//        SEEK,
//        FLEE,
//        STATIONARY,
//        RELOAD
//    }

//    #region Inspector Variables
//    [Tooltip("Max health value, currently at 5 (5 hits to die).")]
//    [SerializeField] private float m_maxHealth = 5;

//    [Header("AI State Radii")]
//    [Tooltip("Seek radius/Blue sphere (large) - distance to seek to player.")]
//    [SerializeField] private float m_seekRadius;
//    [Tooltip("Flee radius/Blue sphere (small) - distance to flee from player.")]
//    [SerializeField] private float m_fleeRadius;
//    [Tooltip("Cover radius/Green sphere - finds objects on the cover layer.")]
//    [SerializeField] private float m_coverRadius;
//    [Tooltip("Attack radius/Red sphere - shoots at player.")]
//    [SerializeField] private float m_attackRadius;
//    [Tooltip("Accounts for floating point precision - when AI knows to switch to Stationary")]
//    [SerializeField] private float m_deadZone;
//    [Tooltip("The distance from enemy to cover point at which to stop calculating a new position.")]
//    [SerializeField] private float m_coverFoundThreshold = 3;
//    [Tooltip("Distance behind the enemy's hand to ensure the ray casts do not start in a collider")]
//    [SerializeField] private float m_rayDetectBufferDist;
//    [Tooltip("World height of body when dead")]
//    [SerializeField] private float m_bodyDropHeight;

//    [Header("Collison Mask Layers")]
//    [Tooltip("Set this to the Cover layer for cover collision detection")]
//    [SerializeField] private LayerMask m_coverLayer;
//    [Tooltip("Set this to the Environment layer for environment collision detection")]
//    [SerializeField] private LayerMask m_environmentLayer;

//    [Header("Sounds")]
//    [SerializeField] private List<AudioClip> m_deathSounds;

//    [Header("Animation")]
//    [SerializeField] private Animator m_enemyAnimator;
//    [Tooltip("The number of death animations (starting at 0)")]
//    [SerializeField] private int deathAnimationCount;

//    [Header("Particles")]
//    [Tooltip("Paricles that will play when the enemy is walking")]
//    [SerializeField] private ParticleSystem m_walkingParticleSystem;
//    #endregion

//   // private float m_enemyRadius;

//    private AudioSource m_audioSource;

//    private float m_health;
//    private float m_distBetweenPlayer;
//    private float m_gunDistToPlayer;
//    private float m_counter = 0;
//    private bool m_isDead = false;
//    private bool m_hasDropped = false;
//    private bool m_hasDroppedTrigger = false;
//    private bool m_finishedReload = true;

//    private STATE m_state;

//    private NavMeshAgent m_agent;
//    private LineRenderer m_line;
//    private GameObject m_player;
//    private List<Transform> m_coverPoints;


//    private WeaponController m_weaponController;
//    private Gun m_gun;
//    private Melee m_melee;

//    public delegate void Dead(AI enemy);
//    public Dead OnDeath;

//    private StateMachine<AI> m_stateMachine;

//    #region Properties 
//    public NavMeshAgent Agent { get { return m_agent; } }
//    public Vector3 PlayerPosition { get { return m_player.transform.position; } }
//    public float CoverRadius { get { return m_coverRadius; } }
//    public LayerMask CoverLayer { get { return m_coverLayer; } }
//    public float CoverFoundThreshold { get { return m_coverFoundThreshold; } }
//    public Gun Gun { get { return m_gun; } }
//    public List<Transform> CoverPoints { get { return m_coverPoints; } }
//    public bool HasDroppedTrigger { set { m_hasDroppedTrigger = value; } }
//    public bool FinishedReload { set { m_finishedReload = value; } }
//    #endregion

//    void Awake()
//    {
//        m_player = GameObject.FindGameObjectWithTag("Player");
//        m_coverPoints = new List<Transform>();
//        m_weaponController = GetComponent<WeaponController>();
//    }

//    void Start()
//    {
//        m_line = GetComponent<LineRenderer>();
//        m_agent = GetComponent<NavMeshAgent>();
//        m_audioSource = GetComponent<AudioSource>();
//        m_enemyAnimator = GetComponentInChildren<Animator>();
//        m_stateMachine = new StateMachine<AI>(this);
//        m_stateMachine.ChangeState(new Wander());

//        if (m_weaponController.GetEquippedGun() != null)
//        {
//            m_gun = m_weaponController.GetEquippedGun();
//            m_gun.SetInfiniteAmmo(true);
//        }
//        else if (m_weaponController.GetEquippedMelee() != null)
//            m_melee = m_weaponController.GetEquippedMelee();

//        else
//            return;

//        m_health = m_maxHealth;
//    }

//    void Update()
//    {
//        if (m_isDead == false)
//        {
//            //To Do: put this in coroutine
//            Debug.Log("Current State: " + m_state);
//            DrawLinePath(m_agent.path);
//            m_distBetweenPlayer = Vector3.Distance(transform.position, m_player.transform.position);
//            m_gunDistToPlayer = Vector3.Distance(m_weaponController.GetEquippedWeapon().transform.position, m_player.transform.position);

//            if (m_health <= 0)
//            {
//                Die();
//                return;
//            }

//            if (CheckStates())
//                SwitchState();

//            if (CanAttack())
//                Attack();

//            m_stateMachine.Update();

//            UpdateAnims();
//            UpdateParticles();

//        }
//        else
//        {
//            if (m_hasDroppedTrigger)
//            {
//                DropDead();
//            }
//        }
//    }

//    //Check states and return true if there is a state change
//    bool CheckStates()
//    {
//        STATE prevState = m_state;

//        if (m_distBetweenPlayer < m_seekRadius)
//        {
//            transform.LookAt(PlayerPosition);

//            if (m_distBetweenPlayer > m_fleeRadius)
//                //If player is within seek and outside flee radius
//                m_state = STATE.SEEK;

//            else if (m_distBetweenPlayer <= m_fleeRadius + m_deadZone && m_distBetweenPlayer >= m_fleeRadius - m_deadZone)
//                //If player is within seek and we're at max flee distance (deadzone for floating point precision)
//                m_state = STATE.STATIONARY;

//            else if (m_distBetweenPlayer < m_fleeRadius)
//                //If player is within flee
//                m_state = STATE.FLEE;
//        }
//        else
//        {
//            m_state = STATE.WANDER;
//        }

//        if (m_gun.GetIsEmpty() || m_finishedReload == false)
//        {
//            m_state = STATE.RELOAD;
//        }

//        if (m_gunDistToPlayer < m_attackRadius && m_gun.GetIsEmpty() == false)
//        {
//            if (m_finishedReload)
//            {
//                if (CanAttack() == false)
//                {
//                    m_state = STATE.SEEK;
//                }
//            }
//        }

//        if (prevState != m_state)
//            return true;

//        else
//            return false;
//    }

//    void SwitchState()
//    {
//        switch (m_state)
//        {
//            case STATE.WANDER:
//                m_stateMachine.ChangeState(new Wander());
//                break;
//            case STATE.SEEK:
//                m_stateMachine.ChangeState(new Seek());
//                break;
//            case STATE.FLEE:
//                m_stateMachine.ChangeState(new Flee());
//                break;
//            case STATE.RELOAD:
//                m_stateMachine.ChangeState(new Reload());
//                break;
//            case STATE.STATIONARY:
//                m_stateMachine.ChangeState(new Stationary());
//                break;
//            default:
//                return;
//        }
//    }

//    //Check if there is nothing blocking the gun
//    bool CanAttack()
//    {
       
//        Vector3 vecBetween = (m_player.transform.position - m_weaponController.m_weaponHold.transform.position);
//        Vector3 rayPos1 = m_weaponController.m_weaponHold.position - (m_weaponController.m_weaponHold.forward * m_rayDetectBufferDist);
//        Vector3 rayPos2 = rayPos1 + m_weaponController.m_weaponHold.right * 0.1f;
//        rayPos1 -= m_weaponController.m_weaponHold.right * 0.1f;

//        Ray ray1 = new Ray(rayPos1, vecBetween);
//        Ray ray2 = new Ray(rayPos2, vecBetween);
//        RaycastHit hit;

//        Debug.DrawRay(rayPos1, vecBetween, Color.green);
//        Debug.DrawRay(rayPos2, vecBetween, Color.green);

//        if (Physics.Raycast(ray1, out hit, vecBetween.magnitude + m_rayDetectBufferDist, m_environmentLayer))
//            return false;
//        if (Physics.Raycast(ray2, out hit, vecBetween.magnitude + m_rayDetectBufferDist, m_environmentLayer))
//            return false;
        

//        return true;
//    }

//    void Attack()
//    {
//        if (m_gunDistToPlayer < m_attackRadius && m_finishedReload)
//        {
//            m_weaponController.m_weaponHold.LookAt(PlayerPosition);
//            if (Gun.Shoot())
//            {
//                m_enemyAnimator.SetTrigger("Shoot");
//            }
           
//        }
//    }

//    void Die()
//    {
//        m_agent.ResetPath();
//        m_walkingParticleSystem.Stop();
//        int randomAnim = Random.Range(0, deathAnimationCount);
//        m_enemyAnimator.SetInteger("WhichDeath", 2);
//        m_enemyAnimator.SetTrigger("Death");
//        RandomPitch();
//        m_audioSource.PlayOneShot(m_deathSounds[Random.Range(0, m_deathSounds.Count)]);
//        GetComponent<NavMeshAgent>().enabled = false;
//        //Rigidbody rb = gameObject.AddComponent<Rigidbody>();
//        //rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
//        m_isDead = true;
//    }

//    void DropDead()
//    {
//        if (m_hasDropped == false)
//        {
//            m_counter += Time.deltaTime;
//            Vector3 target = new Vector3(transform.position.x, m_bodyDropHeight, transform.position.z);

//            transform.position = Vector3.Lerp(transform.position, target, m_counter);

//            if (transform.position.y == m_bodyDropHeight)
//            {
//                m_hasDropped = true;
//            }
//        }
//    }

//    public void TakeHit(int a_damage, RaycastHit a_hit)
//    {
//        TakeDamage(a_damage);
//    }

//    public void TakeDamage(int a_damage)
//    {
//        m_health -= a_damage;
//    }

//    public void TakeImpact(int a_damage, RaycastHit a_hit, Projectile a_projectile)
//    {

//    }

//    private void UpdateAnims()
//    {
//        float myVelocity = m_agent.velocity.magnitude;
//        Vector3 localVel = transform.InverseTransformDirection(m_agent.velocity.normalized);

//        m_enemyAnimator.SetFloat("Velocity", myVelocity);

//        m_enemyAnimator.SetFloat("MovementDirectionRight", localVel.x);
//        m_enemyAnimator.SetFloat("MovementDirectionForward", localVel.z);
//    }

//    private void UpdateParticles()
//    {
//        if (m_agent.velocity != Vector3.zero)
//        {
//            if(m_walkingParticleSystem.isPlaying == false)
//            m_walkingParticleSystem.Play();
//        }
//        else
//        {
//            m_walkingParticleSystem.Stop();
//        }
//    }

//    private void RandomPitch()
//    {
//        m_audioSource.pitch = Random.Range(0.95f, 1.05f);
//    }

//    void DrawLinePath(NavMeshPath path)
//    {
//        if (path.corners.Length < 2) //if the path has 1 or no corners, there is no need
//            return;

//        m_line.positionCount = path.corners.Length; //set the array of positions to the amount of corners

//        for (int i = 0; i < path.corners.Length; i++)
//        {
//            m_line.SetPosition(i, path.corners[i]); //go through each corner and set that to the line renderer's position
//        }
//    }

//#if UNITY_EDITOR
//    private void OnDrawGizmos()
//    {
//        Gizmos.color = Color.blue;
//        Gizmos.DrawWireSphere(transform.position, m_seekRadius);
//        Gizmos.DrawWireSphere(transform.position, m_fleeRadius);
//        Gizmos.color = Color.red;
//        if (m_weaponController != null)
//        {
//            Gizmos.DrawWireSphere(m_weaponController.m_weaponHold.transform.position, m_attackRadius);
//            Gizmos.color = Color.green;
//            Gizmos.DrawWireSphere(m_weaponController.m_weaponHold.transform.position, m_coverRadius);
//        }
//    }
//#endif
//}














