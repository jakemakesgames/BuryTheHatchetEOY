using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Mark Phillips
//Created 25/07/2018
//Last edited 25/07/2018
public class LootManager : MonoBehaviour
{
    // private Player player;

    private bool isDirty;

    private void Awake()
    {
        // player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        CaculateStats();

        Enemy[] enemies = FindObjectsOfType<Enemy>();

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].OnDeath += GenerateLoot;
        }
    }

    private void CaculateStats()
    {

    }

    void GenerateLoot(Enemy deadEnemy)
    {
        if (isDirty)
        {
            CaculateStats();
        }
    }
}