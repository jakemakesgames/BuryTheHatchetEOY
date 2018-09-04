using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(DestructibleObject))]
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
    [SerializeField] Transform m_phaseTwo;
    [SerializeField] Transform m_targetPlayer;
    [SerializeField] GameObject m_barrelOne;
    [SerializeField] GameObject m_barrelTwo;
    [SerializeField] GameObject m_railTrack;

    //Public Variables
    [SerializeField] float m_cooldownTime = 0;
    [SerializeField] float m_overheatValue = 0;
    [SerializeField] bool m_isOverheated = false;
    [SerializeField] int m_bossHealth = 0;
    [SerializeField] float m_bossSpeed = 0;
    [SerializeField] float m_cartSpeed;
    [SerializeField] int m_enragedHealth;

    #endregion

    #region Private Variables

    private float m_cooldownTimer;
    private float m_overheating;
    private bool m_enraged;

    #region Lerp attempt
    private float m_startTime;
    private float m_journeyLength;
    private float m_disTrav;
    private float m_journeyFrac;

    private bool m_reverseCart;
    #endregion

    private WeaponController m_weaponController;
    private NavMeshAgent m_bossAgent;
    private DestructibleObject m_barrels;
    private bool m_barrelsDestroyed = false;

    #endregion

    // Use this for initialization
    void Start ()
    {
        m_bossAgent = GetComponent<NavMeshAgent>();
        m_bossAgent.GetComponent<NavMeshAgent>().enabled = false;
        m_weaponController = GetComponent<WeaponController>();
        m_barrels = GetComponent<DestructibleObject>();

        #region
        ///Lerp Attempt
        //Gets the starting time
        m_startTime = Time.time;

        //Gets the distance between the two objects
        m_journeyLength = Vector3.Distance(m_leftSide.position, m_rightSide.position);
        #endregion

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

        //m_weaponController.GetEquippedGun().transform.LookAt(m_targetPlayer);

        //While the barrel hasnt been destroyed
        if (!m_barrelsDestroyed)
        {

            m_weaponController.GetEquippedGun().transform.LookAt(m_targetPlayer);

            #region Lerp
            ///Lerp Attempt
            //
            m_disTrav = (Time.time - m_startTime) * m_cartSpeed;
            m_journeyFrac = m_disTrav / m_journeyLength;

            if (m_reverseCart)
            {
                //Lerps from the right position to the left position
                m_boss.transform.position = Vector3.Lerp(m_rightSide.position, m_leftSide.position, m_journeyFrac);
            }
            else
            {
                //Lerps from the left position to right right position
                m_boss.transform.position = Vector3.Lerp(m_leftSide.position, m_rightSide.position, m_journeyFrac);
            }

            //Checks if the boss has reached one of the two objects
            if ((Vector3.Distance(m_boss.transform.position, m_rightSide.position) == 0.0f || 
                Vector3.Distance(m_boss.transform.position, m_leftSide.position) == 0.0f))
            {
                if (m_reverseCart)
                {
                    //Heads to the right side
                    m_reverseCart = false;
                }
                else
                {
                    //Heads to the left side
                    m_reverseCart = true;
                }

                m_startTime = Time.time;
            }
            #endregion

            if (m_barrelOne == false && m_barrelTwo == false)
            {
                m_barrelsDestroyed = true;
                m_bossAgent.GetComponent<NavMeshAgent>().enabled = true;
                m_railTrack.SetActive(false);
                m_minecart.SetActive(false);
                m_boss.transform.position = m_phaseTwo.position;
            }
        }
        else if (m_barrelsDestroyed)
        {

            //m_bossAgent.GetComponent<NavMeshAgent>().enabled = true;
            m_weaponController.GetEquippedGun().transform.LookAt(m_targetPlayer);
            m_bossAgent.SetDestination(m_targetPlayer.position);

            if (m_bossHealth <= m_enragedHealth)
            {
                m_enraged = true;
            }
        }
    }

    public void Overheat()
    {
        //Machine Gun overheating
        if (!m_isOverheated)
        {
            if (m_enraged == false)
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
            else if (m_enraged == true)
            {
                m_weaponController.Shoot();
            }
        }
        //Machien gun overheated
        else if (m_isOverheated)
        {
            if (m_enraged == false)
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
            else if (m_enraged == true)
            {
                m_isOverheated = false;
            }
        }
    }

    public void TakeHit(int a_damage, RaycastHit a_hit)
    {
        TakeDamage(a_damage);
    }

    public void TakeDamage(int a_damage)
    {
        m_bossHealth -= a_damage;

        if (m_bossHealth <= 0)
        {
            Death();
        }
    }

    public void TakeImpact(int a_damage, RaycastHit a_hit, Projectile a_projectile)
    {
        
    }

    #endregion

    private void Death()
    {
        Destroy(m_boss);
    }
    
}
