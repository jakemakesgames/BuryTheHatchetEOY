using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DrawPath : MonoBehaviour {

    LineRenderer line;
    Transform target;
    NavMeshAgent agent;

	// Use this for initialization
	void Start ()
    {
        line = GetComponent<LineRenderer>();
        agent = GetComponent<NavMeshAgent>();
        getPath();
	}

    // Update is called once per frame
    IEnumerator getPath()
    {
        line.SetPosition(0, transform.position); //set the line's origin

        agent.SetDestination(target.position); //create the path
        yield return new WaitForEndOfFrame(); //wait for the path to generate

        DrawLinePath(agent.path);

        agent.isStopped = true;//add this if you don't want to move the agent


    }
    void DrawLinePath(NavMeshPath path)
    {
        if (path.corners.Length < 2) //if the path has 1 or no corners, there is no need
            return;

        line.positionCount = path.corners.Length; //set the array of positions to the amount of corners

        for (var i = 1; i < path.corners.Length; i++)
        {
            line.SetPosition(i, path.corners[i]); //go through each corner and set that to the line renderer's position
        }
    }
}
