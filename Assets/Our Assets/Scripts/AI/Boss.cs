using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class Boss : MonoBehaviour, IDamagable
{
    #region Serialized Variables

    //Public Game Objects
    [SerializeField] GameObject m_boss;
    [SerializeField] GameObject m_minecart;
    [SerializeField] Transform m_leftSide;
    [SerializeField] Transform m_rightSide;

    //Public Variables
    [SerializeField] int m_cooldownTime = 0;
    [SerializeField] int m_overheating = 0;
    [SerializeField] int m_bossHealth = 0;

    #endregion

    #region Private Variables

    private NavMeshAgent m_bossAgent;
    private bool m_barrelsDestroyed = false;

    #endregion

    // Use this for initialization
    void Start ()
    {
        m_bossAgent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        Movement();
	}

    #region Public Functions

    public void Movement()
    {
        if (!m_barrelsDestroyed)
        {
            //Minecarts acceleration towards the edge of the Rail Track
            m_bossAgent.SetDestination(m_leftSide.position);

            //if (m_bossAgent.remainingDistance <= 0)
            //{
            //    m_bossAgent.SetDestination(m_rightSide.position);
            //}
        }
    }

    public void Overheat()
    {
        //Shoots until the gun overheats
        for (int i = 0; i < m_overheating; i++)
        {
            //Cooldown Time
            if (i == m_overheating)
            {
                m_cooldownTime -= i;
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
