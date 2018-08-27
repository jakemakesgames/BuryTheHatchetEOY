using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
//Michael Corben
//Based on Tutorial:https://www.youtube.com/watch?v=rZAnnyensgs&list=PLFt_AvWsXl0ctd4dgE1F8g3uec4zKNRV0&index=3
//Created 24/07/2018
//Last edited 25/08/2018


[RequireComponent (typeof(NavMeshAgent))]
[RequireComponent(typeof(WeaponController))]
[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour {

    #region Movement variables
        [Header("Movement variables")]
        [Tooltip("Movement speed of the player")]
        [SerializeField] private float m_speed = 10f;
        [Tooltip("Time spent in the roll")]
        [SerializeField] private float m_dashTime = 10f;
        [Tooltip("Movement speed of the player during the roll")]
        [SerializeField] private float m_dashSpeed = 1000f;
    #endregion

    #region private member variables
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
    #endregion

    #region In world game objects
        [Header("In world game objects")]
        [Tooltip("The camera used to orient the player when the camera rotates, required for movement to work")]
        [SerializeField] private Camera m_camera;
        [Tooltip("This object will be where ever the player is looking")]
        [SerializeField] private GameObject m_crosshair;
        [Tooltip("Will change the text of this object to the amount " +
            "of ammo left in the currently equipped gun's clip")]
        [SerializeField] private Text m_clipAmmoDisplay;
        [Tooltip("will change the text of this object to the amount of ammo" +
            "the player has left excluding the ammo in the clip")]
        [SerializeField] private Text m_totalAmmoDisplay;
    #endregion

    #region sounds and particles
    [Header("Sounds")]
        [Tooltip("One of the sounds that'll player when the player moves")]
        [SerializeField] private AudioClip m_clothesRustleSound;
        [Tooltip("The sound that'll play when the player is walking")]
        [SerializeField] private AudioClip m_walkingSound;
        [Tooltip("The sound that'll play when the player is rolling")]
        [SerializeField] private AudioClip m_rollSound;
        [Header("Particles")]
        [Tooltip("Paricles that will play when the player is walking")]
        [SerializeField] private ParticleSystem m_walkingParticleSystem;
        [Tooltip("Particles that will play when the player rolls")]
        [SerializeField] private ParticleSystem m_rollParticleSystem;
    #endregion    

    public Animator m_playerAnimator;

    //calls the equipped weapons attacking method 
    //(swing for melee or shoot for gun)
    //via the weapon controller script
    //and also checks if the player wishes to reload
    public void Attack() {
        Gun equippedGun = m_weaponController.GetEquippedGun();
        if (equippedGun == null) {
            Melee equippedMelee = m_weaponController.GetEquippedMelee();
            if (equippedMelee == null)
                return;
            m_weaponController.Swing();
            m_playerAnimator.SetTrigger("Swing");
        }
        else if (equippedGun.m_isAutomatic && equippedGun.IsIdle) {
            if (Input.GetMouseButton(0)) {
                if (m_weaponController.Shoot())
                    m_playerAnimator.SetTrigger("Shoot");
            }
            else if (Input.GetKeyDown(KeyCode.R)) {
                m_weaponController.ReloadEquippedGun();
                m_playerAnimator.SetTrigger("Reload");
            }
        }
        else if (equippedGun.IsIdle) {
            if (Input.GetMouseButtonDown(0)) {
                if (m_weaponController.Shoot())
                    m_playerAnimator.SetTrigger("Shoot");
            }
            else if (Input.GetKeyDown(KeyCode.R) && m_weaponController.GetEquippedGun().IsFull == false) {
                m_weaponController.ReloadEquippedGun();
                m_playerAnimator.SetTrigger("Reload");
            }
        }
    }
    
    //Calculates the players velocity for the next frame
    private void Move()
    {
        m_movementVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
		Vector3 direction = m_camera.transform.rotation * m_movementVector;
        direction.y = 0;
        Vector3 moveVelocity = direction.normalized * m_speed;
        m_velocity = moveVelocity;
        if (!m_isDashing)
            m_nma.velocity = m_velocity * Time.deltaTime;
    }

    //Forces the player to look at the mouse position on screen as well as place a crosshair object where the player is looking
    private void PlayerLookAt() {
        Ray ray = m_viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;
        if (groundPlane.Raycast(ray, out rayDistance)) {
            Transform hand = m_weaponController.WeaponHold;
            Vector3 pointOnGround = ray.GetPoint(rayDistance);
            Vector3 heightCorrectedLookPoint = new Vector3(pointOnGround.x, transform.position.y, pointOnGround.z);
            transform.LookAt(heightCorrectedLookPoint);
            heightCorrectedLookPoint = new Vector3(pointOnGround.x, hand.position.y, pointOnGround.z);
            hand.LookAt(heightCorrectedLookPoint);
            m_weaponController.WeaponHold = hand;
            if (m_crosshair != null)
                m_crosshair.transform.position = heightCorrectedLookPoint;
        }
    }

    //Quickly moves the player in the direction they are facing
    private void Dash() {
        if (!m_isDashing) {
            if (Input.GetMouseButtonDown(1)) {
				m_playerAnimator.SetTrigger ("Roll");
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

    //Gives a ui text object the players ammo for their currently equipped weapon
    private void DisplayAmmo() {
        Gun equippedGun = m_weaponController.GetEquippedGun();
        if (equippedGun != null) {
            if (m_clipAmmoDisplay != null)
                m_clipAmmoDisplay.GetComponent<Text>().text =
                    (equippedGun.GetCurrentClip().ToString() + " / " + equippedGun.m_clipSize.ToString());

            if (m_totalAmmoDisplay != null)
                m_totalAmmoDisplay.GetComponent<Text>().text =
                    (equippedGun.GetCurrentAmmo().ToString() + " / " + equippedGun.m_maxAmmo.ToString());
        }
    }
    
    //Caches the weapon info of the currently equipped weapon for storing on weapon switch
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

    //Checks if the player has access the weapon that the player atempemted to equip
    //if they do the method then stores the values of the currently equipped weapon
    //then equipes the player with the new weapon with the stored values for that specific weapon
    private void ChangeWeapon(int a_inumerator) {
        if (m_player.m_weaponsAvailableToPlayer[a_inumerator]) {
            if (m_player.m_heldWeapons[a_inumerator] != null) {
                SetWeaponInfo();
                if (m_weaponController.GetEquippedWeapon() != null)
                    m_player.AssignWeaponInfo(m_equippedWeaponInumerator, m_ammoInClip, m_ammoInReserve);

                m_equippedWeaponInumerator = a_inumerator;
                m_weaponController.EquipWeapon(m_player.m_heldWeapons[a_inumerator]);
                if (!m_player.ToEquipIsMelee(a_inumerator)) {
                    m_weaponController.GetEquippedGun().SetCurrentClip(m_player.ToEquipCurrentClip(a_inumerator));
                    m_weaponController.GetEquippedGun().SetCurrentReserveAmmo(m_player.ToEquipCurrentReserve(a_inumerator));
                }
            }
        }
    }

    //Equip the weapon stored in the location
    //based on which number the player presses 
    private void SwitchWeapon() {
        Gun equippedGun = m_weaponController.GetEquippedGun();
        bool canSwitch = true;
        if (equippedGun == null) {
            canSwitch = m_weaponController.GetEquippedMelee().IsIdle;
        }
        else {
            canSwitch = equippedGun.IsIdle;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            int weaponInumerator = 0;
            ChangeWeapon(weaponInumerator);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            int weaponInumerator = 1;
            ChangeWeapon(weaponInumerator);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            int weaponInumerator = 2;
            ChangeWeapon(weaponInumerator);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) {
            int weaponInumerator = 3;
            ChangeWeapon(weaponInumerator);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5)) {
            int weaponInumerator = 4;
            ChangeWeapon(weaponInumerator);
        }
    }

    //To be called by the animator after the melee weapon swing animation has finished playing
    public void ResetMelee() {
        m_weaponController.GetEquippedMelee().EndSwing();
    }
    
    //Get all requied attached components and store them for later use
    private void Awake() {
        m_nma = GetComponent<NavMeshAgent>();
        m_weaponController = GetComponent<WeaponController>();
        m_player = GetComponent<Player>();
        m_playerAnimator = GetComponentInChildren<Animator>();
        m_viewCamera = m_camera;
        m_nmaAcceleration = m_nma.acceleration;
        m_nmaAngledSpeed = m_nma.angularSpeed;
        m_nmaSpeed = m_nma.speed;
    }

    private void Update() {
        //Only run if the game is not paused
        if (Time.timeScale > 0) {
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
            UpdateAnims();
        }
    }

    //Charlie
    private void UpdateAnims()
    {
        float myVelocity = m_velocity.magnitude;
        Vector3 localVel = transform.InverseTransformDirection(m_velocity);
        
        m_playerAnimator.SetFloat("Velocity", myVelocity);
        
        m_playerAnimator.SetFloat("MovementDirectionRight", localVel.x);
        m_playerAnimator.SetFloat("MovementDirectionForward", localVel.z);

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
