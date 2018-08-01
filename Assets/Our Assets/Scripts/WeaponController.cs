using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Michael Corben
//Based on Tutorial:https://www.youtube.com/watch?v=rZAnnyensgs&list=PLFt_AvWsXl0ctd4dgE1F8g3uec4zKNRV0&index=3
//Created 24/07/2018
//Last edited 01/08/2018

public class WeaponController : MonoBehaviour {

    public Transform m_weaponHold;
    public Gun m_startingGun;
    private Gun m_equippedGun;
    public Melee m_startingMelee;
    private Melee m_equippedMelee;
    public LayerMask m_entityCollisionMask;
    public LayerMask m_terrainCollisionMask;

    public void Awake() {
        if (m_startingGun != null) {
            EquipGun(m_startingGun);
            m_equippedGun.SetEntityCollisionLayer(m_entityCollisionMask);
            m_equippedGun.SetEnvironmentCollisionLayer(m_terrainCollisionMask);
        }
        else if (m_startingMelee != null)
        {
            EquipMelee(m_startingMelee);
            m_equippedMelee.SetEntityCollisionLayer(m_entityCollisionMask);
            m_equippedMelee.SetEnvironmentCollisionLayer(m_terrainCollisionMask);
        }
    }
    public void EquipGun(Gun a_gunToEquip) {
        if (m_equippedGun != null) {
            Destroy(m_equippedGun.gameObject);
        }
        else if (m_equippedMelee != null) {
            Destroy(m_equippedMelee.gameObject);
        }
        m_equippedGun = Instantiate(a_gunToEquip, m_weaponHold) as Gun;
        m_equippedGun.transform.parent = m_weaponHold;
    }
    public void EquipMelee(Melee a_meleeToEquip)
    {
        if (m_equippedGun != null) {
            Destroy(m_equippedGun.gameObject);
        }
        else if (m_equippedMelee != null) {
            Destroy(m_equippedMelee.gameObject);
        }
        m_equippedMelee = Instantiate(a_meleeToEquip, m_weaponHold) as Melee;
        m_equippedMelee.transform.parent = m_weaponHold;
    }
    public void ReloadEquippedGun() {
        if (m_equippedGun != null) {
            m_equippedGun.Reload();
        }
    }
    public Gun GetEquippedGun() { return m_equippedGun; }
    public Melee GetEquippedMelee() { return m_equippedMelee; }
    public GameObject GetEquippedWeapon() {
        if (m_equippedGun != null) {
            return m_equippedGun.gameObject;
        }
        else if (m_equippedMelee != null) {
            return m_equippedMelee.gameObject;
        }
        else
            return null;
    }
    public bool GetIsAuto() { return m_equippedGun.m_isAutomatic; }
    public bool GetIsReloading() { return m_equippedGun.m_isReloading; }
    public bool GetIsGunEmpty() { return m_equippedGun.GetIsEmpty(); }
    public int GetCurrentClip() { return m_equippedGun.GetCurrentClip(); }
    public int GetCurrentAmmo() { return m_equippedGun.GetCurrentAmmo(); }
    public void Shoot() {
        if (m_equippedGun != null)
        {
            m_equippedGun.Shoot();
        }
    }
}
