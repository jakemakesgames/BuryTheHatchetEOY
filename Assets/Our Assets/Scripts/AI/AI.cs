using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using StateMachine;

//Mark Phillips
//Created 13/08/2018
//Last edited 15/08/2018

[RequireComponent(typeof(WeaponController))]
[RequireComponent(typeof(NavMeshAgent))]
public class AI : MonoBehaviour, IDamagable
{
    enum STATE
    {
        WANDER,
        SEEK,
        FLEE,
        STATIONARY,
        RELOAD
    }


    [Tooltip("Max health value, currently at 5 (5 hits to die).")]
    [SerializeField]
    float m_maxHealth = 5;

    [Tooltip("Seek radius/Blue sphere (large) - distance to seek to player.")]
    [SerializeField]
    float m_seekRadius;

    [Tooltip("Flee radius/Blue sphere (small) - distance to flee from player.")]
    [SerializeField]
    float m_fleeRadius;

    [Tooltip("Cover radius/Green sphere - finds objects on the cover layer.")]
    [SerializeField]
    float m_coverRadius;


    [Tooltip("Accounts for floating point precision - when AI knows to switch to Stationary")]
    [SerializeField]
    float m_deadZone;

    float m_enemyRadius;

    [Tooltip("Attack radius/Red sphere - shoots at player.")]
    [SerializeField]
    float m_attackRadius;

    [Tooltip("The distance from enemy to cover point at which to stop calculating a new position.")]
    [SerializeField]
    float m_coverFoundThreshold = 3;

    [Tooltip("Set this to the Cover layer for cover collision detection")]
    [SerializeField]
    LayerMask m_coverLayer;

    [Tooltip("Set this to the Environment layer for environment collision detection")]
    [SerializeField]
    LayerMask m_environmentLayer;

    float m_health;
    float m_distBetweenPlayer;
    float m_gunDistToPlayer;
    [SerializeField] float m_rayDetectBufferDist;

    STATE m_state;

    private NavMeshAgent m_agent;
    LineRenderer m_line;
    private GameObject m_player;
    private List<Transform> m_coverPoints;
    public Animator enemyAnimator;

    private WeaponController m_weaponController;
    private Gun m_gun;
    private Melee m_melee;

    public delegate void Dead(AI enemy);
    public Dead OnDeath;

    public StateMachine<AI> m_stateMachine;

    //---------------------------------------------------------------------------
    //                              Gets / Sets           
    //---------------------------------------------------------------------------
    public NavMeshAgent Agent { get { return m_agent; } }

    public Vector3 PlayerPosition { get { return m_player.transform.position; } }

    public float CoverRadius { get { return m_coverRadius; } }

    public LayerMask CoverLayer { get { return m_coverLayer; } }

    public float CoverFoundThreshold { get { return m_coverFoundThreshold; } }

    public Gun Gun { get { return m_gun; } }

    public List<Transform> CoverPoints { get { return m_coverPoints; } }
    //---------------------------------------------------------------------------

