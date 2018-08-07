using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Mark Phillips
//Created 25/07/2018
//Last edited 6/08/2018
public class LootManager : MonoBehaviour
{
    private GameObject m_playerGO;
    private Player m_player;
    private Gun m_playerGun;
    private float m_remainingAmmoPercent;
    private float m_missingHealthPercent;
    private int m_lootType;

    private bool wasDropped = false;

    [SerializeField] private GameObject m_healthPrefab;
    [SerializeField] private GameObject m_ammoPrefab;
    [SerializeField] private GameObject m_goldPrefab;

    [Tooltip("The percent of health at which health drop chance is increased.")]
    [SerializeField] private float m_healthThreshold;
    [Tooltip("Health drop chance in percent.")]
    [SerializeField] private float m_healthChance;


    [Tooltip("The percent of ammo at which health drop chance is increased.")]
    [SerializeField] private float m_ammoThreshold;
    [Tooltip("Ammo drop chance in percent.")]
    [SerializeField] private float m_ammoChance;

    [Tooltip("Radius around enemy to spawn loot.")]
    [SerializeField] float m_spawnRadius;




    private void Awake()
    {
        m_playerGO = GameObject.FindGameObjectWithTag("Player");
        m_player = m_playerGO.GetComponent<Player>();

        m_healthThreshold /= 100;
        m_healthChance /= 100;
        m_ammoThreshold /= 100;
        m_ammoChance /= 100;

        //Get all enemies and subscribe GenerateLoot to their OnDeath delegate
        Enemy[] enemies = FindObjectsOfType<Enemy>();

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].OnDeath += GenerateLoot;
        }
    }

    void Start()
    {
        m_playerGun = m_playerGO.GetComponent<WeaponController>().GetEquippedGun();
    }
    private void CalculateStats()
    {
        m_missingHealthPercent = m_player.GetHealth() / m_player.GetMaxHealth();
        m_remainingAmmoPercent = m_playerGun.GetTotalAmmo();
        m_remainingAmmoPercent /= m_playerGun.GetMaxAmmo();

        float lootChance;
        float healthChance;
        float ammoChance;

        if (m_missingHealthPercent <= m_healthThreshold)
        {
            healthChance = m_healthChance;
            wasDropped = true;
        }
        else
        {
            healthChance = 100.0f / 2.0f; // 100 / amount of items
            wasDropped = false;
        }

        if (m_remainingAmmoPercent <= m_ammoThreshold)
        {
            ammoChance = m_ammoChance;
            wasDropped = true;
        }
        else
        {
            ammoChance = 100.0f / 2.0f;
            wasDropped = false;
        }

        lootChance = Random.Range(0f, 1f);

        if (lootChance <= healthChance)
        {
            //health
            m_lootType = 0;
        }
        else if (lootChance <= ammoChance + healthChance)
        {
            //ammo
            m_lootType = 1;
        }

    }

    void GenerateLoot(Enemy deadEnemy)
    {
        CalculateStats();
        GameObject loot;

        Vector3 spawnPos = RandomCircle(deadEnemy.transform.position, m_spawnRadius);
        Quaternion randomRotationY = RandomRotationY(deadEnemy.transform.rotation);

        //Instantiate loot;
        if (m_lootType == 0)
        {
            loot = Instantiate(m_healthPrefab, spawnPos, randomRotationY);
        }
        if (m_lootType == 1)
        {
            loot = Instantiate(m_ammoPrefab, deadEnemy.transform.position, Quaternion.identity);
        }

        Vector3 newSpawnPos = RandomCircle(deadEnemy.transform.position, m_spawnRadius);
        while (newSpawnPos == spawnPos)
        {
            newSpawnPos = RandomCircle(deadEnemy.transform.position, 5.0f);
        }

        GameObject gold = Instantiate(m_goldPrefab, newSpawnPos, randomRotationY);
        gold.GetComponent<Gold>().SetGoldFlag(wasDropped);
    }

    Quaternion RandomRotationY(Quaternion a_rotation)
    {
        Quaternion randomRotationY = new Quaternion(a_rotation.x, Random.Range(0, 360), a_rotation.z, a_rotation.w);
        return randomRotationY;
    }

    Vector3 RandomCircle(Vector3 a_center, float a_radius)
    {
        float angle = Random.value * 360;
        Vector3 pos;
        pos.x = a_center.x + a_radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        pos.y = a_center.y ;
        pos.z = a_center.z + a_radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        return pos;

    }
}