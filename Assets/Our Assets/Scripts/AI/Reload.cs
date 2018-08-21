using UnityEngine;
using StateMachine;
using UnityEngine.AI;

//Mark Phillips
//Created 13/08/2018
//Last edited 21/08/2018

public class Reload : IState<AI>
{
    AI m_owner;
    Vector3 m_targetLocation;
    Collider[] m_hitColliders;
    bool m_coverFound = false;


    public void Enter(AI a_owner)
    {
        m_owner = a_owner;
    }

    public void Exit(AI a_owner)
    {
    }

    public void Update(AI a_owner)
    {
        if (IsCoverAvailable())
        {
            if (m_coverFound == false)
            {
                FindNearestCover();
                SetPathToCover();
            }

            if (m_owner.Agent.remainingDistance >= m_owner.CoverFoundThreshold)
                FindNearestCover(); // stop finding a new cover position when within threshold

            if (HasDestinationReached() == false)
                SetPathToCover();
            else
            {
                ReloadGun();
            }
        }
        else
            ReloadGun();
        
    }

    private bool HasDestinationReached()
    {
        if (!m_owner.Agent.pathPending)
        {
            if (m_owner.Agent.remainingDistance <= m_owner.Agent.stoppingDistance)
            {
                if (!m_owner.Agent.hasPath || m_owner.Agent.velocity.sqrMagnitude == 0f)
                    return true;
            }
        }
        return false;
    }

    private bool IsCoverAvailable()
    {
        m_hitColliders = Physics.OverlapSphere(m_owner.transform.position, m_owner.CoverRadius, m_owner.CoverLayer);

        if (m_hitColliders.Length == 0)

            return false;

        else
            return true;
    }

    private void SetPathToCover()
    {
        m_owner.Agent.destination = m_targetLocation;
    }

    private void ReloadGun()
    {
        m_owner.Agent.SetDestination(m_owner.transform.position);
        m_owner.Gun.Reload();
    }

    void FindNearestCover()
    {
        Transform nearestPoint = m_hitColliders[0].transform;
        float nearestDistance = float.MaxValue;
        float distance;

        for (int i = 0; i < m_hitColliders.Length; i++)
        {
            distance = (m_owner.transform.position - m_hitColliders[i].transform.position).sqrMagnitude;

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestPoint = m_hitColliders[i].transform;
            }
        }

        Vector3 dirFromPlayer = nearestPoint.position;
        dirFromPlayer = ((nearestPoint.position - m_owner.PlayerPosition).normalized);

        Vector3 finalPoint = (nearestPoint.position + dirFromPlayer);
        finalPoint = new Vector3(finalPoint.x, nearestPoint.position.y, finalPoint.z);

        m_targetLocation = finalPoint;
        m_coverFound = true;
    }
}
