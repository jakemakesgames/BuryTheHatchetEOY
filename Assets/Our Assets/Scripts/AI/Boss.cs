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
    [SerializeField] Transform m_bossGunPos;
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
    [SerializeField] float m_gunRotateSpeed;
    [SerializeField] float m_bossRotateSpeed;
    [SerializeField] int m_enragedHealth;
    [SerializeField] float m_distanceToBoss;
    [SerializeField] float m_bossFightDist;

    #endregion

    #region Private Variables

    private float m_cooldownTimer;
    private float m_overheating;
    private bool m_enraged;
    private bool m_barrelsDestroyed = false;
    private float m_distance;
    private bool m_startFight;

    #region Lerp attempt
    private float m_startTime;
    private float m_journeyLength;
    private float m_disTrav;
    private float m_journeyFrac;
    private bool m_movingLeft = true;
    #endregion

    private WeaponController m_weaponController;
    private NavMeshAgent m_bossAgent;
    private DestructibleObject m_barrels;
    private Quaternion m_rotation;
    private Vector3 m_targetDir;

    #endregion

    // Use this for initialization
    void Start()
    {
        m_bossAgent = GetComponent<NavMeshAgent>();
        m_bossAgent.GetComponent<NavMeshAgent>().enabled = false;
        m_weaponController = GetComponent<WeaponController>();
        m_barrels = GetComponent<DestructibleObject>();

        #region
        ///Lerp Attempt

        m_boss.transform.position = m_rightSide.transform.position;

        //Gets the distance between the two objects
        m_journeyLength = Vector3.Distance(m_leftSide.position, m_rightSide.position);
        #endregion

    }

    // Update is called once per frame
    void Update()
    {
        StartBossFight();

        if (m_startFight)
        {
            Movement();
            Overheat();
        }
    }

    #region Public Functions

    public void Movement()
    {
        m_weaponController.GetEquippedGun().transform.position = m_bossGunPos.transform.position;

        //While the barrel hasnt been destroyed
        if (!m_barrelsDestroyed)
        {
            m_targetDir = m_targetPlayer.position - m_weaponController.GetEquippedGun().transform.position;

            m_rotation = Quaternion.LookRotation(m_targetDir);
            m_weaponController.GetEquippedGun().transform.rotation = Quaternion.Lerp(m_weaponController.GetEquippedGun().transform.rotation,
                m_rotation, m_gunRotateSpeed * Time.deltaTime);

            #region Lerp
            ///Lerp Attempt
            //
            m_disTrav = (Time.time - m_startTime) * m_cartSpeed;
            m_journeyFrac = m_disTrav / m_journeyLength;


            //m_startPos = transform.position;


            Vector3 targetPos = m_movingLeft ? m_leftSide.position : m_rightSide.position;
            Vector3 fromPos = m_movingLeft ? m_rightSide.position : m_leftSide.position;

            m_boss.transform.position = Vector3.Lerp(fromPos, targetPos, m_journeyFrac);


            if (Vector3.Distance(m_boss.transform.position, targetPos) < 0.01f)
            {
                m_movingLeft = !m_movingLeft;
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
            #region Boss Rotation

            m_targetDir = m_targetPlayer.position - m_boss.transform.position;

            m_rotation = Quaternion.LookRotation(m_targetDir);
            m_boss.transform.rotation = Quaternion.Lerp(m_boss.transform.rotation, m_rotation, m_bossRotateSpeed * Time.deltaTime);

            #endregion

            #region Gun Rotation

            //Gets the direction
            m_targetDir = m_targetPlayer.position - m_weaponController.GetEquippedGun().transform.position;
            m_rotation = Quaternion.LookRotation(m_targetDir);
            //Lerps between the guns current rotation to the target rotation
            m_weaponController.GetEquippedGun().transform.rotation = Quaternion.Lerp(m_weaponController.GetEquippedGun().transform.rotation,
                m_rotation, m_gunRotateSpeed * Time.deltaTime);

            #endregion

            m_distance = Vector3.Distance(m_boss.transform.position, m_targetPlayer.position);

            if (m_distance > m_distanceToBoss)
            {
                m_bossAgent.isStopped = false;
                m_bossAgent.SetDestination(m_targetPlayer.position);
            }
            else
            {
                m_bossAgent.isStopped = true;
            }

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
        Destroy(m_bossGun);
    }

    private void StartBossFight()
    {
        if (m_startFight == false)
        {
            if ((m_boss.transform.position - m_targetPlayer.position).magnitude <= m_bossFightDist)
            {
                m_startFight = true;

                //Gets the starting time
                m_startTime = Time.time;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_targetPlayer.position, m_distanceToBoss);

        Gizmos.DrawWireSphere(m_boss.transform.position, m_bossFightDist);
    }
}
