using UnityEngine;
using StateMachine;

//Mark Phillips
//Created 13/08/2018
//Last edited 15/08/2018

public class Reload : IState<AI>
{
    AI m_owner;
    Vector3 m_targetLocation;
    Collider[] m_hitColliders;

    public void Enter(AI a_owner)
    {
        Debug.Log("Entering Reload");
        m_owner = a_owner;
    }

    public void Exit(AI a_owner)
    {
        Debug.Log("Exiting Reload");
    }

    public void Update(AI a_owner)
    {
        if (IsCoverAvailable())
        {
            FindNearestCover(); // stop finding cover when within threshold

            if (HasDestinationReached() == false)
            {
                MoveToCover();
            }
            else
            {
                m_owner.Gun.Reload();
            }
        }
        else
        {
            m_owner.Gun.Reload();
        }
    }

    private bool HasDestinationReached()
    {
        if (!m_owner.GetAgent().pathPending)
        {
            if (m_owner.GetAgent().remainingDistance <= m_owner.GetAgent().stoppingDistance)
            {
                if (!m_owner.GetAgent().hasPath || m_owner.GetAgent().velocity.sqrMagnitude == 0f)
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

    private void MoveToCover()
    {
        //if (!m_targetFound)
        {
           // Collider[] hitColliders = Physics.OverlapSphere(m_owner.transform.position, m_owner.CoverRadius, m_owner.CoverLayer);
           //
           // if (hitColliders.Length == 0)
           // {
           //     m_owner.Gun.Reload();
           //     m_coverFound = true;
           //     return;
           // }
           //for (int i = 0; i < hitColliders.Length; i++)
           //{
           //    m_owner.CoverPointsm.Add(hitColliders[i].transform);
           //}

            m_targetLocation = FindNearestCover();
            agent.destination = m_targetLocation;
            m_targetFound = true;
        }

        if (agent.remainingDistance >= m_coverFoundThreshold)
        {
            m_targetLocation = FindNearestCover();
            agent.destination = m_targetLocation;
        }

        // Check if we've reached the destination
        if (HasDestinationReached())
        {
            if (!m_gun.m_isReloading && m_gun.GetIsEmpty())
            {
                m_gun.Reload();
            }
            m_coverFound = true;
            m_targetFound = false;
        }

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

        //foreach (Transform coverPoint in m_coverPoints)
        //{
        //    //distance = Vector3.Distance(transform.position, coverPoint.position);
        //    distance = (transform.position - coverPoint.position).sqrMagnitude;
        //
        //    if (distance < nearestDistance)
        //    {
        //        nearestDistance = distance;
        //        nearestPoint = coverPoint;
        //    }
        //}

        Vector3 dirFromPlayer = nearestPoint.position;
        dirFromPlayer = ((nearestPoint.position - m_owner.PlayerPosition).normalized);

        Vector3 finalPoint = (nearestPoint.position + dirFromPlayer);
        finalPoint = new Vector3(finalPoint.x, nearestPoint.position.y, finalPoint.z);

        m_targetLocation =  finalPoint;
    }
}
