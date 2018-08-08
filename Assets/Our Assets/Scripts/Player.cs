using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Michael Corben
//Based on Tutorial:https://www.youtube.com/watch?v=rZAnnyensgs&list=PLFt_AvWsXl0ctd4dgE1F8g3uec4zKNRV0&index=3
//Created 24/07/2018
//Last edited 08/08/2018

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour, IDamagable {
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

    private bool m_dead;
    private int m_health;
    private int m_money = 0;
    private List<WeaponInfo> m_heldWeaponsInfo = new List<WeaponInfo>();
    [SerializeField] private int m_maxHealth;
    [SerializeField] private int m_startingMoney;
    [Header("Needs to be the same number as held weapons")]
    public List<bool> m_weaponsAvailableToPlayer;
    [Header("Needs to be filled with weapon prefabs")]
    public List<GameObject> m_heldWeapons;

    public event System.Action OnDeath;

    public void TakeHit(int a_damage, RaycastHit a_hit) {
        TakeDamage(a_damage);
    }
    public void TakeDamage(int a_damage) {
        m_health -= a_damage;
        if (m_health <= 0 && !m_dead) {
            Die();
        }
    }

    public void Heal(int a_healAmount) { m_health += a_healAmount; }
    public void AddMoney(int a_money) { m_money += a_money; }
    public void SetMoney(int a_money) { m_money = a_money; }
    public int GetMoney() { return m_money; }

    public int GetHealth() { return m_health; }

    public int GetMaxHealth() { return m_maxHealth; }

    //returns false if a successful assignment could
    public bool AssignWeaponInfo(int a_listIterator, int a_clip, int a_reserveAmmo) {

        if (m_heldWeapons[a_listIterator].GetComponent<Gun>() != null) {
            if (!m_heldWeapons[a_listIterator].GetComponent<Gun>().SetCurrentClip(a_clip)) {
                return false;
            }
            m_heldWeaponsInfo[a_listIterator] = new WeaponInfo(false, a_clip, a_reserveAmmo);
            return true;
        }
        else if (m_heldWeapons[a_listIterator].GetComponent<Melee>() != null) {
            m_heldWeaponsInfo[a_listIterator] = new WeaponInfo(true, 0, 0);
            return true;
        }
        else {
            return false;
        }
    }

    public bool ToEquipIsMelee(int a_iterator) {
        return m_heldWeaponsInfo[a_iterator].m_isMelee;
    }
    public int ToEquipCurrentClip(int a_iterator) {
        return m_heldWeaponsInfo[a_iterator].m_curClip;
    }
    public int ToEquipCurrentReserve(int a_iterator) {
        return m_heldWeaponsInfo[a_iterator].m_curReserve;
    }

    private void Die() {
        m_dead = true;
        if (OnDeath != null) {
            OnDeath();
        }
    }

    private void Awake () {
        m_health = m_maxHealth;
        m_money = m_startingMoney;
        m_heldWeaponsInfo.Capacity = m_heldWeapons.Count;
        for (int i = 0; i < m_heldWeapons.Count; i++) {
            if (m_heldWeapons[i].GetComponent<Gun>() != null) {
                int currentAmmo = m_heldWeapons[i].GetComponent<Gun>().GetCurrentAmmo();
                int currentClip = m_heldWeapons[i].GetComponent<Gun>().GetCurrentClip();
                m_heldWeaponsInfo[i] = new WeaponInfo(false, currentClip, currentAmmo);
            }
            else if (m_heldWeapons[i].GetComponent<Melee>() != null) {
                m_heldWeaponsInfo[i] = new WeaponInfo(true, 0, 0);
            }
        }
    }
    
    private void Update () {
        if (m_health > m_maxHealth) {
            m_health = m_maxHealth;
        }
        Debug.Log("Player health " + m_health);
        Debug.Log("Player money " + m_money);
    }
}
