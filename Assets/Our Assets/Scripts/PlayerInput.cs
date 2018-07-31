using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
//Michael Corben
//Based on Tutorial:https://www.youtube.com/watch?v=rZAnnyensgs&list=PLFt_AvWsXl0ctd4dgE1F8g3uec4zKNRV0&index=3
//Created 24/07/2018
//Last edited 31/07/2018


[RequireComponent (typeof(NavMeshAgent))]
[RequireComponent(typeof(WeaponController))]
public class PlayerInput : MonoBehaviour {

    [SerializeField] private float m_speed;
    [SerializeField] private float m_dashSpeed;
    private NavMeshAgent m_nma;
    private Vector3 m_velocity;
    private Vector3 m_movement;
    private Vector3 m_mosePos;
    private Camera m_viewCamera;
    private WeaponController m_weaponController;

    [SerializeField] private Text m_clipAmmoDisplay;
    [SerializeField] private Text m_totalAmmoDisplay;

    public float Speed
    {
        get
        {
            return m_speed;
        }

        set
        {
            m_speed = value;
        }
    }
    public float DashSpeed
    {
        get
        {
            return m_dashSpeed;
        }

        set
        {
            m_dashSpeed = value;
        }
    }

    public void Attack()
    {
        if (m_weaponController.GetIsAuto()) {
            if (Input.GetMouseButton(0)) {
                m_weaponController.Shoot();
            }
            if (m_weaponController.GetIsGunEmpty()) {
                if (Input.GetMouseButtonDown(0)) {
                    m_weaponController.ReloadEquippedGun();
                }
            }
        }
        else {
            if (Input.GetMouseButtonDown(0)) {
                if (m_weaponController.GetIsGunEmpty()) {
                    m_weaponController.ReloadEquippedGun();
                }
                m_weaponController.Shoot();
            }
        }
    }

    private void Move(Vector3 a_velocity) {
        m_velocity = a_velocity;
    }

    private void LookAt(Vector3 a_lookPoint) {
        Vector3 heightCorrectedLookPoint = new Vector3(a_lookPoint.x, transform.position.y, a_lookPoint.z);
        transform.LookAt(heightCorrectedLookPoint);
    }
    private void Dash() {
        Vector3 movement = new Vector3(transform.forward.x, 0, transform.forward.z);
        Vector3 moveVelocity = movement.normalized * m_dashSpeed;
        Move(moveVelocity);
    }
    private void Awake() {
        m_nma = GetComponent<NavMeshAgent>();
        m_weaponController = GetComponent<WeaponController>();
        m_viewCamera = Camera.main;
    }

    private void Update () {
        //Player movement
        Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = movement.normalized * m_speed;
        Move(moveVelocity);

        //Player looking at mouse
        Ray ray = m_viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance)) {
            Vector3 pointOnGround = ray.GetPoint(rayDistance);
            LookAt(pointOnGround);
        }

        //Player attacking
        Attack();

        //Player dashing
        if (Input.GetMouseButtonDown(1)) {
            Dash();
        }
        //Ammo display
        if (m_clipAmmoDisplay != null) {
            m_clipAmmoDisplay.GetComponent<Text>().text = 
                (m_weaponController.GetCurrentClip().ToString() + " / " + m_weaponController.GetEquippedGun().m_clipSize.ToString());
        }

        if (m_totalAmmoDisplay != null) {
            m_totalAmmoDisplay.GetComponent<Text>().text =
                (m_weaponController.GetCurrentAmmo().ToString() + " / " + m_weaponController.GetEquippedGun().m_maxAmmo.ToString());
        }
    }
    //Anything involving the player's inputs and physics will go here
    private void FixedUpdate() {
        m_nma.SetDestination(transform.position + m_velocity * Time.fixedDeltaTime);
    }
}
