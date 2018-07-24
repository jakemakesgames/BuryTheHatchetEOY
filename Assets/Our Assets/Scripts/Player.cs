using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Michael Corben
//Based on Tutorial:https://www.youtube.com/watch?v=rZAnnyensgs&list=PLFt_AvWsXl0ctd4dgE1F8g3uec4zKNRV0&index=3
//Created 24/07/2018
//Last edited 25/07/2018

public class Player : MonoBehaviour, IDamagable {

    [SerializeField] private int m_maxHealth;
    private int m_health;
    private bool m_dead;
    private int m_money = 0;
    [SerializeField] private Transform m_crosshairPos;
    [SerializeField] private GameObject m_equippedWeapon;
    [SerializeField] private List<GameObject> m_heldWeapons;

    public void TakeHit(int a_damage, RaycastHit a_hit) {
        m_health -= a_damage;
        if (m_health <= 0 && !m_dead) {
            m_dead = true;
        }
    }

    private void OnDeath()
    {
        m_dead = true;
    }

	void Start () {
        m_health = m_maxHealth;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
