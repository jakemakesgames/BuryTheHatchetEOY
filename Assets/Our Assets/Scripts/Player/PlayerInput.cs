﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
//Michael Corben
//Based on Tutorial:https://www.youtube.com/watch?v=rZAnnyensgs&list=PLFt_AvWsXl0ctd4dgE1F8g3uec4zKNRV0&index=3
//Created 24/07/2018
//Last edited 7/11/2018


[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(WeaponController))]
[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour {
    public enum InteractableObject
    {
        NONE = 0,
        RESPAWNPOINT,
        MINECART,
        PICKUP
    }

    //----------------------------
    //Variables and access
    //----------------------------
    #region Melee attacking variables

    [Header("Melee Weapon")]
    [Tooltip("The damage delt to any enemy within the hitbox")]
    [SerializeField]
    private int m_meleeDamage = 10;

    [Tooltip("Movement speed of the player whilst lunging with their hatchet")]
    [SerializeField]
    private float m_lungeSpeed = 25f;

    [Tooltip("Time the player is lunging in seconds")]
    [SerializeField]
    private float m_lungeTime = 0.5f;

    [Tooltip("Time the player is swinging in seconds")]
    [SerializeField]
    private float m_swingTime = 0.5f;

    [Tooltip("The hit box of the melee swing")]
    [SerializeField]
    private BoxCollider m_meleeHitBox;

    //The damageable objects hit during this swing
    private List<IDamagable> m_hitThisSwing = new List<IDamagable>();

    #endregion

    //----------------------------
    #region Movement/ animation variables
    [Header("Radii")]
    [SerializeField] private float m_inCombatRadius = 10f;
    [SerializeField] private bool m_drawRadius = true;

    [Header("!!CHARLIE!!")]
    public Animator m_playerAnimator;
    [Tooltip("The empty game object on the players hips to get relative movement")]
    [SerializeField] private Transform m_hips;

    [Header("ReloadMovementEffects")]
    [Tooltip("Will the player slow down when they reload")]
    [SerializeField] private bool m_slowWhenReload = false;
    [Tooltip("Will the player stop when they reload, overrides slow when reload")]
    [SerializeField] private bool m_stopWhenReload = false; 
    [Tooltip("the speed multiplier when the player is slowed by reloading")]
    [SerializeField] private float m_reloadingWalkSpeedMult = 1f;

    [Header("Movement variables")]
    [Tooltip("Movement speed of the player")]
    [SerializeField] private float m_speed = 5f;

    [Tooltip("Percent speed increace per sceond")]
    [SerializeField] private float m_accelerationRate = 0.1f;

    [Tooltip("Percent speed decrease per sceond")]
    [SerializeField] private float m_decelerationRate = 0.99f;

    [Header("Roll Variables")]
    [Tooltip("Do we use the below roll curve to control the roll?" +
        " Or do we use the below values")]
    [SerializeField] private bool m_usingCurveForRoll;

    [Tooltip("Controls the speed of the roll over time")]
    [SerializeField] private AnimationCurve m_rollCurve;

    [Tooltip("Movement speed of the player at the start of the roll")]
    [SerializeField] private float m_rollSpeedStart = 100f;

    [Tooltip("The Time in seconds the player has to wait before they can roll again after rolling")]
    [Range(0.125f, 1f)] [SerializeField] private float m_rollCoolDownTime = 0.25f;

    [Tooltip("Speed increase/decrease per sceond when rolling")]
    [Range(1.1f, 5)]  [SerializeField] private float m_rollAccelerationRate = 3f;

    [Tooltip("Controls the x component of the roll curves, higher values make the roll switch curves faster")]
    [SerializeField] private float m_rollTimeMultiplier = 1.2f;

    [Tooltip("The Time in seconds the player is invincible after starting to roll")]
    [SerializeField] private float m_invicibilityTime = 1f;

    [Header("Spur Variables")]
    [Tooltip("The thing that'll spin while tthe player cannot roll")]
    [SerializeField] private GameObject m_spur;

    [Tooltip("the rate of spin over time?")]
    [SerializeField] private AnimationCurve m_spurAnimCurve;

    [Tooltip("The max angle of rotation the spur will go to by the end of the spin")]
    [SerializeField] private float m_spurMaxAngle;

    [Tooltip("The time in seconds the object will spin after th eplayer presses the roll button")]
    [SerializeField] private float m_spurSpinTime = 1f;

    private bool m_spurSpinning = false;

    private Vector3 m_velocityModifyer = Vector3.zero;
    #endregion

    //----------------------------
    #region Shooting variables

    [Header("Shooting Variables")]
    [Tooltip("Does the player have infinite ammo?")]
    [SerializeField] private bool m_infiniteAmmo;

    [Tooltip("The ammo flashing image")]
    [SerializeField] private Image m_ammoFlash;

    [Tooltip("Will the player pause when they shoot")]
    [SerializeField] private bool m_willPause;

    [Tooltip("The time the player is paused for while shooting")]
    [SerializeField] private float m_shootPauseTime;

    //for use within the animation event of the same name
    private bool m_canShoot;

    #endregion

    //----------------------------
    #region In world game objects
    [Header("In world game objects")]
    [Tooltip("The camera used to orient the player when the camera rotates, required for movement to work")]
    [SerializeField] private Camera m_camera;

    [Tooltip("The reference to the canvas for the player and the crosshair")]
    [SerializeField] private Canvas m_canvas;

    [Header("Ammo display")]

    [Tooltip("The cards ammo display controller")]
    [SerializeField] private AmmoController m_ammoController;

    [Tooltip("the tag of the ammo clip text object")]
    [SerializeField] private string m_clipTextTag;
    [Tooltip("the tag of the ammo reserve text object")]
    [SerializeField] private string m_ammoTextTag;
    [Tooltip("Will change the text of this object to the amount " +
        "of ammo left in the currently equipped gun's clip")]
    [SerializeField] private Text m_clipAmmoDisplay;
    [Tooltip("will change the text of this object to the amount of ammo" +
        "the player has left excluding the ammo in the clip")]
    [SerializeField] private Text m_totalAmmoDisplay;
    private GameObject m_interactionObject;
    #endregion

    //----------------------------
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

    [Tooltip("The sound that will player when the player can roll again")]
    [SerializeField] private AudioClip m_canRollSound;

    [Header("Particles")]
    [Tooltip("Paricles that will play when the player is walking")]
    [SerializeField] private ParticleSystem m_walkingParticleSystem;

    [Tooltip("Particles that will play when the player rolls")]
    [SerializeField] private ParticleSystem m_rollParticleSystem;

    [Tooltip("The particle effect to indicate when the player is invincible")]
    [SerializeField] private ParticleSystem m_invincibilityParticle;

    [Tooltip("The particle that will play from the players feet when they shoot")]
    [SerializeField] private ParticleSystem m_shootDustParticle;

    [Header("Volumes")]
    [Range(0, 1)] [SerializeField] private float m_walkVol = 0.5f;
    [Range(0, 1)] [SerializeField] private float m_clothesVol = 0.5f;
    [Range(0, 1)] [SerializeField] private float m_rollVol = 0.5f;
    #endregion

    //----------------------------
    #region private member variables
    private int m_equippedWeaponInumerator;
    private int m_ammoInClip;
    private int m_ammoInReserve;

    private float m_nmaSpeed;
    private float m_rollCoolDownTimer = 0;
    private float m_nmaAngledSpeed;
    private float m_nmaAcceleration;
    private float m_rollStartTime;
    private float m_rollTimePassed;
    private float m_invicibilityTimer = 0;
    private float m_inCombatTimer = 0f;
    private float m_lungeTimer = 0f;
    private float m_swingTimer = 0f;
    private float m_isShootingTimer = 0f;
    private float m_rollSpeed = 0f;
    private float m_distanceToClosestEnemy = 200f;
    private float m_lastVolume = 0;

    private bool m_isHoldingGun;
    private bool m_canAttack = true;
    private bool m_isRolling = false;
    private bool m_canRoll = true;
    private bool m_rollAccelerating = true;
    private bool m_inCombat = false;
    private bool m_isInvincible = false;
    private bool m_isLunging = false;
    private bool m_isShooting = false;

    private InteractableObject m_currentlyCanInteractWith;
    private InteractableObject m_currentlyInteractingWith;

    private NavMeshAgent m_nma;

    private Vector3 m_velocity;
    private Vector3 m_acceleration;
    private Vector3 m_movementVector;
    private Vector3 m_preMoveVector;
    private Vector3 m_rollVelocity;
    private Vector3 m_velPreRoll;

    private Camera m_viewCamera;
    private WeaponController m_weaponController;
    private Player m_player;
    private AudioSource m_audioSource;
    #endregion

    //----------------------------
    #region properties
    public bool IsInvincible { get { return m_isInvincible; } set { m_isInvincible = value; } }

    public bool CanAttack { get { return m_canAttack; } set { m_canAttack = value; } }

    public bool InCombat { get { return m_inCombat; } }

    private bool GunFull { get { return m_weaponController.EquippedGun.IsFull; } }

    private bool GunEmpty { get { return m_weaponController.EquippedGun.GetIsEmpty(); } }

    public int CurrentAmmoInClip {
        get {
            if (m_weaponController.GetEquippedGun() != null)
                return m_weaponController.GetEquippedGun().CurrentClip;
            else
                return 0;
        }
    }

    public int CurrentHealth     {
        get { return m_player.GetHealth(); }
    }

    public float DistanceToClosestEnemy { get { return m_distanceToClosestEnemy; } set { m_distanceToClosestEnemy = value; } }

    public InteractableObject CurrentlyCanInteractWith {
        get { return m_currentlyCanInteractWith; }
        set { m_currentlyCanInteractWith = value; }
    }

    public InteractableObject CurrentlyInteractingWith {
        get { return m_currentlyInteractingWith; }
        set { m_currentlyInteractingWith = value; }
    }

    public GameObject InteractionObject {
        get { return m_interactionObject; }
        set { m_interactionObject = value; }
    }


    public Vector3 VelocityModifyer {
        get { return m_velocityModifyer; }
        set { m_velocityModifyer = value; }
    }
    public Player Player {
        get { return m_player; }
        set { m_player = value; }
    }

    public WeaponController WeapCont {
        get { return m_weaponController; }
        set { m_weaponController = value; }
    }

    public AmmoController AmmoCont {
        get { return m_ammoController; }
    }
    #endregion

    //----------------------------
    //Methods and functionality
    //----------------------------

    //----------------------------
    #region Spinning Thing Coroutine
    IEnumerator SpinThing() {
        System.DateTime startSpinTime = System.DateTime.Now;
        m_spurSpinning = true;
        float spinTimer = 0.0f;
        float startAngle = m_spur.transform.eulerAngles.z;

        while (spinTimer < m_spurSpinTime) {
            float currentTime = (float)(System.DateTime.Now - startSpinTime).TotalSeconds;
            float acceleration = 1.5f * currentTime;
            float angle = m_spurAnimCurve.Evaluate(spinTimer / m_spurSpinTime) * m_spurMaxAngle;
            m_spur.GetComponentInChildren<RectTransform>().eulerAngles = new Vector3(0.0f, 0.0f, -(angle + startAngle));
            spinTimer += (Time.deltaTime * acceleration);
            yield return 0;
        }
        m_spur.GetComponentInChildren<RectTransform>().eulerAngles = new Vector3(0.0f, 0.0f, -(m_spurMaxAngle + startAngle));
        m_spurSpinning = false;
        m_canRoll = true;
        if (m_rollSpeaker != null && m_canRollSound != null) m_rollSpeaker.PlayOneShot(m_canRollSound, Player.SFXVolume);
    }
    #endregion

    //----------------------------
    #region Player action methods
    //calls the equipped weapons attacking method 
    //(swing for melee or shoot for gun)
    //via the weapon controller script
    //and also checks if the player wishes to reload
    public void Attack() {

        //When the timer for the melee attack hit box has finished turn it off
        if (m_swingTimer < Time.time)
            m_meleeHitBox.enabled = false;
        
        //If the player is in a reload animation check if the user can and wants to reload and if so stay in the reload animation
        if (   m_playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Top.Character_Anim_Reload_v01") 
            || m_playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Top.Character_Anim_Reload_v01 0")) {
            if (Input.GetKey(KeyCode.R) && m_weaponController.GetEquippedGun().IsFull == false) {
                if (m_weaponController.ReloadEquippedGun()) {
                    if (m_ammoController != null)
                        m_ammoController.Reload();
                    m_playerAnimator.ResetTrigger("FinishedReloading");
                    m_playerAnimator.SetBool("Reload", true);
                }
            }
        }

        //If the gun is full or the player doesn't want to reload tell the animator to exit the reloading animations
        if (Input.GetKey(KeyCode.R) == false || GunFull) { m_playerAnimator.SetTrigger("FinishedReloading"); m_playerAnimator.SetBool("Reload", false); }
            
        //Play the gun empty idle
        if (GunEmpty)
            m_playerAnimator.SetBool("GunEmpty", true);
        else if (GunEmpty == false)
            m_playerAnimator.SetBool("GunEmpty", false);

        //Get's the currently equipped weapon and executes
        //the appropriate attack action and animation
        if (   m_canShoot 
            || m_playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Top.Character_Anim_Idle_v01") 
            || m_playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Top.Walking")
            || m_playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Top.Empty")) {

            Gun equippedGun = m_weaponController.GetEquippedGun();
            if (equippedGun.IsIdle && m_meleeHitBox.enabled == false) {

                if (m_isShooting)
                    DelayedShoot();

                else {
                    #region unused melee
                //---------------//
                //MELEE ATTACKING//
                //---------------//
                //if (Input.GetMouseButtonDown(1)) {
                //    m_playerAnimator.SetTrigger("HatchetSwingTrigger");
                //
                //    m_isLunging = true;
                //    m_lungeTimer = Time.time + m_lungeTime;
                //    m_swingTimer = Time.time + m_swingTime;
                //
                //    if (m_meleeHitBox != null)
                //        m_meleeHitBox.enabled = true;
                //}
                #endregion

                    //-------------//
                    //GUN ATTACKING//
                    //-------------//
                    if (Input.GetMouseButtonDown(0)) {
                        if (m_willPause) {
                            m_isShootingTimer = Time.time + m_shootPauseTime;
                            m_isShooting = true;
                            return;
                        }

                        if (m_weaponController.EquippedGun.CanShoot()) {
                            if (m_ammoController != null)
                                m_ammoController.Shoot();
                            m_canShoot = false;
                            m_playerAnimator.SetTrigger("Shoot");
                        }
                    }

                    //-------------//
                    //GUN RELOADING//
                    //-------------//
                    else if (Input.GetKey(KeyCode.R) && m_weaponController.GetEquippedGun().IsFull == false) {
                        if (m_weaponController.ReloadEquippedGun()) {
                            if (m_ammoController != null)
                                m_ammoController.Reload();
                            m_playerAnimator.ResetTrigger("FinishedReloading");
                            m_playerAnimator.SetBool("Reload", true);
                        }
                    }
                }
                #region old melee
            //if (equippedGun == null) {
            //    Melee equippedMelee = m_weaponController.GetEquippedMelee();
            //    if (equippedMelee == null)
            //        return;
            //
            //    if (equippedMelee.IsIdle) {
            //        if (Input.GetMouseButtonDown(0)) {
            //            if (m_inCombat) {
            //                m_weaponController.Swing();
            //                m_playerAnimator.SetTrigger("HatchetSwingTrigger");
            //            }
            //            else {
            //                m_inCombat = true;
            //                m_inCombatTimer = Time.time + m_inCombatTime;
            //            }
            //        }
            //    }
            //}
            #endregion
                #region unneeded auto gun shooting
            /*else if (equippedGun.m_isAutomatic && equippedGun.IsIdle) {
                if (Input.GetMouseButton(0)) {
                    if (m_inCombat) {
                        if (m_weaponController.Shoot() && m_playerAnimator.GetBool("Reloading") == false)
                            m_playerAnimator.SetTrigger("Shoot");
                    }
                    else {
                        m_inCombat = true;
                        m_inCombatTimer = Time.time + m_inCombatTime;
                    }
                }
                else if (Input.GetKey(KeyCode.R)) {
                    if (m_weaponController.ReloadEquippedGun() && m_playerAnimator.GetBool("Reloading") == false)
                        m_playerAnimator.SetBool("Reloading", true);
                }
            }*/
            #endregion
            }
        }
    }

    //Delayed shooting
    private void DelayedShoot() {
        if (m_isShootingTimer < Time.time) {
            if (m_weaponController.EquippedGun.CanShoot()) {
                m_isShooting = false;

                if (m_ammoController != null)
                    m_ammoController.Shoot();

                m_playerAnimator.SetTrigger("Shoot");

                if (m_shootDustParticle != null)
                    m_shootDustParticle.Play();

                if (m_player.m_camAnimator != null)
                    m_player.m_camAnimator.KickbackShake();
            }
        }
    }

    //Calculates the players velocity for the next frame
    private void Move() {
        Vector3 moveVelocity = Vector3.one;

        //Calculating velocity
        if ((m_willPause && m_isShooting) == false) {
            //----------------//
            //LUNGING MOVEMENT//
            //----------------//
            if (m_isLunging)
            {
                if (m_lungeTimer < Time.time)
                    m_isLunging = false;
                else
                    m_velocity = m_lungeSpeed * transform.forward;
            }

            //----------------//
            //REGULAR MOVEMENT//
            //----------------//
            else if (m_isRolling == false)
            {

                m_acceleration = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
                m_acceleration *= m_accelerationRate;

                m_movementVector = Vector3.Lerp(m_movementVector, m_acceleration, m_decelerationRate);

                if (m_movementVector.sqrMagnitude > 1f)
                    m_movementVector.Normalize();

                Vector3 direction = m_viewCamera.transform.rotation * m_movementVector;
                direction.y = 0;
                moveVelocity = direction.normalized * m_speed;
                m_velocity = moveVelocity;

                //Sound and Particle effects
                if ((m_walkSpeaker == null || m_clothesSpeaker == null || m_walkingParticleSystem == null) == false)
                {
                    if (m_movementVector.sqrMagnitude > 0)
                    {
                        if (m_walkSpeaker.isPlaying == false)
                        {
                            if (m_walkSpeaker != null && m_walkingSound != null) {
                                if (Random.Range(0, 1) == 1) { m_walkSpeaker.Play(); m_walkSpeaker.pitch = Random.Range(0.9f, 1.1f); }
                            }

                            if (m_clothesSpeaker != null && m_clothesSpeaker != null) {
                                m_clothesSpeaker.Play();
                                m_clothesSpeaker.loop = true;
                                m_clothesSpeaker.pitch = Random.Range(0.9f, 1.1f);
                            }
                            if (m_walkingParticleSystem != null && m_walkingParticleSystem.isPlaying == false)
                                m_walkingParticleSystem.Play();
                        }
                        else
                        {

                        }
                    }
                    else if (m_walkSpeaker.isPlaying || m_clothesSpeaker.loop)
                    {
                        if (m_walkSpeaker != null && m_walkingSound != null)
                            m_walkSpeaker.Stop();

                        if (m_clothesSpeaker != null && m_clothesSpeaker != null)
                            m_clothesSpeaker.loop = false;

                        if (m_walkingParticleSystem != null)
                            m_walkingParticleSystem.Stop();
                    }
                }
            }

            //----------------//
            //ROLLING MOVEMENT//
            //----------------//
            else {
                //time passed = t
                //acceleration rate = a
                if (m_usingCurveForRoll) {
                    m_rollTimePassed = (Time.time - m_rollStartTime);
                    m_rollSpeed = m_rollCurve.Evaluate(m_rollTimePassed * m_rollTimeMultiplier) * m_rollAccelerationRate;
                    m_rollVelocity = transform.forward * m_rollSpeed;
                }
                else {
                    m_rollTimePassed = (Time.time - m_rollStartTime) * (m_rollTimeMultiplier + m_rollAccelerationRate);
                    if (m_rollAccelerating) {
                        //Accelerate along a parabola starting at 0 ending at 1
                        //velocity = -1 * (t - a)^2 + a^2
                        m_rollSpeed = -1 * Mathf.Pow(m_rollTimePassed - m_rollAccelerationRate, 2) + Mathf.Pow(m_rollAccelerationRate, 2);
                        m_rollVelocity = transform.forward * m_rollSpeed;
                        if (m_rollTimePassed - m_rollAccelerationRate >= 0)
                            m_rollAccelerating = false;
                    }
                    else {
                        //Decelerate along an exponential graph starting at 1 and tending toward 0
                        //velocity = a^( -( t - a ) + 2)
                        //float power = -(m_rollTimePassed - m_rollAccelerationRate) + 2;
                        //m_rollVelocity = transform.forward * ((Mathf.Pow(m_rollAccelerationRate, power)));

                        //Decelerate along a horizontal porabola starting at one and ending at zero
                        //-a squareRoot(at - a^2) + a^2
                        float squareRoot = Mathf.Sqrt(m_rollAccelerationRate * m_rollTimePassed - Mathf.Pow(m_rollAccelerationRate, 2));
                        m_rollSpeed = (-m_rollAccelerationRate * (squareRoot + Mathf.Pow(m_rollAccelerationRate, 2)));
                        m_rollVelocity = transform.forward * m_rollSpeed;

                        Debug.Log("Decelerating");
                    }
                }
                if (m_rollSpeed >= 0)
                    m_velocity = m_rollVelocity + m_velPreRoll;

                else
                    m_velocity = m_preMoveVector + m_velPreRoll;
            }
        }
        else
            m_velocity = Vector3.zero;
        if (m_slowWhenReload) {
            if (m_playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Top.Character_Anim_Reload_v01") ||
                    m_playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Character_Anim_Reload_v01 0")) {
                m_velocity *= m_reloadingWalkSpeedMult;
            }
        }
            m_nma.velocity = m_velocity + VelocityModifyer;

        if (m_stopWhenReload) {
            if (m_playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Top.Character_Anim_Reload_v01") ||
                   m_playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Character_Anim_Reload_v01 0")) {
                m_nma.velocity *= 0;
            }
        }
        VelocityModifyer = Vector3.zero;
    }

    //Forces the player to look at the mouse position on screen
    private void PlayerLookAt() {
        if ((m_isShooting && m_willPause) == false) { 
            Transform hand = m_weaponController.EquippedGun.Muzzle;

            //Cast a ray from the given camera through the mouse to the created ground plane
            Ray ray = m_viewCamera.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, hand.position);

            if (m_weaponController.EquippedGun != null) {
                Vector3 planePos = m_weaponController.EquippedGun.Muzzle.position;
                groundPlane = new Plane(Vector3.up, planePos);
            }

            float rayDistance;

            //Cast the ray from the camera to the generated ground plane which will be created at the players hand position
            if (groundPlane.Raycast(ray, out rayDistance)) {
                Vector3 lookAtPoint = ray.GetPoint(rayDistance);
                Vector3 heightCorrectedLookPoint = new Vector3(lookAtPoint.x, transform.position.y, lookAtPoint.z);
                transform.LookAt(heightCorrectedLookPoint);

                hand.LookAt(lookAtPoint);
                m_weaponController.EquippedGun.Muzzle = hand;
            }
        }
    }

    //forces the players held gun to look at the mouse position on screen
    private void GunLookAt() {
        if ((m_isShooting && m_willPause) == false)
        {
            Transform hand = m_weaponController.WeaponHold;

            //Cast a ray from the given camera through the mouse to the created ground plane
            Ray ray = m_viewCamera.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, hand.position);

            if (m_weaponController.EquippedGun != null)
            {
                Vector3 planePos = m_weaponController.EquippedGun.Muzzle.position;
                groundPlane = new Plane(Vector3.up, planePos);
            }

            float rayDistance;

            //Cast the ray from the camera to the generated ground plane which will be created at the players hand position
            if (groundPlane.Raycast(ray, out rayDistance)) {
                Vector3 lookAtPoint = ray.GetPoint(rayDistance);
                if (m_weaponController.EquippedGun != null && m_weaponController.EquippedGun.IsReloading == false) {
                    hand.LookAt(lookAtPoint);
                    m_weaponController.WeaponHold = hand;
                }
            }
        }
    }

    //Quickly moves the player in the direction they are facing
    private void Roll() {
        if (m_isRolling == false && m_isLunging == false) {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.LeftShift) || Input.GetMouseButtonDown(1)) {
                if (m_playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Revolver Shoot") == false) {
                    m_rollStartTime = Time.time;
                    m_invicibilityTimer = m_rollStartTime + m_invicibilityTime;
                    m_playerAnimator.SetBool("Roll", true);
                    Invoke("ResetRollBool", 0.2f);

                    m_playerAnimator.SetBool("Reloading", false);
                    m_isRolling = true;
                    m_canRoll = false;
                    if (m_spur != null)
                        StartCoroutine(SpinThing());
                    m_rollAccelerating = true;
                    IsInvincible = true;

                    if (m_nma.velocity != Vector3.zero)
                    {
                        Vector3 newForward = m_nma.velocity.normalized;
                        transform.forward = newForward;
                    }

                    m_velPreRoll = m_velocity;

                    if (m_rollSpeaker != null && m_rollSound != null) { m_rollSpeaker.Play(); m_rollSpeaker.pitch = Random.Range(0.9f, 1.1f);}


                    if (m_rollParticleSystem != null)
                        m_rollParticleSystem.Play();

                    if (m_invincibilityParticle != null)
                        m_invincibilityParticle.Play();
                }
            }
        }
    }

    private void ResetRollBool() { m_playerAnimator.SetBool("Roll", false); }

    //Player interacting with the world in way other than moving and attacking
    private void Interact() {

    }

    //Coroutine used to detect if the player has entered combat range
    private IEnumerator CheckCombatRadius() {
        while (true) {
            if (m_distanceToClosestEnemy <= m_inCombatRadius)
                m_inCombat = true;
            else
                m_inCombat = false;
            m_distanceToClosestEnemy = 200f;
            yield return new WaitForSeconds(0.25f);
        }
    }
    
    IEnumerator FlashAmmo() {
        Color healthColour = m_ammoFlash.color;
        float alpha = 0;
        m_ammoFlash.color = new Color(healthColour.r, healthColour.g, healthColour.b, alpha);
        while (true) {
            if (m_weaponController.EquippedGun.GetIsEmpty()) {
                alpha -= Time.deltaTime;
                m_ammoFlash.color = new Color(healthColour.r, healthColour.g, healthColour.b, alpha);
                if (alpha <= 0)
                    alpha = 1f;
            }
            else { m_ammoFlash.color = new Color(healthColour.r, healthColour.g, healthColour.b, 0); }
            yield return new WaitForEndOfFrame();
        }
    }

    #endregion

    //----------------------------
    #region Interaction methods
    private void CampFireInteraction()
    {

    }

    private void MinecartInteraction()
    {

    }

    private void PickupInteraction()
    {

    }
    #endregion

    //----------------------------
    #region Weapon methods
    //Gives a ui text object the players ammo for their currently equipped weapon
    private void DisplayAmmo() {
        Gun equippedGun = m_weaponController.GetEquippedGun();
        if (equippedGun != null) {
            if (m_clipAmmoDisplay != null)
                m_clipAmmoDisplay.GetComponent<Text>().text =
                    (equippedGun.GetCurrentClip().ToString() + " / " + equippedGun.GetCurrentAmmo().ToString());
        }
        else {
            if (m_clipAmmoDisplay != null)
                m_clipAmmoDisplay.GetComponent<Text>().text = (" ");
        }
    }

    //Caches the weapon info of the currently equipped weapon for storing on weapon switch
    private void SetWeaponInfo() {
        if (m_weaponController.GetEquippedGun() == null) {
            m_ammoInClip = 0;
            m_ammoInReserve = 0;
            m_isHoldingGun = false;
        }
        else         {
            m_ammoInClip = m_weaponController.GetEquippedGun().GetCurrentClip();
            m_ammoInReserve = m_weaponController.GetEquippedGun().GetCurrentAmmo();
            m_isHoldingGun = true;
        }
    }

    //Checks if the player has access the weapon that the player atempemted to equip
    //if they do, the method then stores the values of the currently equipped weapon
    //then equipes the player with the new weapon with the stored values for that specific weapon
    //private void ChangeWeapon(int a_inumerator) {
    //    if (Player.m_weaponsAvailableToPlayer[a_inumerator]) {
    //        if (Player.m_heldWeapons[a_inumerator] != null) {
    //            SetWeaponInfo();
    //
    //            if (m_weaponController.GetEquippedWeapon() != null)
    //                Player.AssignWeaponInfo(m_equippedWeaponInumerator, m_ammoInClip, m_ammoInReserve);
    //            m_equippedWeaponInumerator = a_inumerator;
    //            m_weaponController.EquipWeapon(Player.m_heldWeapons[a_inumerator]);
    //            Player.HeldWeaponLocation = a_inumerator + 1;
    //            m_playerAnimator.SetInteger("whichWeapon", Player.HeldWeaponLocation);
    //
    //            if (Player.ToEquipIsMelee(a_inumerator) == false) {
    //                Gun gun = m_weaponController.GetEquippedGun();
    //                gun.SetCurrentClip(Player.ToEquipCurrentClip(a_inumerator));
    //                gun.SetCurrentReserveAmmo(Player.ToEquipCurrentReserve(a_inumerator));
    //                if (gun.CurrentClip < gun.ClipSize)
    //                    gun.IsFull = false;
    //            }
    //        }
    //    }
    //}

    //Equip the weapon stored in the location
    //based on which number the player presses 
    //private void SwitchWeapon() {
    //    Gun equippedGun = m_weaponController.GetEquippedGun();
    //    bool canSwitch = true;
    //    if (equippedGun == null) {
    //        canSwitch = m_weaponController.GetEquippedMelee().IsIdle;
    //    }
    //    else {
    //        canSwitch = equippedGun.IsIdle;
    //    }
    //    if (Input.GetKeyDown(KeyCode.Alpha1)) {
    //        int weaponInumerator = 0;
    //        ChangeWeapon(weaponInumerator);
    //    }
    //    else if (Input.GetKeyDown(KeyCode.Alpha2)) {
    //        int weaponInumerator = 1;
    //        ChangeWeapon(weaponInumerator);
    //    }
    //    else if (Input.GetKeyDown(KeyCode.Alpha3)) {
    //        int weaponInumerator = 2;
    //        ChangeWeapon(weaponInumerator);
    //    }
    //    else if (Input.GetKeyDown(KeyCode.Alpha4)) {
    //        int weaponInumerator = 3;
    //        ChangeWeapon(weaponInumerator);
    //    }
    //    else if (Input.GetKeyDown(KeyCode.Alpha5)) {
    //        int weaponInumerator = 4;
    //        ChangeWeapon(weaponInumerator);
    //    }
    //}

    private void OnHitObject(Collider a_c) {
        IDamagable damagableObject = a_c.GetComponent<IDamagable>();

        if (damagableObject != null) {
            bool ignore = false;
            for (int i = 0; i < m_hitThisSwing.Count - 1; i++) {
                if (damagableObject == m_hitThisSwing[i])
                    ignore = true;
            }
            if (ignore == false) {
                damagableObject.TakeDamage(m_meleeDamage);
                m_hitThisSwing.Add(damagableObject);
            }
        }
    }
    #endregion

    //----------------------------
    #region animation functions

    public void HalfWay() { Debug.Log("HalfRoll: " + m_rollTimePassed); }

    //To be called via an animation event which will tell the roll melthod
    //when to switch from the first curve to the second
    public void SlowingRoll() { m_rollAccelerating = false; }

    //To be called as an animation event at the end of the roll animation.
    //Resets the player to a non-rolling state
    public void EndRoll() {
        m_nma.speed = m_nmaSpeed;
        m_nma.angularSpeed = m_nmaAngledSpeed;
        m_nma.acceleration = m_nmaAcceleration;

        m_rollCoolDownTimer = Time.time + m_rollCoolDownTime;
        m_playerAnimator.SetBool("Roll", false);

        m_isRolling = false;

        if (m_rollSpeaker != null && m_rollSound != null)
            m_rollSpeaker.Stop();

        if (m_rollParticleSystem != null)
            m_rollParticleSystem.Stop();
    }

    //To be called by the animator after the melee weapon swing animation has finished playing
    public void EndSwing() { if (m_meleeHitBox != null) m_meleeHitBox.enabled = false; }

    //Shoots the equipped gun when the animation event triggers it todo so
    public void Shoot() {
        if (m_weaponController.EquippedGun != null)
            m_weaponController.Shoot();
        if (m_player.m_camAnimator != null)
            m_player.m_camAnimator.KickbackShake();
    }

    //Plays the dust knockback particle when the animations event triggers it.
    public void ShootDust() { if (m_shootDustParticle != null) m_shootDustParticle.Play(); }

    //Tells the player that they canshoot when an animation event fires
    public void CanShoot() { m_canShoot = true; }

    //Charlie
    private void UpdateAnims() {
        float myVelocity = m_velocity.magnitude;

        Vector3 localVel;
        if (m_hips == null)
            localVel = transform.InverseTransformDirection(m_velocity.normalized);

        else
            localVel = m_hips.InverseTransformDirection(m_velocity.normalized);

        m_playerAnimator.SetFloat("Velocity", myVelocity);

        m_playerAnimator.SetFloat("MovementDirectionRight", localVel.x);
        m_playerAnimator.SetFloat("MovementDirectionForward", localVel.z);
    }
    #endregion

    //----------------------------
    #region Misc Functionality
    private void UpdateVolumes() {
        if (m_lastVolume != Player.SFXVolume) {
            m_clothesSpeaker.volume = Player.SFXVolume;
            m_rollSpeaker.volume = Player.SFXVolume;
            m_walkSpeaker.volume = Player.SFXVolume;
            m_lastVolume = Player.SFXVolume;
        }
    }
    #endregion

    //----------------------------
    #region startup
    //Get all requied attached components and store them for later use
    private void Awake() {
        m_nma = GetComponent<NavMeshAgent>();
        m_weaponController = GetComponent<WeaponController>();
        Player = GetComponent<Player>();
        m_audioSource = GetComponent<AudioSource>();

        Text[] texts = FindObjectsOfType<Text>();
        for (int i = 0; i < texts.Length; i++) {
            if (texts[i].tag == m_clipTextTag)
                m_clipAmmoDisplay = texts[i];

            else if (texts[i].tag == m_ammoTextTag)
                m_totalAmmoDisplay = texts[i];
        }

        //Create the speakers for the individual sounds

        m_walkSpeaker = gameObject.AddComponent<AudioSource>();
        m_walkSpeaker.volume = Player.SFXVolume;
        if (m_walkSpeaker != null)
            m_walkSpeaker.clip = m_walkingSound;

        m_clothesSpeaker = gameObject.AddComponent<AudioSource>();
        m_clothesSpeaker.volume = Player.SFXVolume;
        if (m_clothesSpeaker != null)
            m_clothesSpeaker.clip = m_clothesRustleSound;

        m_rollSpeaker = gameObject.AddComponent<AudioSource>();
        m_rollSpeaker.volume = Player.SFXVolume;
        if (m_rollSpeaker != null)
            m_rollSpeaker.clip = m_rollSound;

        if (m_walkingParticleSystem != null)
            m_walkingParticleSystem.Stop();
        if (m_rollParticleSystem != null)
            m_rollParticleSystem.Stop();
        if (m_invincibilityParticle != null)
            m_invincibilityParticle.Stop();

        m_playerAnimator = GetComponentInChildren<Animator>();
        if (m_camera == null)
            m_viewCamera = Camera.main;
        else
            m_viewCamera = m_camera;
        m_nmaAcceleration = m_nma.acceleration;
        m_nmaAngledSpeed = m_nma.angularSpeed;
        m_nmaSpeed = m_nma.speed;
        StartCoroutine(CheckCombatRadius());
    }

    private void Start() {
        //m_equippedWeaponInumerator = Player.HeldWeaponLocation - 1;
        m_meleeHitBox.enabled = false;
        m_weaponController.EquippedGun.SetInfiniteAmmo(m_infiniteAmmo);
        for (int i = m_weaponController.EquippedGun.ClipSize; i > m_weaponController.EquippedGun.CurrentClip; i--) {
            m_ammoController.Shoot();
        }
        if (m_ammoFlash != null)
            StartCoroutine(FlashAmmo());
    }
    #endregion

    //----------------------------
    #region Runtime
    private void Update() {
        //Only run if the game is not paused
        if (Time.timeScale > 0 && Player.Dead == false) {
            if (m_isRolling == false) {
                //Switch Weapons
                //SwitchWeapon();

                //Player looking at mouse
                PlayerLookAt();

                //Player attacking
                if (CanAttack)
                    Attack();

                //if (m_rollCoolDownTimer <= Time.time) {
                //    m_canRoll = true;
                //    if (m_rollSpeaker != null && m_canRollSound != null)
                //        m_rollSpeaker.PlayOneShot(m_canRollSound);
                //}
            }
            if (m_invicibilityTimer <= Time.time) {
                IsInvincible = false;
                if (m_invincibilityParticle != null)
                    m_invincibilityParticle.Stop();
            }

            //Player rolling
            if (m_canRoll)
                Roll();

            //Player movement
            Move();

            //Ammo display
            DisplayAmmo();

            //Charlie
            UpdateAnims();

            //Checking if the volumes have changed and applying the change to these speakers
            UpdateVolumes();
        }
    }

    //Melee check
    //----------------------------
    private void OnTriggerStay(Collider other) {
        if (other.gameObject.layer == 1 << m_weaponController.EntityCollisionMask)
            OnHitObject(other);
    }
    #endregion

    //----------------------------
    #region Editor
    private void OnDrawGizmosSelected() {
        if (m_drawRadius)
            Gizmos.DrawWireSphere(transform.position, m_inCombatRadius);
    }
    #endregion
}
