using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Mark Phillips
//Created 25/07/2018
//Last edited 25/07/2018
public class LootManager : MonoBehaviour
{
    private GameObject m_playerGO;
    private Player m_player;
    private Gun m_playerGun;
    private int m_remainingAmmo;
    private int m_remainingHealth;
    private GameObject m_pickup 
    [SerializeField] private GameObject m_healthPrefab;
    [SerializeField] private GameObject m_ammoPrefab;
    [SerializeField] private GameObject m_goldPrefab;
    [SerializeField] private int m_healthDropMin;
    [SerializeField] private int m_healthDropMax;
    [SerializeField] private int m_ammoDropMin;
    [SerializeField] private int m_ammoDropMax;
    [SerializeField] private int m_goldDropMin;
    [SerializeField] private int m_goldDropMax;

    private void Awake()
    {
        m_playerGO = GameObject.FindGameObjectWithTag("Player");
        m_player = m_playerGO.GetComponent<Player>();
        m_playerGun = m_playerGO.GetComponent<WeaponController>().GetEquippedGun();
        
        //Calculate initial stats
        CaculateStats();

        //Get all enemies and subscribe GenerateLoot to their OnDeath delegate
        Enemy[] enemies = FindObjectsOfType<Enemy>();

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].OnDeath += GenerateLoot;
        }
    }

    private void CaculateStats()
    {
        m_remainingHealth = m_player.GetMaxHealth() - m_player.GetHealth();
        m_remainingAmmo = m_playerGun.m_maxAmmo - m_playerGun.GetTotalAmmo();



    }

    void GenerateLoot(Enemy deadEnemy)
    {
        CaculateStats();

        //Instantiate loot;
    }
}