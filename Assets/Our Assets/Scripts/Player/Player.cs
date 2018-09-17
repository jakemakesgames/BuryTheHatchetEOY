using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//Michael Corben
//Based on Tutorial:https://www.youtube.com/watch?v=rZAnnyensgs&list=PLFt_AvWsXl0ctd4dgE1F8g3uec4zKNRV0&index=3
//Created 24/07/2018
//Last edited 12/09/2018

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour, IDamagable {

    //This is for storing information on the currently uneqipped weapons of the player
    public struct WeaponInfo {
        public bool m_isMelee;
        public int m_curClip;
        public int m_curReserve;

        public WeaponInfo(bool a_isMelee, int a_curClip, int a_curReserve) {
            m_isMelee = a_isMelee;
            m_curClip = a_curClip;
            m_curReserve = a_curReserve;
        }
    }

    #region Member variables
        private bool m_dead;
        private int m_health;
        private int m_heldWeaponLocation;
        private float m_deathFadeOutTimer;
        private Vector3 m_respawnPoint;
        private List<WeaponInfo> m_heldWeaponsInfo = new List<WeaponInfo>();
        private AudioSource m_audioSource;
        private PlayerInput m_input;
    #endregion

    #region Inspector Variables
        [SerializeField] private int m_maxHealth;
        [SerializeField] private float m_deathFadeOutTime;
        [Tooltip("The audio clip that will play whenever the player gets hit")]
        [SerializeField] private AudioClip m_hitSound;
        [Tooltip("The audio clip  that will play once the player has died")]
        [SerializeField] private AudioClip m_dieSound;
        [Tooltip("The particles that will play when ever the player gets hit")]
        [SerializeField] private ParticleSystem m_hitParticleSystem;
        [Tooltip("The particles that will play once the player has died")]
        [SerializeField] private ParticleSystem m_dieParticleSystem;

        [Tooltip("Button would you press to equip the starting weapon")]
        [SerializeField] private int m_startingWeaponLocation = 2;

        [Header("Needs to be the same number as held weapons")]
        [Tooltip("which weapons assigned in the below " +
            "list are currently available to the player")]
        public List<bool> m_weaponsAvailableToPlayer;
        [Header("Needs to be filled with weapon prefabs")]
        [Tooltip("All weapons the player will be able to wield " +
            "throughout the game and which position they'll be stored")]
        public List<GameObject> m_heldWeapons;
        public Animator m_playerAnimator;

        public bool Dead {
            get { return m_dead; }
            set { m_dead = value; }
        }

    public event System.Action OnDeath;
    #endregion
    
    public int HeldWeaponLocation {
        get { return m_heldWeaponLocation; }
        set { m_heldWeaponLocation = value; }
    }
    //IDamageble interfaces methods for taking damage
    #region IDamagable methods
    public void TakeDamage(int a_damage) {
        if (m_input.IsInvincible)
            return;
        m_health -= a_damage;
        if (m_health <= 0 && !Dead)
            Die();
        if(m_hitParticleSystem != null)
            m_hitParticleSystem.Play();
        if (m_hitSound != null)
            m_audioSource.PlayOneShot(m_hitSound);
    }
    public void TakeHit(int a_damage, RaycastHit a_hit) {
        TakeDamage(a_damage);
    }
    public void TakeImpact(int a_damage, RaycastHit a_hit, Projectile a_projectile) {
        TakeDamage(a_damage);
    }
    #endregion

    #region Health manipulation
    public void Heal(int a_healAmount) { m_health += a_healAmount; }
    public int GetHealth() { return m_health; }
    public int GetMaxHealth() { return m_maxHealth; }
    #endregion

    //Returns false if a successful assignment couldn't occur
    //Sets information for a weapon the player is unequipping
    public bool AssignWeaponInfo(int a_listIterator, int a_clip, int a_reserveAmmo) {
        HeldWeaponLocation = a_listIterator;
        if (m_heldWeapons[a_listIterator].GetComponent<Gun>() != null) {
            if (m_heldWeapons[a_listIterator].GetComponent<Gun>().SetCurrentClip(a_clip) == false)
                return false;
            m_heldWeaponsInfo[a_listIterator] = new WeaponInfo(false, a_clip, a_reserveAmmo);
            return true;
        }
        else if (m_heldWeapons[a_listIterator].GetComponent<Melee>() != null) {
            m_heldWeaponsInfo[a_listIterator] = new WeaponInfo(true, 0, 0);
            return true;
        }
        else
            return false;
    }

    //Returns information about the weapon which is about to be equipped

    #region To Equip Information
        public bool ToEquipIsMelee(int a_iterator) {
            return m_heldWeaponsInfo[a_iterator].m_isMelee;
        }
        public int ToEquipCurrentClip(int a_iterator) {
            return m_heldWeaponsInfo[a_iterator].m_curClip;
        }
        public int ToEquipCurrentReserve(int a_iterator) {
            return m_heldWeaponsInfo[a_iterator].m_curReserve;
        }
    #endregion

    #region Player death handling
    //Calls all subscribed OnDeath methods when the player dies
    //and tells the player it is dead allowing for other 
    //functionality to occur elsewhere
    private void Die() {
        Dead = true;
        if (OnDeath != null)
            OnDeath();
        if (m_playerAnimator != null)
            m_playerAnimator.SetTrigger("Death");
        if (m_dieSound != null)
            m_audioSource.PlayOneShot(m_dieSound);
        //SceneManager.LoadScene(0);
    }

    //Moves the player to the respawn position
    public void Respawn() {
        transform.position = m_respawnPoint;
        m_deathFadeOutTimer = m_deathFadeOutTime;
        Dead = false;
        m_health = m_maxHealth;
    }
    #endregion

    //Sets up health, weapon information and respawn point
    private void Awake () {
        m_health = m_maxHealth;
        m_heldWeaponsInfo.Capacity = m_heldWeapons.Count;
        m_respawnPoint = transform.position;
        for (int i = 0; i < m_heldWeapons.Count; i++) {
            if (m_heldWeapons[i].GetComponent<Gun>() != null) {
                int currentAmmo = m_heldWeapons[i].GetComponent<Gun>().GetCurrentAmmo();
                int currentClip = m_heldWeapons[i].GetComponent<Gun>().GetCurrentClip();
                m_heldWeaponsInfo.Add(new WeaponInfo(false, currentClip, currentAmmo));
            }
            else if (m_heldWeapons[i].GetComponent<Melee>() != null)
                m_heldWeaponsInfo.Add(new WeaponInfo(true, 0, 0));
        }
        m_audioSource = GetComponent<AudioSource>();
        m_playerAnimator = GetComponentInChildren<Animator>();
        m_input = GetComponent<PlayerInput>();
        m_deathFadeOutTimer = m_deathFadeOutTime;
        m_playerAnimator.SetInteger("whichWeapon", m_startingWeaponLocation);
        HeldWeaponLocation = m_startingWeaponLocation;
    }
    
    //Makes sure health never goes above maximum
    //and handles fade transitions on death and respawn
    private void Update () {
        if (m_health > m_maxHealth)
            m_health = m_maxHealth;
        if(Dead) {
            m_deathFadeOutTimer -= Time.deltaTime;
            //fade the screen to black
            if (m_deathFadeOutTimer <= 0)
                Respawn();
        }
    }
}
