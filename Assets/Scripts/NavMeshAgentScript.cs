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
		agent.autoBraking = true;
		agent.autoRepath = true;
		MoveToNextPoint ();
		agent.avoidancePriority = Random.Range (0, 99);

		//targets = gameObject.GetComponentsInChildren<Transform> ();
		//target = new GameObject[targets.Length];

		//foreach (Transform t in targets)
		//{
		//	value++;
		//	target.SetValue (t.gameObject, value - 1);
		//}

		targets = GameObject.FindGameObjectWithTag ("NavMesh Objects").GetComponentsInChildren<Transform> ();

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

		agent.destination = targets [destPoint].transform.position;

		//destPoint = (destPoint + 1) % targets.Length;
		destPoint = Random.Range(0, targets.Length);
	}
}