    //---------------------------------------------------------------------------
    void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_coverPoints = new List<Transform>();
        m_weaponController = GetComponent<WeaponController>();
    }

    void Start()
    {
        m_line = GetComponent<LineRenderer>();
        m_agent = GetComponent<NavMeshAgent>();
        m_stateMachine = new StateMachine<AI>(this);
        m_stateMachine.ChangeState(new Wander());

        if (m_weaponController.GetEquippedGun() != null)
        {
            m_gun = m_weaponController.GetEquippedGun();
            m_gun.SetInfiniteAmmo(true);
        }
        else if (m_weaponController.GetEquippedMelee() != null)
            m_melee = m_weaponController.GetEquippedMelee();

        else
            return;

        m_health = m_maxHealth;
    }

    void Update()
    {
        //To Do: put this in coroutine
        Debug.Log("Current State: " + m_state);
        DrawLinePath(m_agent.path);
        m_distBetweenPlayer = Vector3.Distance(transform.position, m_player.transform.position);
        m_gunDistToPlayer = Vector3.Distance(m_weaponController.GetEquippedWeapon().transform.position, m_player.transform.position);

        if (m_health <= 0)
        {
            Die();
            return;
        }

        if (CheckStates())
            SwitchState();

        if (CanAttack())
            Attack();

        m_stateMachine.Update();

        UpdateAnims();
    }

    //Check states and return true if there is a state change
    bool CheckStates()
    {
        STATE prevState = m_state;

        if (m_distBetweenPlayer < m_seekRadius)
        {
            transform.LookAt(PlayerPosition);

            if (m_distBetweenPlayer > m_fleeRadius)
                //If player is within seek and outside flee radius
                m_state = STATE.SEEK;

            else if (m_distBetweenPlayer <= m_fleeRadius + m_deadZone && m_distBetweenPlayer >= m_fleeRadius - m_deadZone)
                //If player is within seek and we're at max flee distance (deadzone for floating point precision)
                m_state = STATE.STATIONARY;

            else if (m_distBetweenPlayer < m_fleeRadius)
                //If player is within flee
                m_state = STATE.FLEE;
        }
        else
        {
            m_state = STATE.WANDER;
        }

        if (m_gun.GetIsEmpty() || m_gun.m_isReloading)
        {
            m_state = STATE.RELOAD;
        }

        if (m_gunDistToPlayer < m_attackRadius && !m_gun.GetIsEmpty())
        {
            if (m_gun.m_isReloading == false)
            {
                if (CanAttack() == false)
                {
                    m_state = STATE.SEEK;
                }
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
            case STATE.WANDER:
                m_stateMachine.ChangeState(new Wander());
                break;
            case STATE.SEEK:
                m_stateMachine.ChangeState(new Seek());
                break;
            case STATE.FLEE:
                m_stateMachine.ChangeState(new Flee());
                break;
            case STATE.RELOAD:
                m_stateMachine.ChangeState(new Reload());
                break;
            case STATE.STATIONARY:
                m_stateMachine.ChangeState(new Stationary());
                break;
            default:
                return;
        }
    }

    //Check if there is nothing blocking the gun
    bool CanAttack()
    {
        Vector3 vecBetween = (m_player.transform.position - m_weaponController.m_weaponHold.transform.position);
        Vector3 rayPos1 = m_weaponController.m_weaponHold.position - (m_weaponController.m_weaponHold.forward * m_rayDetectBufferDist);
        Vector3 rayPos2 = rayPos1 + m_weaponController.m_weaponHold.right * 0.1f;
        rayPos1 -= m_weaponController.m_weaponHold.right * 0.1f;

        Ray ray1 = new Ray(rayPos1, vecBetween);
        Ray ray2 = new Ray(rayPos2, vecBetween);
        RaycastHit hit;

        Debug.DrawRay(rayPos1, vecBetween, Color.green);
        Debug.DrawRay(rayPos2, vecBetween, Color.green);

        if (Physics.Raycast(ray1, out hit, vecBetween.sqrMagnitude + m_rayDetectBufferDist, m_environmentLayer))
            return false;
        if (Physics.Raycast(ray2, out hit, vecBetween.sqrMagnitude + m_rayDetectBufferDist, m_environmentLayer))
            return false;


        return true;
    }

    void Attack()
    {
        if (m_gunDistToPlayer < m_attackRadius && !m_gun.GetIsEmpty())
        {
            m_weaponController.m_weaponHold.LookAt(PlayerPosition);
            m_gun.Shoot();
        }
    }

    void Die()
    {
        if (OnDeath != null)
            OnDeath(this);

        Destroy(gameObject);
    }
    public void TakeHit(int a_damage, RaycastHit a_hit)
    {
        TakeDamage(a_damage);
    }

    public void TakeDamage(int a_damage)
    {
        m_health -= a_damage;
    }

    public void TakeImpact(int a_damage, RaycastHit a_hit, Projectile a_projectile)
    {

    }

    private void UpdateAnims()
    {
        float myVelocity = m_agent.velocity.magnitude;
        Vector3 localVel = transform.InverseTransformDirection(m_agent.velocity);

        enemyAnimator.SetFloat("Velocity", myVelocity);

        enemyAnimator.SetFloat("MovementDirectionRight", localVel.x);
        enemyAnimator.SetFloat("MovementDirectionForward", localVel.z);
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
        Gizmos.DrawWireSphere(transform.position, m_seekRadius);
        Gizmos.DrawWireSphere(transform.position, m_fleeRadius);
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














