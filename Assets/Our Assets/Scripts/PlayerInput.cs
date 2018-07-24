using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Michael Corben
//Based on Tutorial:https://www.youtube.com/watch?v=rZAnnyensgs&list=PLFt_AvWsXl0ctd4dgE1F8g3uec4zKNRV0&index=3
//Created 24/07/2018
//Last edited 25/07/2018


[RequireComponent (typeof(Rigidbody))]
public class PlayerInput : MonoBehaviour {

    [SerializeField] private float m_speed;
    [SerializeField] private float m_dashSpeed;
    private Rigidbody m_rb;
    private Vector3 m_velocity;
    private Vector3 m_movement;
    private Vector3 m_mosePos;
    private Camera m_viewCamera;
    private WeaponController m_weaponController;

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

    }

    public void Move(Vector3 a_velocity) {
        m_velocity = a_velocity;
    }

    public void LookAt(Vector3 a_lookPoint) {
        Vector3 heightCorrectedLookPoint = new Vector3(a_lookPoint.x, transform.position.y, a_lookPoint.z);
        transform.LookAt(heightCorrectedLookPoint);
    }
    public void Dash()
    {
        m_rb.AddForce(m_rb.transform.forward * m_dashSpeed);
    }
    private void Start() {
        m_rb = GetComponent<Rigidbody>();
        m_weaponController = GetComponent<WeaponController>();
        m_viewCamera = Camera.main;
    }
    // Update is called once per frame
    void Update () {
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
        if (Input.GetMouseButton(0))
        {
            m_weaponController.Shoot();
        }

    }
   //Anything involving the player's inputs and physics will go here
    public void FixedUpdate() {
        m_rb.MovePosition(m_rb.position + m_velocity * Time.fixedDeltaTime);
    }
}
