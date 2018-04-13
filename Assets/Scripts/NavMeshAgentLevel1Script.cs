#pragma warning disable 618

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshAgentLevel1Script : MonoBehaviour {

	public Transform[] targets;
	public int destPoint = 0;
	NavMeshAgent agent;

	public NavMeshAgentLevel1Script script;

	public Vector3 currentPosition;
	public Quaternion currentRotation;
	public Vector3 currentVelocity;

	public bool difference = false;

	public float updateInterval;

	float lastSyncTime = 0f;
	float syncDelay = 0f;
	float syncTime = 0f;
	Vector3 startPos = Vector3.zero;
	Vector3 endPos = Vector3.zero;
	Quaternion startRot;

	public Vector3 estimatedPos;
	public Vector3 estimatedPos1;
	public Vector3 estimatedPos2;

	//float networkTime;

	//bool sendUpdates = true;

	void Awake()
	{
		//if (Network.isServer)
		//{
			script.enabled = true;
		//}

		//GetComponent<NetworkView>().observed = this;
		//GetComponent<NetworkView> ().stateSynchronization = NetworkStateSynchronization.Unreliable;

	}

	// Use this for initialization
	void Start () 
	{
		agent = GetComponent <NavMeshAgent> ();
		agent.autoBraking = true;
		agent.autoRepath = true;
		MoveToNextPoint ();
		agent.avoidancePriority = Random.Range (0, 99);
		targets = GameObject.FindGameObjectWithTag ("NavMesh Objects").GetComponentsInChildren<Transform> ();

	}
	
	// Update is called once per frame
	void Update () 
	{

		//agent.SetDestination (target.position);	
		// && agent.remainingDistance < 0.5f
		if (!agent.pathPending && agent.remainingDistance < 0.5f && Network.isServer)
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
		
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		Vector3 syncPosition = Vector3.zero;
		if (stream.isWriting)
		{
			//Debug.Log ("Server");
			syncPosition = transform.position;
			stream.Serialize (ref syncPosition);
			currentVelocity = GetComponent<NavMeshAgent> ().velocity;
			stream.Serialize (ref currentVelocity);
			currentRotation = transform.rotation;
			stream.Serialize (ref currentRotation);
			Debug.Log (syncPosition);
			Debug.Log (currentVelocity);
			Debug.Log (currentRotation);
		} 
		else 
		{
			//Debug.Log ("Client");
			stream.Serialize (ref syncPosition);
			stream.Serialize (ref currentVelocity);
			stream.Serialize (ref currentRotation);
			//Debug.Log (syncPosition);
			//Debug.Log (currentVelocity);
			//Debug.Log (currentRotation);
			syncTime = 0f;
			syncDelay = Time.time - lastSyncTime;
			lastSyncTime = Time.time;
			startPos = transform.position;
			startRot = transform.rotation;
			endPos = syncPosition;
			//transform.position = endPos;
		}

	}
}
