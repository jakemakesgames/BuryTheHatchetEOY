﻿using UnityEngine;
using StateMachine;
using UnityEngine.AI;
using System.Collections.Generic;

//Mark Phillips
//Created 13/08/2018
//Last edited 21/08/2018

public class FindCover : IState<AI>
{
    AI m_owner;
    Vector3 m_targetLocation;
    Collider[] m_hitColliders;
    bool m_initialCoverFound = false;


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
            if (m_initialCoverFound == false)
            {
                FindNearestCover();
                SetPathToCover();
            }

            if (m_owner.Agent.remainingDistance >= m_owner.CoverFoundThreshold)
                FindNearestCover(); // stop finding a new cover position when within threshold

            if (HasDestinationReached() == false)
            {
                SetPathToCover();
                m_owner.AtCover = false;
            }
            else
            {
                m_owner.AtCover = true;
                m_owner.MovingToCover = false;
            }
        }
        else
        {
            m_owner.AtCover = false;
            m_owner.MovingToCover = false;
        }

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
        m_owner.MovingToCover = true;
    }

    //private void ReloadGun()
    //{
    //    if (m_owner.Gun.ReloadOne())
    //    {
    //        m_owner.Agent.SetDestination(m_owner.transform.position);
    //        m_owner.FinishedReload = false;
    //        //Set anim bool
    //    }
    //    else
    //    {
    //        m_owner.FinishedReload = true;
    //    }
    //}

    void FindNearestCover()
    {
        List<Collider> cols = new List<Collider>();
        cols.AddRange(m_hitColliders);

        Transform nearestPoint = m_owner.transform;
        //float nearestDistance = float.MaxValue;
        //float distance;
        float distToPlayer = (m_owner.transform.position - m_owner.PlayerPosition).sqrMagnitude;



        cols.Sort((a, b) => (m_owner.transform.position - a.transform.position).sqrMagnitude.CompareTo((m_owner.transform.position - b.transform.position).sqrMagnitude));


        for (int i = 0; i < cols.Count; i++)
        {
            if ((m_owner.transform.position - cols[i].transform.position).sqrMagnitude < distToPlayer) //If dist from me to cover is less than dist to player
            {
                if ((m_owner.transform.position - cols[i].transform.position).sqrMagnitude < (cols[i].transform.position - m_owner.PlayerPosition).sqrMagnitude) //If dist from me to cover is less than dist from cover to player
                {
                    nearestPoint = cols[i].transform;
                    m_owner.CurrCoverObj = nearestPoint;
                    break;
                }
            }
        }


        Vector3 dirFromPlayer = nearestPoint.position;
        dirFromPlayer = ((nearestPoint.position - m_owner.PlayerPosition).normalized);

        Vector3 finalPoint = (nearestPoint.position + dirFromPlayer);
        finalPoint = new Vector3(finalPoint.x, nearestPoint.position.y, finalPoint.z);

        m_targetLocation = finalPoint;
        m_owner.CoverPos = finalPoint;
        m_initialCoverFound = true;
    }
}