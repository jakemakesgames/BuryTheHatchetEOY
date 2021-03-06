﻿using UnityEngine;
using StateMachine;
using UnityEngine.AI;
using System.Collections.Generic;

//Mark Phillips
//Created 13/08/2018
//Last edited 21/08/2018

public class FindCover : IState<AI>
{
    Vector3 m_targetLocation;
    Collider[] m_hitColliders;
    Transform m_nearestPoint;
    bool m_coverFound;
    int m_didHitCount;
    int m_highestHitCount;
    int m_numOfIfChecks = 3;


    public void Enter(AI a_owner)
    {
        m_coverFound = false;
    }

    public void Exit(AI a_owner)
    {

    }

    public void Update(AI a_owner)
    {
        if (IsCoverAvailable(a_owner))
        {
            a_owner.NoCover = false;

            if (a_owner.CurrCoverObj == null || m_coverFound == false)
            {
                FindNearestCover(a_owner);
                if (a_owner.CurrCoverObj == null)
                {
                    return;
                }
                CalcRelativeCoverPos(a_owner);
                SetPathToCover(a_owner);
            }
            else if (a_owner.Gun.GetIsEmpty() && a_owner.CurrCoverObj != null)
            {
                m_nearestPoint = a_owner.CurrCoverObj;
            }
            else if (a_owner.IsPeeking && a_owner.CurrCoverObj != null)
            {
               m_nearestPoint = a_owner.CurrCoverObj;
            }

            if (a_owner.Agent.remainingDistance >= a_owner.CoverFoundThreshold)
                CalcRelativeCoverPos(a_owner); // stop finding a new cover position when within threshold


            if (HasDestinationReached(a_owner) == false)
            {
                SetPathToCover(a_owner);
                a_owner.AtCover = false;
            }
            else
            {
                a_owner.AtCover = true;
                m_coverFound = false;
            }
        }
        else
        {
            a_owner.AtCover = false;
            a_owner.NoCover = true;
        }

    }

    private bool HasDestinationReached(AI a_owner)
    {
        if (!a_owner.Agent.pathPending)
        {
            if (a_owner.Agent.remainingDistance <= a_owner.Agent.stoppingDistance)
            {
                if (!a_owner.Agent.hasPath || a_owner.Agent.velocity.sqrMagnitude == 0f)
                    return true;
            }
        }
        return false;
    }

    private bool IsCoverAvailable(AI a_owner)
    {
        m_hitColliders = Physics.OverlapSphere(a_owner.transform.position, a_owner.CoverRadius, a_owner.CoverLayer);

        if (m_hitColliders.Length == 0)

            return false;

        else
            return true;
    }

    private void SetPathToCover(AI a_owner)
    {
        a_owner.Agent.destination = m_targetLocation;
        //a_owner.MovingToCover = true;
    }
    void FindNearestCover(AI a_owner)
    {
        List<Collider> cols = new List<Collider>();
        cols.AddRange(m_hitColliders);

        m_nearestPoint = a_owner.CurrCoverObj;

        float distToPlayer = (a_owner.transform.position - a_owner.PlayerPosition).sqrMagnitude;

        cols.Sort((a, b) => (a_owner.transform.position - a.transform.position).sqrMagnitude.CompareTo((a_owner.transform.position - b.transform.position).sqrMagnitude));
        m_highestHitCount = 0;

        for (int i = 0; i < cols.Count; i++)
        {
            m_didHitCount = 0;
            if (cols[i].transform.tag == "CoverFree")
            {
                float distMeCover = (a_owner.transform.position - cols[i].transform.position).sqrMagnitude;
                Vector3 vecMePlayer = a_owner.transform.position - a_owner.PlayerPosition;
                Vector3 vecCoverPlayer = cols[i].transform.position - a_owner.PlayerPosition;
                m_didHitCount++;

                if (distMeCover < distToPlayer) //If dist from me to cover is less than dist to player
                {
                    m_didHitCount++;
                    if (distMeCover <= Vector3.Dot(vecMePlayer, vecCoverPlayer) * Vector3.Dot(vecMePlayer, vecCoverPlayer)) //If dist from me to cover is less than the dot of dist from me to player and cover to player
                    {
                        m_didHitCount++;
                        if (a_owner.CurrCoverObj != null)
                        {
                            a_owner.CurrCoverObj.tag = "CoverFree";
                        }
                        m_nearestPoint = cols[i].transform;
                        a_owner.NextCoverObj = m_nearestPoint;
                        a_owner.NextCoverObj.tag = "CoverTaken";
                        if (a_owner.CurrCoverObj == null)
                        {
                            a_owner.CurrCoverObj = m_nearestPoint;
                        }
                        break;
                    }
                }
            }
            if (m_highestHitCount < m_didHitCount)
            {
                m_highestHitCount = m_didHitCount;
            }
        }

        //if (m_highestHitCount < m_numOfIfChecks)
        if(m_nearestPoint == a_owner.CurrCoverObj)
        {
            a_owner.NoOtherCover = true;
        }
        else
        {
            a_owner.NoOtherCover = false;
        }
        m_coverFound = true;
    }

    void CalcRelativeCoverPos(AI a_owner)
    {
        Vector3 dirFromPlayer = m_nearestPoint.position;
        dirFromPlayer = ((m_nearestPoint.position - a_owner.PlayerPosition).normalized);

        Vector3 finalPoint = (m_nearestPoint.position + dirFromPlayer);
        finalPoint = new Vector3(finalPoint.x, m_nearestPoint.position.y, finalPoint.z);

        m_targetLocation = finalPoint;
        a_owner.CoverPos = finalPoint;
    }
}
