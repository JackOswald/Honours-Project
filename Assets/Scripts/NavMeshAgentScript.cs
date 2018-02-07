using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshAgentScript : MonoBehaviour {

	public Transform[] targets;
	public int destPoint = 0;
	NavMeshAgent agent;

	// Use this for initialization
	void Start () 
	{
		agent = GetComponent <NavMeshAgent> ();
		agent.autoBraking = false;
		MoveToNextPoint ();

	}
	
	// Update is called once per frame
	void Update () 
	{
		//agent.SetDestination (target.position);	
		if (!agent.pathPending && agent.remainingDistance < 0.5f)
		{
			MoveToNextPoint ();
		}
	}

	void MoveToNextPoint()
	{
		if (targets.Length == 0) 
		{
			return;
		}

		agent.destination = targets [destPoint].position;

		//destPoint = (destPoint + 1) % targets.Length;
		destPoint = Random.Range(0, targets.Length);
	}
}
