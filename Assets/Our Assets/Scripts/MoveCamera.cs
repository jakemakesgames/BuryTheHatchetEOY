using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MoveCamera : MonoBehaviour {

    [SerializeField] private float m_speed = 10;
    private Rigidbody m_rb;
    private Vector3 m_velocity;
    private void Move(Vector3 a_velocity)
    {
        m_velocity = a_velocity;
    }
    private void Start() {
        m_rb = GetComponent<Rigidbody>();
    }

        void Update ()
    {
        Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = movement.normalized * m_speed;
        Move(moveVelocity);
    }
    private void FixedUpdate() {
        m_rb.MovePosition(m_rb.position + m_velocity * Time.fixedDeltaTime);
    }
}
