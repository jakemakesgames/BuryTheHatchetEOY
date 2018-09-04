using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
//Michael Corben
//Based on Tutorial:https://www.youtube.com/watch?v=rZAnnyensgs&list=PLFt_AvWsXl0ctd4dgE1F8g3uec4zKNRV0&index=3
//Created 24/07/2018
//Last edited 03/09/2018


[RequireComponent (typeof(NavMeshAgent))]
[RequireComponent(typeof(WeaponController))]
[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour {

    #region Movement/ animation variables
        [Header("Movement variables")]
        [Tooltip("Movement speed of the player")]
        [SerializeField] private float m_speed = 5f;
        [Tooltip("Percent speed increace per sceond")]
        [SerializeField] private float m_accelerationRate = 0.1f;
        [Tooltip("Percent speed decrease per sceond")]
        [SerializeField] private float m_deccelerationRate = 0.99f;
        [Tooltip("Movement speed of the player at the start of the roll " +
        "The other speeds are set within the animation with events")]
        [SerializeField] private float m_rollSpeedStart = 20f;
        [Tooltip("The Time in seconds the player has to wait before they can roll again after rolling")]
        [SerializeField] private float m_rollCoolDownTime = 1f;
        [Tooltip("If an enemy is within this distance from the player " +
            "consider the player in combat")]
        [SerializeField] private float m_inCombatRadius = 10f;
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
        private AudioSource m_walkSpeaker;
        private AudioSource m_clothesSpeaker;
        private AudioSource m_rollSpeaker;
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
        [Header("Volumes")]
        [SerializeField] [Range(0, 1)] private float m_walkVol = 0.5f;
        [SerializeField] [Range(0, 1)] private float m_clothesVol = 0.5f;
        [SerializeField] [Range(0, 1)] private float m_rollVol = 0.5f;
    #endregion    

    #region private member variables
        private int m_equippedWeaponInumerator;
        private int m_ammoInClip;
        private int m_ammoInReserve;
        private float m_nmaSpeed;
        private float m_rollCoolDownTimer = 0;
        private float m_nmaAngledSpeed;
        private float m_nmaAcceleration;
        private bool m_isHoldingGun;
        private bool m_isRolling = false;
        private bool m_canRoll = true;
        private bool m_rollAccelerating = true;
        private bool m_inCombat = false;
        private NavMeshAgent m_nma;
        private Vector3 m_velocity;
        private Vector3 m_acceleration;
        private Vector3 m_movementVector;
        private Vector3 m_preMoveVector;
        private Camera m_viewCamera;
        private WeaponController m_weaponController;
        private Player m_player;
        private AudioSource m_audioSource;
    #endregion

    [Header("CHARLIE!")]
    public Animator m_playerAnimator;

    #region Player action methods
    //calls the equipped weapons attacking method 
    //(swing for melee or shoot for gun)
    //via the weapon controller script
    //and also checks if the player wishes to reload
    public void Attack() {
        //Get's the currently equipped weapon and executes the appropriate attack action
        //and animation
        Gun equippedGun = m_weaponController.GetEquippedGun();
        if (equippedGun == null) {
            Melee equippedMelee = m_weaponController.GetEquippedMelee();
            if (equippedMelee == null) {
                return;
            }
            if (equippedMelee.IsIdle) {
                if (Input.GetMouseButtonDown(0)) {
                    m_weaponController.Swing();
                    m_playerAnimator.SetTrigger("HatchetSwingTrigger");
                }
            }
            else
            {
                Debug.Log("axe not idle");
            }
        }

        else if (equippedGun.m_isAutomatic && equippedGun.IsIdle) {
            if (Input.GetMouseButton(0)) {
                if (m_weaponController.Shoot() && m_playerAnimator.GetBool("Reloading") == false)
                    m_playerAnimator.SetTrigger("Shoot");
            }
            else if (Input.GetKey(KeyCode.R)) {
                if (m_weaponController.ReloadEquippedGun() && m_playerAnimator.GetBool("Reloading") == false)
                    m_playerAnimator.SetBool("Reloading", true);
            }
        }
        else if (equippedGun.IsIdle) {
            m_playerAnimator.SetBool("Reloading", false);
            if (Input.GetMouseButtonDown(0)) {
                if (m_weaponController.Shoot() && m_playerAnimator.GetBool("Reloading") == false)
                    m_playerAnimator.SetTrigger("Shoot");
            }
            else if (Input.GetKey(KeyCode.R) && m_weaponController.GetEquippedGun().IsFull == false) {
                if(m_weaponController.ReloadEquippedGun() && m_playerAnimator.GetBool("Reloading") == false)
                    m_playerAnimator.SetBool("Reloading", true);
            }
        }
    }
    
    //Calculates the players velocity for the next frame
    private void Move() {
        if (m_isRolling)
            m_isRolling = true;
        //Calculating velocity
        if (m_isRolling == false)
            m_acceleration = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        else if (m_rollAccelerating == false)
            m_acceleration *= -1;
        
        m_acceleration *= m_accelerationRate;
        m_movementVector = Vector3.Lerp(m_movementVector, m_acceleration, m_deccelerationRate);
        if(m_movementVector.sqrMagnitude > 0.1)
            m_movementVector.Normalize();

        Vector3 direction = m_camera.transform.rotation * m_movementVector;
        direction.y = 0;
        Vector3 moveVelocity = direction.normalized * m_speed * Time.deltaTime;
        m_velocity = moveVelocity;
        m_nma.velocity = m_velocity;

        if (m_isRolling == false) {
            //Sound and Particle effects
            if ((m_walkSpeaker == null || m_clothesSpeaker == null || m_walkingParticleSystem == null) == false) {
                if (m_movementVector.sqrMagnitude > 0) {
                    if (m_walkSpeaker.isPlaying == false) {
                        if (m_walkSpeaker != null && m_walkingSound != null) 
                            m_walkSpeaker.Play(); /*NEED TO IMPLAMENT VOLUME CONTROL, RANDOM PITCHING AND RANDOMISE IF IT PLAYS*/

                        if (m_clothesSpeaker != null && m_clothesSpeaker != null) {
                            m_clothesSpeaker.Play();
                            m_clothesSpeaker.loop = true;
                        }
                        if (m_walkingParticleSystem != null)
                            m_walkingParticleSystem.Play();
                    }
                    else {

                    }
                }
                else if (m_walkSpeaker.isPlaying || m_clothesSpeaker.loop) {
                    if (m_walkSpeaker != null && m_walkingSound != null)
                        m_walkSpeaker.Stop();

                    if (m_clothesSpeaker != null && m_clothesSpeaker != null)
                        m_clothesSpeaker.loop = false;

                    if (m_walkingParticleSystem != null)
                        m_walkingParticleSystem.Stop();
                }
            }
        }
    }

    //Forces the player to look at the mouse position on screen
    //as well as place a crosshair object where the player is looking
    private void PlayerLookAt() {
        //Cast a ray from the given camera through the mouse to the created ground plane
        Ray ray = m_viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;
        //adjust the height of the position found by the ray to the height of the players 'hand'
        if (groundPlane.Raycast(ray, out rayDistance)) {
            Transform hand = m_weaponController.WeaponHold;

            Vector3 pointOnGround = ray.GetPoint(rayDistance);
            Vector3 heightCorrectedLookPoint = new Vector3(pointOnGround.x, transform.position.y, pointOnGround.z);

            ///TODO :: correct the look point to be along the line 
            ///rather than just height adjusted as the camera is not topdown
            transform.LookAt(heightCorrectedLookPoint);
            heightCorrectedLookPoint = new Vector3(pointOnGround.x, hand.position.y, pointOnGround.z);
            if(m_weaponController.EquippedGun != null)
                hand.LookAt(heightCorrectedLookPoint);
            m_weaponController.WeaponHold = hand;
            //place the crosshiar game object at this 'height corrected look point'
            if (m_crosshair != null)
                m_crosshair.transform.position = heightCorrectedLookPoint;
        }
    }

    //Quickly moves the player in the direction they are facing
    private void Roll() {
        if (m_isRolling == false) {
            if (Input.GetMouseButtonDown(1)) {
                m_playerAnimator.SetTrigger("Roll");
                m_isRolling = true;
                m_canRoll = false;
                m_rollAccelerating = true;
                if (m_nma.velocity != Vector3.zero) {
                    Vector3 newVel = m_nma.velocity.normalized;
                    m_acceleration = m_rollSpeedStart * newVel;
                    transform.forward = newVel;
                }
                else
                    m_acceleration = transform.forward * m_rollSpeedStart;

                if(m_rollSpeaker != null && m_rollSound != null)
                    m_rollSpeaker.Play(); /*NEED TO IMPLAMENT VOLUME CONTROL AND RANDOM PITCHING*/

                if(m_rollParticleSystem != null)
                    m_rollParticleSystem.Play();
            }
        }
    }

    //Coroutine used to detect if the player has entered combat range
    private IEnumerator CheckEnemyDistance() {
        while (true) {
            Collider[] enemies = Physics.OverlapSphere(transform.position, m_inCombatRadius, m_weaponController.EntityCollisionMask);
            m_inCombat = (enemies.Length > 0);
            m_playerAnimator.SetBool("WeaponActive", m_inCombat);
            yield return new WaitForSeconds(0.25f);
        }
    }
    #endregion

    #region Weapon methods
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
    //if they do, the method then stores the values of the currently equipped weapon
    //then equipes the player with the new weapon with the stored values for that specific weapon
    private void ChangeWeapon(int a_inumerator) {
        if (m_player.m_weaponsAvailableToPlayer[a_inumerator]) {
            if (m_player.m_heldWeapons[a_inumerator] != null) {
                SetWeaponInfo();

                if (m_weaponController.GetEquippedWeapon() != null)
                    m_player.AssignWeaponInfo(m_equippedWeaponInumerator, m_ammoInClip, m_ammoInReserve);

                m_equippedWeaponInumerator = a_inumerator;
                m_weaponController.EquipWeapon(m_player.m_heldWeapons[a_inumerator]);
                m_playerAnimator.SetInteger("whichWeapon", a_inumerator + 1);

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

    #endregion

    #region animation event functions

    public void SlowingRoll() {
        m_rollAccelerating = false;
    }

    //To be called as an animation event at the end of the roll animation.
    //Resets the player to a non-rolling state
    public void EndRoll() {
        m_nma.speed = m_nmaSpeed;
        m_nma.angularSpeed = m_nmaAngledSpeed;
        m_nma.acceleration = m_nmaAcceleration;

        m_isRolling = false;

        if (m_rollSpeaker != null && m_rollSound != null)
            m_rollSpeaker.Stop();

        if (m_rollParticleSystem != null)
            m_rollParticleSystem.Stop();
    }
    //To be called by the animator after the melee weapon swing animation has finished playing
    public void EndSwing() {
        m_weaponController.GetEquippedMelee().EndSwing();
    }
    #endregion

    //Charlie
    private void UpdateAnims()
    {
        float myVelocity = m_velocity.magnitude;
        
        //Debug.Log(myVelocity);
        Vector3 localVel = transform.InverseTransformDirection(m_velocity.normalized);
        if (m_preMoveVector != m_velocity) {
            localVel = m_preMoveVector + m_velocity;
            localVel.Normalize();
        }
        m_playerAnimator.SetFloat("Velocity", myVelocity);
        
        m_playerAnimator.SetFloat("MovementDirectionRight", localVel.x);
        m_playerAnimator.SetFloat("MovementDirectionForward", localVel.z);
        /*
        Debug.Log(m_playerAnimator.GetFloat("MovementDirectionRight")/100);
        
        ORIGINAL CODE//////////////////////////////////////////////////////////////////////////////
                playerAnimator.SetFloat ("MovementDirectionRight", m_velocity.x * transform.right.x);
                playerAnimator.SetFloat ("MovementDirectionForward", m_velocity.z * transform.forward.z);
        ////////////////////////////////////////////////////////////////////////////////////////////
        
        
                if (m_velocity.x * transform.right.x == 0){
                    playerAnimator.SetFloat ("MovementDirectionForward", m_velocity.z * transform.forward.z);
                }
        
        
        playerAnimator.SetFloat ("MovementDirectionForward", m_movementVector.x * transform.forward.x);
        
        playerAnimator.SetFloat ("MovementDirectionRight", m_movementVector.z * transform.right.z);
        */    
        m_preMoveVector = m_movementVector;
    }

    //Get all requied attached components and store them for later use
    private void Awake() {
        m_nma = GetComponent<NavMeshAgent>();
        m_weaponController = GetComponent<WeaponController>();
        m_player = GetComponent<Player>();
        m_audioSource = GetComponent<AudioSource>();

        //Create the speakers for the individual sounds
        
        m_walkSpeaker = gameObject.AddComponent<AudioSource>();
        m_walkSpeaker.volume = m_walkVol;
        if (m_walkSpeaker != null)
            m_walkSpeaker.clip = m_walkingSound;

        m_clothesSpeaker = gameObject.AddComponent<AudioSource>();
        m_clothesSpeaker.volume = m_clothesVol;
        if (m_clothesSpeaker != null)
            m_clothesSpeaker.clip = m_clothesRustleSound;

        m_rollSpeaker = gameObject.AddComponent<AudioSource>();
        m_rollSpeaker.volume = m_rollVol;
        if (m_rollSpeaker != null)
            m_rollSpeaker.clip = m_rollSound;

        if (m_walkingParticleSystem != null)
            m_walkingParticleSystem.Stop();
        if(m_rollParticleSystem != null)
            m_rollParticleSystem.Stop();

        m_playerAnimator = GetComponentInChildren<Animator>();
        m_viewCamera = m_camera;
        m_nmaAcceleration = m_nma.acceleration;
        m_nmaAngledSpeed = m_nma.angularSpeed;
        m_nmaSpeed = m_nma.speed;

        StartCoroutine(CheckEnemyDistance());
    }

    private void Update() {
        //Only run if the game is not paused
        if (Time.timeScale > 0 && m_player.Dead == false) {
            if (m_isRolling == false) {
                //Switch Weapons
                SwitchWeapon();

                //Player looking at mouse
                PlayerLookAt();

                //Player attacking
                Attack();
            }
            
            else if(m_rollCoolDownTimer <= Time.time)
                m_canRoll = true;

            //Player movement
            Move();

            //Player rolling
            if(m_canRoll)
                Roll();
            
            //Ammo display
            DisplayAmmo();

            //Charlie
            UpdateAnims();
        }
    }

}
