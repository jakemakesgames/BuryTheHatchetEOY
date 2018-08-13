using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using StateMachine;
//Mark Phillips
//Created 24/07/2018
//Last edited 13/08/2018

[RequireComponent(typeof(WeaponController))]
[RequireComponent(typeof(NavMeshAgent))]
public class AI : MonoBehaviour, IDamagable
{

    [Tooltip("Max health value, currently at 5 (5 hits to die).")]
    [SerializeField]
    float m_maxHealth = 5;

    [Tooltip("Seek radius/Blue sphere (large) - distance to seek to player.")]
    [SerializeField] float m_seekRadius;

    [Tooltip("Flee radius/Blue sphere (small) - distance to flee from player.")]
    [SerializeField] float m_fleeRadius;

    [Tooltip("Cover radius/Green sphere - finds objects on the cover layer.")]
    [SerializeField] private float m_coverRadius;

    float m_enemyRadius;

    [Tooltip("Attack radius/Red sphere - shoots at player.")]
    [SerializeField] float m_attackRadius;

    [Tooltip("The distance from enemy to cover point at which to stop calculating a new position.")]
    [SerializeField] float m_coverFoundThreshold = 3;

    [Tooltip("Set this to the Cover layer for cover collision detection")]
    [SerializeField] LayerMask m_coverLayer;

    [Tooltip("Set this to the Environment layer for enivironment collision detection")]
    [SerializeField] LayerMask m_environmentLayer;

    float m_health;
    float m_distBetweenPlayer;
    float m_gunDistToPlayer;

    private NavMeshAgent agent;
    LineRenderer m_line;
    private GameObject m_player;
    private List<Transform> m_coverPoints;

    private WeaponController m_weaponController;
    private Gun m_gun;
    private Melee m_melee;

    public delegate void Dead(Enemy enemy);
    public Dead OnDeath;

    public StateMachine<AI> m_stateMachine { get; set; }

    public Vector3 GetPlayerPos() { return m_player.transform.position; }

    void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_coverPoints = new List<Transform>();
        m_weaponController = GetComponent<WeaponController>();
    }

    void Start ()
    {
        m_line = GetComponent<LineRenderer>();
        agent = GetComponent<NavMeshAgent>();
        m_stateMachine = new StateMachine<AI>(this);
        m_stateMachine.ChangeState(new Wander());

        if (m_weaponController.GetEquippedGun() != null)
        {
            m_gun = m_weaponController.GetEquippedGun();
            m_gun.SetInfiniteAmmo(true);
        }
        else if (m_weaponController.GetEquippedMelee() != null)
        {
            m_melee = m_weaponController.GetEquippedMelee();
        }
        else
        {
            return;
        }

        m_health = m_maxHealth;
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        DrawLinePath(agent.path);
        m_distBetweenPlayer = Vector3.Distance(transform.position, m_player.transform.position);
        m_gunDistToPlayer = Vector3.Distance(m_weaponController.GetEquippedWeapon().transform.position, m_player.transform.position);

        if (m_distBetweenPlayer < m_seekRadius)
        {
            if (m_distBetweenPlayer > m_fleeRadius)
            {
                //If player is within seek and outside flee radius
                m_stateMachine.ChangeState(new Seek());
            }
            else if (DestinationReached())
            {
                //If player is within seek and we've reached the destination
                m_stateMachine.ChangeState(new Stationary());
            }
        }
        if (m_distBetweenPlayer < m_fleeRadius)
        {
            m_stateMachine.ChangeState(new Flee());
        }
        m_stateMachine.Update();
	}

    public void SetDestination(Vector3 a_target)
    {
        agent.destination = a_target;
    }

    public bool DestinationReached()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public Vector3 GetVelocity() { return agent.velocity; }

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
