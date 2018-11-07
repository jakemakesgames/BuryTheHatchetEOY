using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Michael Corben
//Based on Tutorial:https://www.youtube.com/watch?v=rZAnnyensgs&list=PLFt_AvWsXl0ctd4dgE1F8g3uec4zKNRV0&index=3
//Created 24/07/2018
//Last edited 07/11/2018

public class WeaponController : MonoBehaviour {

    //----------------------------
    #region Setup variables
    [Tooltip("The position the equipped weapon will be")]
    [SerializeField] public Transform m_weaponHold;

    [Header("Gun")]
    [Tooltip("If anything is here it will be equipped when the game starts")]
    [SerializeField] private Gun m_startingGun;
    [Range(1,6)] [SerializeField] private int m_startingAmmo;
    private Gun m_equippedGun;
    [Tooltip("If there is anything here and not in " +
        "the starting gun it will be equipped when the game starts")]
    [SerializeField] private Melee m_startingMelee;
    private Melee m_equippedMelee;

    [Header("LayerMasks")]
    [SerializeField] private LayerMask m_entityCollisionMask;
    [Tooltip("The layer on which object that can be destroyed are to be assigned")]
    [SerializeField] private LayerMask m_terrainCollisionMask;
    [Tooltip("The layer that objects bullets should ricochet off should be assigned")]
    [SerializeField] private LayerMask m_ricochetCollisionMask;
    [Tooltip("The layer on which object that can be destroyed are to be assigned " +
        "Currently only works for the melee weapon")]
    [SerializeField] private LayerMask m_destroyableCollisionMask;
    #endregion

    //----------------------------
    #region Properties
    public Transform WeaponHold {
        get { return m_weaponHold; } 
        set { m_weaponHold = value; }
    }

    public Gun EquippedGun {
        get { return m_equippedGun; }
        set { m_equippedGun = value; }
    }
    public Melee EquippedMelee {
        get { return m_equippedMelee; }
        set { m_equippedMelee = value; }
    }

    public LayerMask EntityCollisionMask {
        get { return m_entityCollisionMask; }
        set { m_entityCollisionMask = value; }
    }

    public float GunSpreadAngle {
        get { return m_equippedGun.SpreadAngle; }
        set { if (m_equippedGun != null)
                m_equippedGun.SpreadAngle = value; }
    }
    #endregion

    //----------------------------
    #region Getters
    //returns if there is an equipped melee
    public Melee GetEquippedMelee() { return EquippedMelee; }

    //Will return a game object of the currently equipped weapon
    //or null if there isn't any eqiuipped weapon
    public GameObject GetEquippedWeapon() {
        if (EquippedGun != null)
            return EquippedGun.gameObject;
        else if (m_equippedMelee != null)
            return m_equippedMelee.gameObject;
        else
            return null;
    }
    
    //returns if there is an equipped gun
    public Gun GetEquippedGun() { return EquippedGun; }
    #endregion

    //----------------------------
    #region Equippers
    //destroys any currently equipped weapon and equips a new gun
    public void EquipGun(Gun a_gunToEquip) {
        if (EquippedGun != null) {
            Destroy(EquippedGun.gameObject);
        }
        else if (EquippedMelee != null) {
            Destroy(EquippedMelee.gameObject);
        }
        EquippedGun = Instantiate(a_gunToEquip, WeaponHold) as Gun;
        EquippedGun.transform.parent = WeaponHold;
        EquippedGun.SetEntityCollisionLayer(EntityCollisionMask);
        EquippedGun.SetEnvironmentCollisionLayer(m_terrainCollisionMask);
        EquippedGun.SetRicochetCollisionLayer(m_ricochetCollisionMask);
    }

    //destroys any currently equipped weapon and equips a new melee weapon
    public void EquipMelee(Melee a_meleeToEquip)
    {
        if (EquippedGun != null) {
            Destroy(EquippedGun.gameObject);
        }
        else if (EquippedMelee != null) {
            Destroy(EquippedMelee.gameObject);
        }
        EquippedMelee = Instantiate(a_meleeToEquip, WeaponHold) as Melee;
        EquippedMelee.transform.parent = WeaponHold;
    }

    //destroys any currently equipped weapon and equips a new weapon
    public void EquipWeapon(GameObject a_weaponToEquip) {
        if (EquippedGun != null) {
            Destroy(EquippedGun.gameObject);
        }
        else if (EquippedMelee != null) {
            Destroy(EquippedMelee.gameObject);
        }
        if (a_weaponToEquip.GetComponent<Melee>() != null) {
            EquippedMelee = Instantiate(a_weaponToEquip.GetComponent<Melee>(), WeaponHold) as Melee;
            EquippedMelee.transform.parent = WeaponHold;
            EquippedMelee.SetEntityCollisionLayer(EntityCollisionMask);
            EquippedMelee.SetDestroyableCollisionLayer(m_destroyableCollisionMask);
        }
        else if (a_weaponToEquip.GetComponent<Gun>() != null) {
            EquippedGun = Instantiate(a_weaponToEquip.GetComponent<Gun>(), WeaponHold) as Gun;
            EquippedGun.transform.parent = WeaponHold;
            EquippedGun.SetEntityCollisionLayer(EntityCollisionMask);
            EquippedGun.SetEnvironmentCollisionLayer(m_terrainCollisionMask);
        }
    }
    #endregion


    //----------------------------
    #region Actions
    //Instantly reloads the equipped gun with no cool down
    public void InstantReload() {
        if (EquippedGun != null)
            EquippedGun.InstantReload();
    }


    //Reloads the equipped gun, returns false if the action failed
    public bool ReloadEquippedGun() {
        if (EquippedGun != null)
            return EquippedGun.ReloadOne();

        return false;
    }

    //Attacks with the equipped gun
    public bool Shoot() {
        if (EquippedGun != null) 
            return EquippedGun.Shoot();

        return false;
    }

    //attacks with the equipped melee
    public void Swing() {
        if (EquippedMelee != null)
            EquippedMelee.Swing();
    }
    #endregion
    
    //Equips a starting weapon
    public void Awake() {
        if (m_startingGun != null) {
            EquipGun(m_startingGun);
            EquippedGun.SetEntityCollisionLayer(EntityCollisionMask);
            EquippedGun.SetEnvironmentCollisionLayer(m_terrainCollisionMask);
            EquippedGun.SetRicochetCollisionLayer(m_ricochetCollisionMask);
            EquippedGun.SetCurrentClip(m_startingAmmo);
        }
        else if (m_startingMelee != null)
        {
            EquipMelee(m_startingMelee);
            EquippedMelee.SetEntityCollisionLayer(EntityCollisionMask);
            EquippedMelee.SetDestroyableCollisionLayer(m_destroyableCollisionMask);
        }
    }
}
