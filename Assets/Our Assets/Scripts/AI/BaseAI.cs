using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using StateMachine;

[RequireComponent(typeof(WeaponController))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
public class BaseAI : MonoBehaviour, IDamagable
{
    #region Inspector Variables
    [Tooltip("Max health value, currently at 5 (5 hits to die).")]
    [SerializeField]
    protected float m_maxHealth = 5;

    [Tooltip("Accounts for floating point precision - when AI knows to switch to Stationary")]
    [SerializeField]
    protected float m_deadZone;
     
    [Tooltip("World height of body when dead")]
    [SerializeField]
    protected float m_bodyDropHeight;

    [Tooltip("Duration in seconds for stun when hit taken")]
    [SerializeField]
    protected float m_stunDuration;

    [Header("Sounds")]
    [SerializeField]
    protected List<AudioClip> m_deathSounds;

    [Header("Animation")]
    [SerializeField]
    private Animator m_enemyAnimator;
    [Tooltip("The number of death animations")]
    [SerializeField]
    protected int m_deathAnimationCount;

    [Header("Particles")]
    [Tooltip("Particles that will play when the enemy is walking")]
    [SerializeField]
    protected ParticleSystem m_walkingParticleSystem;
    [Tooltip("Particles that will play when the enemy dies")]
    [SerializeField]
    protected ParticleSystem m_bloodPoolParticleSystem;
    #endregion

    protected float m_health;
    protected float m_distBetweenPlayer;
    protected float m_counter = 0;
    protected float m_takeHitTimer;
    protected bool m_hasTakenImpact = false;
    protected bool m_isDead = false;
    protected bool m_hasDropped = false;
    protected bool m_hasDroppedTrigger = false;
    protected Vector3 m_respawnPoint;
    protected NavMeshAgent m_agent;
    protected AudioSource m_audioSource;
    protected GameObject m_player;
    protected WeaponController m_weaponController;

    public delegate void Dead(AI enemy);
    public Dead OnDeath;

    //private StateMachine<AI> m_stateMachine;

    #region Properties 
    public NavMeshAgent Agent { get { return m_agent; } }
    public Vector3 PlayerPosition { get { return m_player.transform.position; } }
    public bool HasDroppedTrigger { set { m_hasDroppedTrigger = value; } }
    public Animator EnemyAnimator {   get { return m_enemyAnimator; } set { m_enemyAnimator = value; } }
    #endregion

    protected virtual void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_weaponController = GetComponent<WeaponController>();
    }

    protected virtual void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_audioSource = GetComponent<AudioSource>();
        EnemyAnimator = GetComponentInChildren<Animator>();
        m_health = m_maxHealth;
        m_respawnPoint = transform.position;
    }

    protected virtual void Update()
    {
        if (m_isDead == false)
        {
            m_distBetweenPlayer = Vector3.Distance(transform.position, m_player.transform.position);

            if (m_health <= 0)
            {
                Die();
                return;
            }

            CheckIfHitTaken();

            UpdateAnims();
            UpdateParticles();
        }
        else
        {
            if (m_hasDroppedTrigger)
            {
                DropDead();
            }
        }
    }
    protected virtual void Attack()
    {

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
        TakeHit(a_damage, a_hit);
        m_agent.velocity += a_projectile.transform.forward * a_projectile.KnockBack;
        m_hasTakenImpact = true;
    }

    private void CheckIfHitTaken()
    {
        if (m_hasTakenImpact)
        {
            m_agent.isStopped = true;
            m_takeHitTimer = Time.time + m_stunDuration;
            m_hasTakenImpact = false;
        }
        if (m_takeHitTimer < Time.time)
        {
            m_agent.isStopped = false;
        }
    }

    protected virtual void Die()
    {
        m_agent.ResetPath();
        m_walkingParticleSystem.Stop();
        int randomAnim = Random.Range(0, m_deathAnimationCount);
        EnemyAnimator.SetInteger("WhichDeath", randomAnim);
        EnemyAnimator.SetTrigger("Death");
        EnemyAnimator.ResetTrigger("Reloading");
        EnemyAnimator.ResetTrigger("Shoot");
        RandomPitch();
        if (m_deathSounds.Count != 0)
        {
            m_audioSource.PlayOneShot(m_deathSounds[Random.Range(0, m_deathSounds.Count)]);
        }
        GetComponent<NavMeshAgent>().enabled = false;
        m_isDead = true;
    }

    private void DropDead()
    {
        if (m_hasDropped == false)
        {
            m_counter += Time.deltaTime;
            Vector3 target = new Vector3(transform.position.x, m_bodyDropHeight, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, target, m_counter);

            if (transform.position.y == m_bodyDropHeight)
            {
                m_hasDropped = true;
                m_bloodPoolParticleSystem.Play();
            }
        }
    }

    public void Respawn()
    {
        if(m_isDead)
        {
            if (m_enemyAnimator != null)
                m_enemyAnimator.SetTrigger("Respawn");
        }
        transform.position = m_respawnPoint;
        m_weaponController.InstantReload();
        m_isDead = false;
        m_hasDropped = false;
        HasDroppedTrigger = false;
        m_health = m_maxHealth;
        GetComponent<NavMeshAgent>().enabled = true;
    }

    private void UpdateAnims()
    {
        float myVelocity = m_agent.velocity.magnitude;
        Vector3 localVel = transform.InverseTransformDirection(m_agent.velocity.normalized);

        EnemyAnimator.SetFloat("Velocity", myVelocity);

        EnemyAnimator.SetFloat("MovementDirectionRight", localVel.x);
        EnemyAnimator.SetFloat("MovementDirectionForward", localVel.z);
    }

    private void UpdateParticles()
    {
        if (m_agent.velocity != Vector3.zero)
        {
            if (m_walkingParticleSystem.isPlaying == false)
                m_walkingParticleSystem.Play();
        }
        else
        {
            m_walkingParticleSystem.Stop();
            m_walkingParticleSystem.Clear();
        }

        if (m_isDead)
        {
            m_bloodPoolParticleSystem.Play();
        }
        else if(m_bloodPoolParticleSystem.isPlaying)
        {
            m_bloodPoolParticleSystem.Stop();
            m_bloodPoolParticleSystem.Clear();
        }
    }

    private void RandomPitch()
    {
        m_audioSource.pitch = Random.Range(0.95f, 1.05f);
    }
}
