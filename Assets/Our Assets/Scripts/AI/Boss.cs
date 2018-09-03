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
    [SerializeField] Transform m_startPosition;
    [SerializeField] Transform m_targetPlayer;

    //Public Variables
    [SerializeField] float m_cooldownTime = 0;
    [SerializeField] float m_overheatValue = 0;
    [SerializeField] bool m_isOverheated = false;
    [SerializeField] int m_bossHealth = 0;
    [SerializeField] float m_bossSpeed = 0;
    [SerializeField] float m_cartSpeed;

    #endregion

    #region Private Variables

    private float m_cooldownTimer;
    private float m_overheating;

    private float m_startTime;
    private float m_currDuration;
    private float m_distToDest;
    private float m_journeyFrac;
    private float m_distToStartDest;
    private float m_startJourneyFrac;
    private bool m_toLeft = false;
    private bool m_toRight = false;
    private bool m_startMove = true;

    private WeaponController m_weaponController;
    private NavMeshAgent m_bossAgent;
    private bool m_barrelsDestroyed = false;

    #endregion

    // Use this for initialization
    void Start ()
    {
        m_bossAgent = GetComponent<NavMeshAgent>();
        m_bossAgent.GetComponent<NavMeshAgent>().enabled = false;
        m_weaponController = GetComponent<WeaponController>();

        //Gets the time we start
        m_startTime = Time.time;
        //Gets the distance between the two objects
        m_distToStartDest = Vector3.Distance(m_startPosition.position, m_leftSide.position);

        m_distToDest = Vector3.Distance(m_leftSide.position, m_rightSide.position);
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
            //m_bossAgent.GetComponent<NavMeshAgent>().enabled = false;

            //Prevents the boss from rotating
            //m_bossAgent.angularSpeed = 0;


            m_weaponController.GetEquippedGun().transform.LookAt(m_targetPlayer);

            
            m_currDuration = (m_startTime + Time.time) * m_cartSpeed;
            m_startJourneyFrac = m_currDuration / m_distToStartDest;
            m_journeyFrac = m_currDuration / m_distToDest;

            if (m_startMove)
            {
                m_boss.transform.position = Vector3.Lerp(m_startPosition.position, m_leftSide.position, m_startJourneyFrac);
                m_currDuration = 0;
                m_toLeft = true;
            }

            //travels from right side to left side based on time
            if (m_boss.transform.position.z <= m_leftSide.position.z && m_toLeft)
            {
                m_boss.transform.position = Vector3.Lerp(m_leftSide.position, m_rightSide.position, m_journeyFrac);
                m_startMove = false;
                m_toLeft = false;
                m_toRight = true;
            }
            else if (m_boss.transform.position.z == m_rightSide.position.z && m_toRight)
            {
                m_boss.transform.position = Vector3.Lerp(m_rightSide.position, m_leftSide.position, m_journeyFrac);
                m_toLeft = true;
                m_toRight = false;
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
