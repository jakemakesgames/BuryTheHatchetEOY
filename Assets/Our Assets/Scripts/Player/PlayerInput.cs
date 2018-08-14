using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
//Michael Corben
//Based on Tutorial:https://www.youtube.com/watch?v=rZAnnyensgs&list=PLFt_AvWsXl0ctd4dgE1F8g3uec4zKNRV0&index=3
//Created 24/07/2018
//Last edited 08/08/2018


[RequireComponent (typeof(NavMeshAgent))]
[RequireComponent(typeof(WeaponController))]
[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour {
    
    [SerializeField] private float m_speed = 10f;
    [SerializeField] private float m_dashTime = 10f;
    [SerializeField] private float m_dashSpeed = 1000f;

    private int m_equippedWeaponInumerator;
    private int m_ammoInClip;
    private int m_ammoInReserve;
    private float m_nmaSpeed;
    private float m_dashTimer = 0;
    private float m_nmaAngledSpeed;
    private float m_nmaAcceleration;
    private bool m_isHoldingGun;
    private bool m_isDashing = false;
    private NavMeshAgent m_nma;
    private Vector3 m_velocity;
	private Vector3 m_movementVector;
    private Camera m_viewCamera;
    private WeaponController m_weaponController;
    private Player m_player;

    [SerializeField] private Text m_clipAmmoDisplay;
    [SerializeField] private Text m_totalAmmoDisplay;
    [SerializeField] private GameObject m_crosshair;
    [SerializeField] private GameObject m_camera;

	//Charlie
	public Animator playerAnimator;

    //calls the equipped weapons attacking method (swing for melee or shoot for gun)
    //via the weapon controller script
    public void Attack() {
        Gun equippedGun = m_weaponController.GetEquippedGun();
        if (equippedGun == null) {
            Melee equippedMelee = m_weaponController.GetEquippedMelee();
            if (equippedMelee == null) {
                return;
            }
            m_weaponController.Swing();
        }
        else if (equippedGun.m_isAutomatic) {
            if (equippedGun.GetIsEmpty()) {
                if (Input.GetMouseButtonDown(0)) {
                    m_weaponController.ReloadEquippedGun();
                }
            }
            else if (Input.GetMouseButton(0)) {
                if (m_weaponController.Shoot())
				    playerAnimator.SetTrigger ("Shoot");
            }
        }
        else {
            if (Input.GetMouseButtonDown(0)) {
                if (equippedGun.GetIsEmpty()) {
                    m_weaponController.ReloadEquippedGun();
                }
                else {
                    if (m_weaponController.Shoot())
                        playerAnimator.SetTrigger("Shoot");
                }
            }
        }
    }

    //calculates a players velocity for the current frame
    private void Move()
    {
        m_movementVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
		Vector3 direction = m_camera.transform.rotation * m_movementVector;
        direction.y = 0;
        Vector3 moveVelocity = direction.normalized * m_speed;
        m_velocity = moveVelocity;
        if (!m_isDashing) {
            m_nma.velocity = m_velocity * Time.deltaTime;
        }
    }

    //forces the player to look at the mouse position on screen
    private void PlayerLookAt() {
        Ray ray = m_viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance)) {
            Transform hand = m_weaponController.m_weaponHold;
            Vector3 pointOnGround = ray.GetPoint(rayDistance);
            Vector3 heightCorrectedLookPoint = new Vector3(pointOnGround.x, transform.position.y, pointOnGround.z);
            transform.LookAt(heightCorrectedLookPoint);
            heightCorrectedLookPoint = new Vector3(pointOnGround.x, hand.position.y, pointOnGround.z);
            hand.LookAt(heightCorrectedLookPoint);
            m_weaponController.m_weaponHold = hand;
            if (m_crosshair != null)
            {
                m_crosshair.transform.position = heightCorrectedLookPoint;
            }
        }
    }

    //quickly moves the player in the direction they are facing
    private void Dash() {
        if (!m_isDashing) {
            if (Input.GetMouseButtonDown(1)) {
				playerAnimator.SetTrigger ("Roll");
                m_isDashing = true;
                m_dashTimer = Time.time + m_dashTime;
                m_nma.velocity = transform.forward * m_dashSpeed;
            }
        }
        else {
            if (m_dashTimer <= Time.time) {
                m_nma.speed = m_nmaSpeed;
                m_nma.angularSpeed = m_nmaAngledSpeed;
                m_nma.acceleration = m_nmaAcceleration;
                m_isDashing = false;
            }
        }
    }

    //gives a ui text object the players ammo for their currently equipped weapon
    private void DisplayAmmo() {
        Gun equippedGun = m_weaponController.GetEquippedGun();

        Debug.Log("Player ammo clip " + equippedGun.GetCurrentClip());
        Debug.Log("Player ammo reserve " + equippedGun.GetCurrentAmmo());
        if (equippedGun != null) {
            if (m_clipAmmoDisplay != null) {
                m_clipAmmoDisplay.GetComponent<Text>().text =
                    (equippedGun.GetCurrentClip().ToString() + " / " + equippedGun.m_clipSize.ToString());
            }

            if (m_totalAmmoDisplay != null) {
                m_totalAmmoDisplay.GetComponent<Text>().text =
                    (equippedGun.GetCurrentAmmo().ToString() + " / " + equippedGun.m_maxAmmo.ToString());
            }
        }
    }
    
    //caches the weapon info of the currently equipped weapon for storing on weapon switch
    private void SetWeaponInfo() {
        if (m_weaponController.GetEquippedGun() == null) {
            m_ammoInClip = 0;
            m_ammoInReserve = 0;
            m_isHoldingGun = false;
        }
        else {
            m_ammoInClip = m_weaponController.GetEquippedGun().GetCurrentClip();
            m_ammoInReserve = m_weaponController.GetEquippedGun().GetCurrentAmmo();
            m_isHoldingGun = true;
        }
    }

    //weapon switching
    private void SwitchWeapon() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            int weaponInumerator = 0;
            if (m_player.m_weaponsAvailableToPlayer[weaponInumerator]) {
                if (m_player.m_heldWeapons[weaponInumerator] != null) {
                    SetWeaponInfo();
                    if (m_weaponController.GetEquippedWeapon() != null) {
                        m_player.AssignWeaponInfo(m_equippedWeaponInumerator, m_ammoInClip, m_ammoInReserve);
                    }
                    m_equippedWeaponInumerator = weaponInumerator;
                    m_weaponController.EquipWeapon(m_player.m_heldWeapons[weaponInumerator]);
                    if (!m_player.ToEquipIsMelee(weaponInumerator)) {
                        m_weaponController.GetEquippedGun().SetCurrentClip(m_player.ToEquipCurrentClip(weaponInumerator));
                        m_weaponController.GetEquippedGun().SetCurrentReserveAmmo(m_player.ToEquipCurrentReserve(weaponInumerator));
                    }
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            int weaponInumerator = 1;
            if (m_player.m_weaponsAvailableToPlayer[weaponInumerator]) {
                if (m_player.m_heldWeapons[weaponInumerator] != null) {
                    SetWeaponInfo();
                    if (m_weaponController.GetEquippedWeapon() != null) {
                        m_player.AssignWeaponInfo(m_equippedWeaponInumerator, m_ammoInClip, m_ammoInReserve);
                    }
                    m_equippedWeaponInumerator = weaponInumerator;
                    m_weaponController.EquipWeapon(m_player.m_heldWeapons[weaponInumerator]);
                    if (!m_player.ToEquipIsMelee(weaponInumerator)) {
                        m_weaponController.GetEquippedGun().SetCurrentClip(m_player.ToEquipCurrentClip(weaponInumerator));
                        m_weaponController.GetEquippedGun().SetCurrentReserveAmmo(m_player.ToEquipCurrentReserve(weaponInumerator));
                    }
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            int weaponInumerator = 2;
            if (m_player.m_weaponsAvailableToPlayer[weaponInumerator]) {
                if (m_player.m_heldWeapons[weaponInumerator] != null) {
                    SetWeaponInfo();
                    if (m_weaponController.GetEquippedWeapon() != null) {
                        m_player.AssignWeaponInfo(m_equippedWeaponInumerator, m_ammoInClip, m_ammoInReserve);
                    }
                    m_equippedWeaponInumerator = weaponInumerator;
                    m_weaponController.EquipWeapon(m_player.m_heldWeapons[weaponInumerator]);
                    if (!m_player.ToEquipIsMelee(weaponInumerator)) {
                        m_weaponController.GetEquippedGun().SetCurrentClip(m_player.ToEquipCurrentClip(weaponInumerator));
                        m_weaponController.GetEquippedGun().SetCurrentReserveAmmo(m_player.ToEquipCurrentReserve(weaponInumerator));
                    }
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) {
            int weaponInumerator = 3;
            if (m_player.m_weaponsAvailableToPlayer[weaponInumerator]) {
                if (m_player.m_heldWeapons[weaponInumerator] != null) {
                    SetWeaponInfo();
                    if (m_weaponController.GetEquippedWeapon() != null) {
                        m_player.AssignWeaponInfo(m_equippedWeaponInumerator, m_ammoInClip, m_ammoInReserve);
                    }
                    m_equippedWeaponInumerator = weaponInumerator;
                    m_weaponController.EquipWeapon(m_player.m_heldWeapons[weaponInumerator]);
                    if (!m_player.ToEquipIsMelee(weaponInumerator)) {
                        m_weaponController.GetEquippedGun().SetCurrentClip(m_player.ToEquipCurrentClip(weaponInumerator));
                        m_weaponController.GetEquippedGun().SetCurrentReserveAmmo(m_player.ToEquipCurrentReserve(weaponInumerator));
                    }
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5)) {
            int weaponInumerator = 4;
            if (m_player.m_weaponsAvailableToPlayer[weaponInumerator]) {
                if (m_player.m_heldWeapons[weaponInumerator] != null) {
                    SetWeaponInfo();
                    if (m_weaponController.GetEquippedWeapon() != null) {
                        m_player.AssignWeaponInfo(m_equippedWeaponInumerator, m_ammoInClip, m_ammoInReserve);
                    }
                    m_equippedWeaponInumerator = weaponInumerator;
                    m_weaponController.EquipWeapon(m_player.m_heldWeapons[weaponInumerator]);
                    if (!m_player.ToEquipIsMelee(weaponInumerator)) {
                        m_weaponController.GetEquippedGun().SetCurrentClip(m_player.ToEquipCurrentClip(weaponInumerator));
                        m_weaponController.GetEquippedGun().SetCurrentReserveAmmo(m_player.ToEquipCurrentReserve(weaponInumerator));
                    }
                }
            }
        }
    }

    private void Awake() {
        m_nma = GetComponent<NavMeshAgent>();
        m_weaponController = GetComponent<WeaponController>();
        m_player = GetComponent<Player>();
        m_viewCamera = Camera.main;
        m_nmaAcceleration = m_nma.acceleration;
        m_nmaAngledSpeed = m_nma.angularSpeed;
        m_nmaSpeed = m_nma.speed;
    }

    private void Update () {
        //Switch Weapons
        SwitchWeapon();

        //Player looking at mouse
        PlayerLookAt();

        //Player attacking
        Attack();

        //Player dashing
        Dash();

        //Player movement
        Move();

        //Ammo display
        DisplayAmmo();

		//Charlie
		UpdateAnims ();
    }
    //Charlie
    private void UpdateAnims()
    {
        float myVelocity = m_velocity.magnitude;
        Vector3 localVel = transform.InverseTransformDirection(m_velocity);


        playerAnimator.SetFloat("Velocity", myVelocity);


        //
        playerAnimator.SetFloat("MovementDirectionRight", localVel.x * transform.right.x);
        playerAnimator.SetFloat("MovementDirectionForward", localVel.z * transform.forward.z);

        //ORIGINAL CODE//////////////////////////////////////////////////////////////////////////////
        //        playerAnimator.SetFloat ("MovementDirectionRight", m_velocity.x * transform.right.x);
        //        playerAnimator.SetFloat ("MovementDirectionForward", m_velocity.z * transform.forward.z);
        //////////////////////////////////////////////////////////////////////////////////////////////


        //        if (m_velocity.x * transform.right.x == 0){
        //            playerAnimator.SetFloat ("MovementDirectionForward", m_velocity.z * transform.forward.z);
        //        }


        //playerAnimator.SetFloat ("MovementDirectionForward", m_movementVector.x * transform.forward.x);

        //playerAnimator.SetFloat ("MovementDirectionRight", m_movementVector.z * transform.right.z);

    }
}
