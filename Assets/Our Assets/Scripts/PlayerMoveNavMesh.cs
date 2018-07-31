using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMoveNavMesh : MonoBehaviour {

    [SerializeField] private float m_speed;
    private Vector3 m_velocity;
    private Vector3 m_movement;
    private NavMeshAgent m_navMeshAgent;

    private void Awake()
    {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
    }
        private void Update() {
        //Player movement
        Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = movement.normalized * m_speed;
        m_velocity = moveVelocity;
    }

        //Anything involving the player's inputs and physics will go here
        private void FixedUpdate()
    {
        m_navMeshAgent.SetDestination(gameObject.transform.position + m_velocity * Time.fixedDeltaTime);
    }
}
