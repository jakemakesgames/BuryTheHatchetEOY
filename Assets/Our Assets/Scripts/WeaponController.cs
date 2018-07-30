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
    private Gun m_equipedGun;
    public LayerMask m_entityCollisionMask;
    public LayerMask m_terrainCollisionMask;

    public void Awake() {
        if (m_startingGun != null) {
            EquipGun(m_startingGun);
            m_equipedGun.SetEntityCollisionLayer(m_entityCollisionMask);
            m_equipedGun.SetTerrainCollisionLayer(m_terrainCollisionMask);
        }
    }
    public void EquipGun(Gun gunToEquip) {
        if (m_equipedGun != null) {
            Destroy(m_equipedGun.gameObject);
        }
        m_equipedGun = Instantiate(gunToEquip, m_weaponHold) as Gun;
        m_equipedGun.transform.parent = m_weaponHold;
    }
    public void ReloadEquipedGun() {
        m_equipedGun.Reload();
    }
    public bool GetIsAuto() { return m_equipedGun.m_isAutomatic; }
    public bool GetIsReloading() { return m_equipedGun.m_isReloading; }
    public bool GetIsGunEmpty() { return m_equipedGun.GetIsEmpty(); }
    public void Shoot() {
        if (m_equipedGun != null)
        {
            m_equipedGun.Shoot();
        }
    }
}
