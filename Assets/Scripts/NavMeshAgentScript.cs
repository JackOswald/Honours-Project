using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class NavMeshAgentScript : MonoBehaviour {


	public Transform[] targets;
	public int destPoint = 0;
	NavMeshAgent agent;

	// Use this for initialization
	void Start () 
	{
		agent = GetComponent <NavMeshAgent> ();
		agent.autoBraking = true;
		agent.autoRepath = true;
		MoveToNextPoint ();
		agent.avoidancePriority = Random.Range (0, 99);

	}
	
	// Update is called once per frame
	void Update () 
	{
		//agent.SetDestination (target.position);	
		// && agent.remainingDistance < 0.5f
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
