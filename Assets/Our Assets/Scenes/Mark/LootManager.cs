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
    private int m_lootType;
    // private Pickup m_pickup

    [SerializeField] private GameObject m_healthPrefab;
    [SerializeField] private GameObject m_ammoPrefab;
    [SerializeField] private GameObject m_goldPrefab;

    [Tooltip("The percent of health at which health drop chance is increased.")]
    [SerializeField] private int m_healthThreshold;
    [Tooltip("Health drop chance in percent.")]
    [SerializeField] private int m_healthChance;
    [Tooltip("Amount of health to drop.")]
    [SerializeField] private int m_healthAmount;

    [Tooltip("The percent of ammo at which health drop chance is increased.")]
    [SerializeField] private int m_ammoThreshold;
    [Tooltip("Ammo drop chance in percent.")]
    [SerializeField] private int m_ammoChance;
    [Tooltip("Amount of ammo to drop.")]
    [SerializeField] private int m_ammoAmount;

    [SerializeField] private int m_goldDropMin;
    [SerializeField] private int m_goldDropMax;

    private void Awake()
    {
        m_playerGO = GameObject.FindGameObjectWithTag("Player");
        m_player = m_playerGO.GetComponent<Player>();
        m_playerGun = m_playerGO.GetComponent<WeaponController>().GetEquippedGun();

        m_healthThreshold /= 100;
        m_healthChance /= 100;
        m_ammoThreshold /= 100;
        m_ammoChance /= 100;

        //Calculate initial stats
        CalculateStats();

        //Get all enemies and subscribe GenerateLoot to their OnDeath delegate
        Enemy[] enemies = FindObjectsOfType<Enemy>();

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].OnDeath += GenerateLoot;
        }
    }

    private void CalculateStats()
    {
        m_remainingHealth = m_player.GetMaxHealth() - m_player.GetHealth();
        m_remainingAmmo = m_playerGun.m_maxAmmo - m_playerGun.GetTotalAmmo();




        switch (m_lootType)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            
             
        }

    }

    void GenerateLoot(Enemy deadEnemy)
    {
        CalculateStats();

        //Instantiate loot;
    }
}