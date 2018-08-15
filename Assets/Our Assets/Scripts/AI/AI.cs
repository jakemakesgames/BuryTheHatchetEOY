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

    [Tooltip("Set this to the Environment layer for enivironment collision detection")]
    [SerializeField]
    LayerMask m_environmentLayer;

    float m_health;
    float m_distBetweenPlayer;
    float m_gunDistToPlayer;
    bool m_isDead;

    STATE m_state;

    private NavMeshAgent m_agent;
    LineRenderer m_line;
    private GameObject m_player;
    private List<Transform> m_coverPoints;

    private WeaponController m_weaponController;
    private Gun m_gun;
    private Melee m_melee;

    public delegate void Dead(AI enemy);
    public Dead OnDeath;

    public StateMachine<AI> m_stateMachine;

    public Vector3 PlayerPosition { get { return m_player.transform.position; } }

    public float CoverRadius { get { return m_coverRadius; } }

    public LayerMask CoverLayer { get { return m_coverLayer; } }

    public Gun Gun { get { return m_gun; } }

    public List<Transform> CoverPoints { get { return m_coverPoints; } }

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
    }

    public NavMeshAgent GetAgent() { return m_agent; }

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
            m_state = STATE.WANDER;

        if (m_gun.GetIsEmpty())
        {
            m_state = STATE.RELOAD;
        }

        if (m_gunDistToPlayer < m_attackRadius && !m_gun.GetIsEmpty())
        {
            if (CanAttack() == false)
                m_state = STATE.SEEK;
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
        RaycastHit hit;

        if (Physics.Raycast(m_weaponController.m_weaponHold.transform.position, vecBetween, out hit, 1000.0f, m_coverLayer))
            return false;
        if (Physics.Raycast(m_weaponController.m_weaponHold.transform.position, vecBetween, out hit, 1000.0f, m_environmentLayer))
            return false;

        return true;
    }

    void Attack()
    {
        if (m_gunDistToPlayer < m_attackRadius && !m_gun.GetIsEmpty())
            m_gun.Shoot();
    }

    void Die()
    {
        if (OnDeath != null)
            OnDeath(this);

        Destroy(gameObject);
        m_isDead = true;
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














