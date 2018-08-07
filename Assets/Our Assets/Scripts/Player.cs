using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Michael Corben
//Based on Tutorial:https://www.youtube.com/watch?v=rZAnnyensgs&list=PLFt_AvWsXl0ctd4dgE1F8g3uec4zKNRV0&index=3
//Created 24/07/2018
//Last edited 1/08/2018

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour, IDamagable {

    private bool m_dead;
    private int m_health;
    private int m_money = 0;
    [SerializeField] private int m_maxHealth;
    [SerializeField] private int m_startingMoney;
    [SerializeField] private Transform m_crosshairPos;
    [SerializeField] private GameObject m_equippedWeapon;

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
        if (m_heldWeapons[a_listIterator].GetComponent<Melee>() != null) {
            return true;
        }
        else if (m_heldWeapons[a_listIterator].GetComponent<Gun>() != null) {
            if (!m_heldWeapons[a_listIterator].GetComponent<Gun>().SetCurrentClip(a_clip)) {
                return false;
            }
            if (!m_heldWeapons[a_listIterator].GetComponent<Gun>().SetCurrentReserveAmmo(a_reserveAmmo)) {
                return false;
            }
        return true;
        }
        else {
            return false;
        }
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
    }
    
    private void Update () {
        if (m_health > m_maxHealth) {
            m_health = m_maxHealth;
        }
        Debug.Log("Player health " + m_health);
    }
}
