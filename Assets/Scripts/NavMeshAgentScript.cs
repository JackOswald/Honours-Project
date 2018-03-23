#pragma warning disable 618

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshAgentScript : MonoBehaviour {

	public Transform[] targets;
	public int destPoint = 0;
	NavMeshAgent agent;

	public NavMeshAgentScript script;

	public Vector3 currentPosition;
	public Quaternion currentRotation;
	public Vector3 currentVelocity;

	public Vector3 estimatedPos;

	public bool difference = false;

	public float updateInterval;

	float lasySyncTime = 0f;
	float syncDelay = 0f;
	float syncTime = 0f;
	Vector3 startPos = Vector3.zero;

	float networkTime;

	bool sendUpdates = true;

	void Awake()
	{
		//if (Network.isServer)
		//{
			script.enabled = true;
		//}

	}

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
		if (!agent.pathPending && agent.remainingDistance < 0.5f && Network.isServer)
		{
			MoveToNextPoint ();
		}
			
		updateInterval += Time.deltaTime;
		if (Network.isServer) 
		{
			GetComponent<NetworkView> ().RPC ("SyncPosition", RPCMode.Others,  
					transform.position, 
					transform.rotation, 
					GetComponent<NavMeshAgent> ().velocity, 
					(float)Network.time);
			

		}
		else
		{
			if (updateInterval > 0.1f) 
			{
				//Debug.Log ("Lerp");
				updateInterval = 0;

				float timeInterval = (networkTime - (float)Network.time);
				Debug.Log (timeInterval);
				//estimatedPos = (transform.position + GetComponent<NavMeshAgent> ().velocity * timeInterval);
				estimatedPos = currentPosition + currentVelocity * timeInterval;
				transform.position = Vector3.Lerp(transform.position, estimatedPos, timeInterval * Time.deltaTime);



				//transform.position = Vector3.Lerp (transform.position, currentPosition, 0.75f);
				//transform.rotation = Quaternion.Lerp (transform.rotation, currentRotation, 0.75f);

			}
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
		
	[RPC]
	void SyncPosition(Vector3 position, Quaternion rotation, Vector3 velocity, float netTime)
	{
		if (Network.isServer)
		{
			return;
		}

		currentPosition = position;
		currentRotation = rotation;
		currentVelocity = velocity;
		networkTime = netTime;

		//Debug.Log ("Call SyncPosition RPC");
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		Vector3 syncPosition = Vector3.zero;
		if (stream.isWriting)
		{
			syncPosition = transform.position;
			stream.Serialize (ref syncPosition);
		} 
		else 
		{
			stream.Serialize (ref syncPosition);
			syncTime = 0f;
		}
	
	}
}
