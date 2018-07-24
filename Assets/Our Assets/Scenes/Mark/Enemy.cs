using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour {
    enum STATE
    {
        WANDER,
        SEEK,
        FLEE,
        STATIONARY,
        COVER
    }
    private float m_health;
    [SerializeField] private float m_speed;
    [SerializeField] private float m_maxSpeed;
    [SerializeField] private float m_seekDist;
    [SerializeField] private float m_fleeDist;
    [SerializeField] private float m_coverDist;
    private float m_enemyRadius;
    [SerializeField] private float m_fireDist;
    private float m_distBetweenPlayer;
    private float m_gunDistToPlayer;
    private Vector3 m_velocity;
    private bool m_isDesperate;
    private bool m_isReloading;
    private GameObject m_weapon;
    //private Gun m_gun;
    //private Melee m_melee;
    private GameObject m_player;
    [SerializeField] private List<Transform> m_wanderPoints;
    delegate void OnDeath(Enemy a_enemy);
    OnDeath died;
    STATE m_state;
    void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_weapon = transform.GetChild(0).gameObject;

        //if(m_weapon.tag == "Gun")
        //{
        //    m_gun = m_weapon.GetComponent<Gun>();
        //}
        //else if (m_weapon.tag == "Melee")
        //{
        //    m_melee = m_weapon.GetComponent<Melee>();
        //}
        //else
        //{
        //    return;
        //}
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        m_distBetweenPlayer = Vector3.Distance(transform.position, m_player.transform.position);
        m_gunDistToPlayer = Vector3.Distance(m_weapon.transform.position, m_player.transform.position);

        if (m_distBetweenPlayer < m_seekDist)
        {
            if (m_distBetweenPlayer > m_fleeDist)
            {
                m_state = STATE.SEEK;
            }
            else if (m_distBetweenPlayer <= m_fleeDist + 0.1 && m_distBetweenPlayer >= m_fleeDist - 0.1)
            {
                m_state = STATE.STATIONARY;
            }
            else if (m_distBetweenPlayer < m_fleeDist)
            {
                m_state = STATE.FLEE;
            }
        }

        if (m_gunDistToPlayer < m_fireDist)
        {
            Attack();
        }

        if (m_health <= 0)
        {
            if (died != null)
            {
                died(this);
            }
        }

        switch (m_state)
        {
            case STATE.WANDER:
                Wander();
                break;
            case STATE.SEEK:
                Seek();
                break;
            case STATE.FLEE:
                Flee();
                break;
            case STATE.STATIONARY:
                return;
            case STATE.COVER:
                FindCover();
                break;
        }
    }

    void Wander()
    {

    }

    void Seek()
    {
        transform.LookAt(m_player.transform.position);
        transform.position = Vector3.MoveTowards(transform.position, m_player.transform.position, m_speed * Time.deltaTime);
    }

    void Flee()
    {
        transform.LookAt(m_player.transform.position);
        transform.position = Vector3.MoveTowards(transform.position, m_player.transform.position, -m_speed * Time.deltaTime);
    }

    void Attack()
    {
        Debug.Log("Attack");
    }

    void FindCover()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
       //if (other.tag == "Enemy" && state )
       //{
       //    //Find new wander pos
       //}
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, m_seekDist);
        Gizmos.DrawWireSphere(transform.position, m_fleeDist);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_weapon.transform.position, m_fireDist);
    }

}
