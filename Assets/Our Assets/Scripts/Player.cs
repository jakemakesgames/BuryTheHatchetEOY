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
    [SerializeField] private List<GameObject> m_heldWeapons;

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

    public void SetMoney(int a_money) { m_money = a_money; }
    public int GetMoney() { return m_money; }

    public int GetHealth() { return m_health; }

    public int GetMaxHealth() { return m_maxHealth; }



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
    }
}
