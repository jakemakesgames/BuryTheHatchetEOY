using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
//Michael Corben
//Based on Tutorial:https://www.youtube.com/watch?v=rZAnnyensgs&list=PLFt_AvWsXl0ctd4dgE1F8g3uec4zKNRV0&index=3
//Created 24/07/2018
//Last edited 31/10/2018

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour, IDamagable {


    //----------------------------
    #region Health Variables
    [Header("Health")]
    [SerializeField] private int m_maxHealth;

    [Tooltip("The coloured object that'll fill up the health bar")]
    [SerializeField] private Image m_healthBar;

    private int m_health;

    #endregion

    //----------------------------
    #region Particles Variables
    [Header("Particles")]
    [Tooltip("The particles that will play when ever the player gets hit")]
    [SerializeField] private ParticleSystem m_hitParticleSystem;

    [Tooltip("The particles that will play once the player has died")]
    [SerializeField] private ParticleSystem m_dieParticleSystem;

    #endregion

    //----------------------------
    #region Audio Variables
    [Header("Audio")]
    [Tooltip("The audio clip that will play whenever the player gets hit")]
    [SerializeField] private AudioClip m_hitSound;

    [Tooltip("The audio clip  that will play once the player has died")]
    [SerializeField] private AudioClip m_dieSound;

    private AudioSource m_audioSource;
    private SoundManager m_soundManager;
    #endregion

    //----------------------------
    #region Other Death Variables
    [Header("Other Death Stuff")]
    [Tooltip("The number of death animations")]
    [SerializeField] private int m_deathAnimCount;

    [SerializeField] private float m_deathFadeOutTime;

    [Tooltip("World height of body when dead")]
    [SerializeField] private float m_bodyDropHeight;

    private bool m_dead;
    private bool m_hasDropped = false;
    private bool m_hasDroppedTrigger = false;
    private float m_dropDeadCounter = 0;
    private float m_deathFadeOutTimer;
    private Vector3 m_respawnPoint;

    #endregion

    //----------------------------
    #region Member variables
    //private int m_heldWeaponLocation;
    //private List<WeaponInfo> m_heldWeaponsInfo = new List<WeaponInfo>();
    private PlayerInput m_input;
    private RespawnPoint m_rp;
    [HideInInspector] public CameraShake m_camAnimator;
    private Animator m_playerAnimator;
    private UIManager m_UIManager;
    #endregion

    //----------------------------
    #region Inspector Variables

    //[Tooltip("Button would you press to equip the starting weapon")]
    //[SerializeField] private int m_startingWeaponLocation = 2;

    //[Header("Needs to be the same number as held weapons")]
    //[Tooltip("which weapons assigned in the below " +
    //    "list are currently available to the player")]
    //public List<bool> m_weaponsAvailableToPlayer;

    //[Header("Needs to be filled with weapon prefabs")]
    //[Tooltip("All weapons the player will be able to wield " +
    //    "throughout the game and which position they'll be stored")]
    //public List<GameObject> m_heldWeapons;

    //[Tooltip("The text mesh object that'll display the heath")]
    //[SerializeField] private TextMeshProUGUI m_healthAmountTextMesh;

    #endregion

    //----------------------------
    #region Properties
    //public int HeldWeaponLocation {
    //    get { return m_heldWeaponLocation; }
    //    set { m_heldWeaponLocation = value; }
    //}

    public Vector3 RespawnPoint { get { return m_respawnPoint; }  set { m_respawnPoint = value; } }

    public bool HasDroppedTrigger {  get { return m_hasDroppedTrigger; } set { m_hasDroppedTrigger = value; } }

    public bool Dead { get { return m_dead; } set { m_dead = value;} }

    public RespawnPoint Rp { get { return m_rp; } set { m_rp = value; } }

    public float SFXVolume {
        get {
            if (m_soundManager == null)
                return 1f;
            else {
                return m_soundManager.MasterVolume * m_soundManager.SFXVolume;
            }
        }
    }
    #endregion

    //IDamageble interfaces methods for taking damage
    //----------------------------
    #region IDamagable methods
    public void TakeDamage(int a_damage) {
        if (m_camAnimator != null)
            m_camAnimator.PlayerHitShake();
        if (m_input.IsInvincible) {
            
            return;
        }
            
        m_health -= a_damage;
        if (m_health <= 0 && !Dead)
            Die();
        if(m_hitParticleSystem != null)
            m_hitParticleSystem.Play();
        if (m_hitSound != null)
            m_audioSource.PlayOneShot(m_hitSound, SFXVolume);
        UpdateHealthDisplay();
    }
    public void TakeHit(int a_damage, RaycastHit a_hit) {
        TakeDamage(a_damage);
    }
    public void TakeImpact(int a_damage, RaycastHit a_hit, Projectile a_projectile) {
        TakeDamage(a_damage);
        m_input.VelocityModifyer = a_projectile.transform.forward * a_projectile.KnockBack;
        if (m_input.IsInvincible) {
            a_projectile.TargetInvincible = true;
        }
    }
    #endregion

    //Methods for outside scripts to change the players health
    //----------------------------
    #region Health manipulation
    public void Heal(int a_healAmount) { m_health += a_healAmount;
        UpdateHealthDisplay(); }
    public int GetHealth() { return m_health; }
    public int GetMaxHealth() { return m_maxHealth; }
    #endregion

    //Returns information about the weapon which is about to be equipped
    //----------------------------
    #region To Equip Information
    //This is for storing information on the currently uneqipped weapons of the player
    //No longer used however as the player will only have one weapon and the melee will
    //Be specific to the player
    public struct WeaponInfo
        {
            public bool m_isMelee;
            public int m_curClip;
            public int m_curReserve;

            public WeaponInfo(bool a_isMelee, int a_curClip, int a_curReserve)
            {
                m_isMelee = a_isMelee;
                m_curClip = a_curClip;
                m_curReserve = a_curReserve;
            }
        }

    //Returns false if a successful assignment couldn't occur
    //Sets information for a weapon the player is unequipping
    //public bool AssignWeaponInfo(int a_listIterator, int a_clip, int a_reserveAmmo) {
    //    HeldWeaponLocation = a_listIterator;
    //    if (m_heldWeapons[a_listIterator].GetComponent<Gun>() != null) {
    //        if (m_heldWeapons[a_listIterator].GetComponent<Gun>().SetCurrentClip(a_clip) == false)
    //            return false;
    //        m_heldWeaponsInfo[a_listIterator] = new WeaponInfo(false, a_clip, a_reserveAmmo);
    //        return true;
    //    }
    //    else if (m_heldWeapons[a_listIterator].GetComponent<Melee>() != null) {
    //        m_heldWeaponsInfo[a_listIterator] = new WeaponInfo(true, 0, 0);
    //        return true;
    //    }
    //    else
    //        return false;
    //}
    //
    //public bool ToEquipIsMelee(int a_iterator) {
    //    return m_heldWeaponsInfo[a_iterator].m_isMelee;
    //}
    //public int ToEquipCurrentClip(int a_iterator) {
    //    return m_heldWeaponsInfo[a_iterator].m_curClip;
    //}
    //public int ToEquipCurrentReserve(int a_iterator) {
    //    return m_heldWeaponsInfo[a_iterator].m_curReserve;
    //}
    #endregion

    //Functionality to handle what will happen if the player dies
    //----------------------------
    #region Player death handling
    //Calls all subscribed OnDeath methods when the player dies
    //and tells the player it is dead allowing for other 
    //functionality to occur elsewhere
    private void Die() {
        Dead = true;

        //if the player animator exists, play a random death animation
        //and disable their navmesh agent to let the player fall with their mesh
        if (m_playerAnimator != null) {
            int randomAnim = Random.Range(0, m_deathAnimCount - 1);
            m_playerAnimator.SetInteger("WhichDeath", randomAnim);
            m_playerAnimator.SetTrigger("Death");
            GetComponent<NavMeshAgent>().enabled = false;
        }

        if (m_camAnimator != null)
            m_camAnimator.PlayerDeath();

        if (m_dieSound != null)
            m_audioSource.PlayOneShot(m_dieSound, SFXVolume);
        
    }

    //Moves the player to the respawn position.
    //Resets the player's health and give the player control over the player character again.
    //Resets/respawns all enemies in the current area.
    public void Respawn() {
        transform.position = RespawnPoint;
        m_deathFadeOutTimer = m_deathFadeOutTime;

        Dead = false;
        m_hasDropped = false;
        HasDroppedTrigger = false;

        m_health = m_maxHealth;
        UpdateHealthDisplay();

        if (Rp != null)
            Rp.ResetEnemies();

        if (m_playerAnimator != null)
            m_playerAnimator.SetTrigger("Respawn");

        if (m_dieParticleSystem != null)
        {
            m_dieParticleSystem.Stop();
            m_dieParticleSystem.Clear();
        }

        //if (m_camAnimator != null)
        //    m_camAnimator.PlayerRespawn();
        //else 
        if (m_UIManager != null)
            m_UIManager.RespawnFade();

        GetComponent<NavMeshAgent>().enabled = true;
        
        if (m_input != null) {
            if (m_input.WeapCont != null) {
                m_input.WeapCont.InstantReload();
                if (m_input.AmmoCont != null) {
                    for (int i = 0; i < m_input.WeapCont.EquippedGun.CurrentClip; i++) {
                        m_input.AmmoCont.Reload();
                    }
                }
            }
        }
    }

    //Drop the player to fall with the animation to appear as to fall
    //to the ground rather than just fall and float
    private void DropDead() {
        if (m_hasDropped == false) {
            m_dropDeadCounter += Time.deltaTime;

            Vector3 target = new Vector3(transform.position.x, m_bodyDropHeight, transform.position.z);

            transform.position = Vector3.Lerp(transform.position, target, m_dropDeadCounter);

            if (transform.position.y == m_bodyDropHeight) {
                if (m_dieParticleSystem != null)
                    m_dieParticleSystem.Play();
                m_hasDropped = true;
                if (m_UIManager != null)
                    m_UIManager.DeathFade();
            }
        }
    }
    #endregion

    //updates the health display
    //----------------------------
    private void UpdateHealthDisplay() {
        float health = m_health;
        float maxHealth = m_maxHealth;
        //if (m_healthAmountTextMesh != null)
        //    m_healthAmountTextMesh.text = m_health + " / " + m_maxHealth.ToString();
        if (m_healthBar != null)
            m_healthBar.fillAmount = (health / maxHealth);
    }

    //Sets up health, weapon information and respawn point
    //----------------------------
    private void Awake () {
        m_health = m_maxHealth;
        UpdateHealthDisplay();
        //m_heldWeaponsInfo.Capacity = m_heldWeapons.Count;
        RespawnPoint = transform.position;
        //for (int i = 0; i < m_heldWeapons.Count; i++) {
        //    if (m_heldWeapons[i].GetComponent<Gun>() != null) {
        //        int currentAmmo = m_heldWeapons[i].GetComponent<Gun>().GetCurrentAmmo();
        //        int currentClip = m_heldWeapons[i].GetComponent<Gun>().GetCurrentClip();
        //        m_heldWeaponsInfo.Add(new WeaponInfo(false, currentClip, currentAmmo));
        //    }
        //    else if (m_heldWeapons[i].GetComponent<Melee>() != null)
        //        m_heldWeaponsInfo.Add(new WeaponInfo(true, 0, 0));
        //}
        m_audioSource = GetComponent<AudioSource>();
        m_playerAnimator = GetComponentInChildren<Animator>();
        m_input = GetComponent<PlayerInput>();
        m_deathFadeOutTimer = m_deathFadeOutTime;
        //m_playerAnimator.SetInteger("whichWeapon", m_startingWeaponLocation);
        //HeldWeaponLocation = m_startingWeaponLocation;
    }

    //Set's up the animator for the camera
    //----------------------------
    private void Start() {
        m_camAnimator = Camera.main.GetComponent<CameraShake>();
        m_playerAnimator = m_input.m_playerAnimator;
        m_UIManager = FindObjectOfType<UIManager>();
        m_soundManager = FindObjectOfType<SoundManager>();
    }

    //Makes sure health never goes above maximum
    //and handles fade transitions on death and respawn
    //----------------------------
    private void Update() {
        if (m_health > m_maxHealth) {
            UpdateHealthDisplay();
            m_health = m_maxHealth;
        }
        else if (m_health <= 0) {
            m_health = 0;
            UpdateHealthDisplay();
        }
        if (Dead) {
            if (HasDroppedTrigger)
                DropDead();

            m_deathFadeOutTimer -= Time.deltaTime;

            if (m_deathFadeOutTimer <= 0)
                Respawn();
        }
    }
}
