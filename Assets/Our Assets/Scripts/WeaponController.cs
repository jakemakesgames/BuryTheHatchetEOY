using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Michael Corben
//Based on Tutorial:https://www.youtube.com/watch?v=rZAnnyensgs&list=PLFt_AvWsXl0ctd4dgE1F8g3uec4zKNRV0&index=3
//Created 24/07/2018
//Last edited 30/07/2018

public class WeaponController : MonoBehaviour {

    public Transform m_weaponHold;
    public Gun m_startingGun;
    private Gun m_equippedGun;
    public LayerMask m_entityCollisionMask;
    public LayerMask m_terrainCollisionMask;

    public void Awake() {
        if (m_startingGun != null) {
            EquipGun(m_startingGun);
            m_equippedGun.SetEntityCollisionLayer(m_entityCollisionMask);
            m_equippedGun.SetTerrainCollisionLayer(m_entityCollisionMask);
        }
    }
    public void EquipGun(Gun gunToEquip) {
        if (m_equippedGun != null) {
            Destroy(m_equippedGun.gameObject);
        }
        m_equippedGun = Instantiate(gunToEquip, m_weaponHold) as Gun;
        m_equippedGun.transform.parent = m_weaponHold;
    }
    public void ReloadEquipedGun() {
        m_equippedGun.Reload();
    }
    public bool GetIsReloading() { return m_equippedGun.m_isReloading; }
    public bool GetIsGunEmpty() { return m_equippedGun.GetIsEmpty(); }
    public void Shoot() {
        if (m_equippedGun != null)
        {
            m_equippedGun.Shoot();
        }
    }
}
