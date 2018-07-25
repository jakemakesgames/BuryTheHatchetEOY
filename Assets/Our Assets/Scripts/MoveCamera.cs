using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour {

    [SerializeField] private float m_speed = 10;
    private Vector3 m_velocity;
    private void Move(Vector3 a_velocity)
    {
        m_velocity = a_velocity;
    }

    // Update is called once per frame
    void Update ()
    {
        //Player movement
        Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
        Vector3 moveVelocity = movement.normalized * m_speed;
        Move(moveVelocity);
    }
    private void FixedUpdate()
    {
        transform.Translate(m_velocity * Time.fixedDeltaTime);
    }
}
