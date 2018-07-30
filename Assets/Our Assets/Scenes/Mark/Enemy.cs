using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//Mark Phillips
//Created 24/07/2018
//Last edited 29/07/2018


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour, IDamagable {
    enum STATE
    {
        WANDER,
        SEEK,
        FLEE,
        STATIONARY,
        COVER
    }
    private float m_health;
    [SerializeField] float m_maxHealth = 5;
    [SerializeField] private float m_speed;
    [SerializeField] private float m_maxSpeed;
    [SerializeField] private float m_seekDist;
    [SerializeField] private float m_fleeDist;
    [SerializeField] private float m_coverDist;
    private float m_enemyRadius;
    [SerializeField] private float m_fireDist;
    private float m_distBetweenPlayer;
    private float m_gunDistToPlayer;
    [SerializeField] private float seekSpeed;
    private bool m_isDesperate;
    private bool m_isReloading;
    private GameObject m_weapon;
    private Gun m_gun;
    private NavMeshAgent agent;
    LineRenderer line;
    //private Melee m_melee;
    private GameObject m_player;
    private List<Transform> m_coverPoints;
    public delegate void Dead(Enemy enemy);
    public Dead OnDeath;
    STATE m_state;
    void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_coverPoints = new List<Transform>();
    }

    // Use this for initialization
    void Start()
    {
        m_weapon = transform.GetChild(0).GetChild(0).gameObject;
        line = GetComponent<LineRenderer>();
        agent = GetComponent<NavMeshAgent>();

        if (m_weapon.tag == "Gun")
       {
           m_gun = m_weapon.GetComponent<Gun>();
       }
       else if (m_weapon.tag == "Melee")
       {
          // m_melee = m_weapon.GetComponent<Melee>();
       }
       else
       {
           return;
       }
   
        m_health = m_maxHealth;
        agent = GetComponent<NavMeshAgent>();

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
                agent.destination = transform.position;

            }
            else if (m_distBetweenPlayer < m_fleeDist)
            {
                m_state = STATE.FLEE;
            }
        }
        else
        {
            m_state = STATE.WANDER;
        }

        if (m_gunDistToPlayer < m_fireDist && m_isReloading == false)
        {
            Attack();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (m_isReloading)
            {
                m_isReloading = false;
            }
            else
            {
                m_isReloading = true;
            }
        }

        if (m_gun.GetIsEmpty())
        {
            m_state = STATE.COVER;
        }
        else if (m_state == STATE.COVER && !m_gun.GetIsEmpty())
        {
           // m_state = STATE.WANDER;
        }
        Debug.Log(m_state);

        if (m_health <= 0)
        {
            if (OnDeath != null)
            {
                OnDeath(this);
            }
            Destroy(gameObject);
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
            case STATE.COVER:
                FindCover();
                break;
            default:
                return;
        }
    }

    void Wander()
    {
        agent.destination = transform.position;
    }
    void Seek()
    {
        transform.LookAt(m_player.transform.position);
        //*!

        /*!
         * 
         * 
         * !*/
        
        //transform.position = Vector3.MoveTowards(transform.position, m_player.transform.position, m_speed * Time.deltaTime);
        agent.destination = m_player.transform.position;
    }

    void Flee()
    {
        transform.LookAt(m_player.transform.position);
        // transform.position = Vector3.MoveTowards(transform.position, m_player.transform.position, -m_speed * Time.deltaTime);
        Vector3 vecBetween = (m_player.transform.position - transform.position);


       agent.destination = transform.position - vecBetween;
    }

    void Attack()
    {
        m_gun.Shoot();
    }
    public void TakeHit(int a_damage, RaycastHit a_hit)
    {
        TakeDamage(a_damage);
    }

    public void TakeDamage(int a_damage)
    {
        m_health -= a_damage;
    }
    public void FindCover()
    {
        int layerMask = 1 << 10; //Layer 10
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_coverDist, layerMask);

        if (hitColliders.Length == 0)
        {
            m_gun.Reload();
        }
        for (int i = 0; i < hitColliders.Length; i++)
        {
            m_coverPoints.Add(hitColliders[i].transform);
        }

        //transform.position = Vector3.MoveTowards(transform.position, FindNearestCover(), m_speed * Time.deltaTime);
        Vector3 targetLocation = FindNearestCover();
        agent.destination = targetLocation;
        DrawLinePath(agent.path);

        if ((transform.position - targetLocation).magnitude < 0.1f)
        { 
            m_gun.Reload();
        }
    }
    Vector3 FindNearestCover()
    {
        Transform nearestPoint = m_coverPoints[0];
        float nearestDistance = float.MaxValue;
        float distance;

        foreach (Transform coverPoint in m_coverPoints)
        {
            //distance = Vector3.Distance(transform.position, coverPoint.position);
            distance = (transform.position - coverPoint.position).sqrMagnitude;

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestPoint = coverPoint;
            }
        }

        Vector3 dirFromPlayer = nearestPoint.position;
        dirFromPlayer = (( nearestPoint.position - m_player.transform.position).normalized);



        Vector3 finalPoint = (nearestPoint.position + dirFromPlayer);
        finalPoint = new Vector3(finalPoint.x, nearestPoint.position.y, finalPoint.z);

        //Vector3 coverFromCentre = Vector3.zero;
       // finalPoint = cover

        return finalPoint;
    }

    void DrawLinePath(NavMeshPath path)
    {
        if (path.corners.Length < 2) //if the path has 1 or no corners, there is no need
            return;

        line.positionCount = path.corners.Length; //set the array of positions to the amount of corners

        for (int i = 0; i < path.corners.Length; i++)
        {
            line.SetPosition(i, path.corners[i]); //go through each corner and set that to the line renderer's position
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, m_seekDist);
        Gizmos.DrawWireSphere(transform.position, m_fleeDist);
        Gizmos.color = Color.red;
        if (m_weapon != null)
        {
            Gizmos.DrawWireSphere(m_weapon.transform.position, m_fireDist);
        }


        if (m_isReloading == true)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(m_weapon.transform.position, m_coverDist);
        }
    }
#endif
}
