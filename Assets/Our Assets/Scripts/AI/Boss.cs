using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(WeaponController))]
public class Boss : MonoBehaviour, IDamagable
{
    #region Serialized Variables

    //Public Game Objects
    [SerializeField] GameObject m_boss;
    [SerializeField] GameObject m_minecart;
    [SerializeField] GameObject m_bossGun;
    [SerializeField] Transform m_leftSide;
    [SerializeField] Transform m_rightSide;
    [SerializeField] Transform m_targetPlayer;

    //Public Variables
    [SerializeField] float m_cooldownTime = 0;
    [SerializeField] float m_overheatValue = 0;
    [SerializeField] bool m_isOverheated = false;
    [SerializeField] int m_bossHealth = 0;

    #endregion

    #region Private Variables

    private float m_cooldownTimer;
    private float m_overheating;
    private WeaponController m_weaponController;
    private NavMeshAgent m_bossAgent;
    private bool m_barrelsDestroyed = false;

    #endregion

    // Use this for initialization
    void Start ()
    {
        m_bossAgent = GetComponent<NavMeshAgent>();
        m_weaponController = GetComponent<WeaponController>();

        m_bossAgent.SetDestination(m_rightSide.position);
    }
	
	// Update is called once per frame
	void Update ()
    {
        Movement();
        Overheat();
	}

    #region Public Functions

    public void Movement()
    {
        //While the barrel hasnt been destroyed
        if (!m_barrelsDestroyed)
        {
            //Prevents the boss from rotating
            m_bossAgent.angularSpeed = 0;

            m_weaponController.GetEquippedGun().transform.LookAt(m_targetPlayer);

            if (m_bossAgent.transform.position.z >= m_rightSide.position.z)
            {
                m_bossAgent.SetDestination(m_leftSide.position);
            }
            else if (m_bossAgent.transform.position.z <= m_leftSide.position.z)
            {
                m_bossAgent.SetDestination(m_rightSide.position);
            }
        }
    }

    public void Overheat()
    {
        //Machine Gun overheating
        if (!m_isOverheated)
        {
            m_weaponController.Shoot();

            //Machine Gun overheated
            if (m_overheating >= m_overheatValue)
            {
                m_isOverheated = true;
                m_cooldownTimer = m_cooldownTime;
            }
            //Machine Gun overheating
            else
            {
                m_overheating += Time.deltaTime;
            }
        }
        //Machien gun overheated
        else if (m_isOverheated)
        {
            //Machine Gun cools down fully
            if (m_cooldownTimer <= 0)
            {
                m_isOverheated = false;
                m_overheating = 0;
            }
            //Timer on cooling down the Machine Gun
            else
            {
                m_cooldownTimer -= Time.deltaTime;
            }
        }
    }

    public void TakeHit(int a_damage, RaycastHit a_hit)
    {
        
    }

    public void TakeDamage(int a_damage)
    {
        m_bossHealth -= a_damage;
    }

    public void TakeImpact(int a_damage, RaycastHit a_hit, Projectile a_projectile)
    {
        
    }

    #endregion
}
