#pragma warning disable 618

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerLatencyCorrectionScript : MonoBehaviour {

	NavMeshAgent agent;

	public GameObject enemyPrefab;
	public PlayerLatencyCorrectionScript plcScript;
	public NavMeshAgentScript navMeshScript;

	public Vector3 currentPosition;
	public Quaternion currentRotation;
	public Vector3 currentVelocity;

	public float updateInterval;


	void Awake()
	{
		if (!Network.isServer)
		{
			plcScript.enabled = true;
		} 
		else 
		{
			plcScript.enabled = false;
		}
	}

	// Use this for initialization
	void Start () 
	{
		agent = GetComponent<NavMeshAgent> ();
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Network.isServer) 
		{
			updateInterval += Time.deltaTime;
			if (updateInterval > 0.1f)
			{
				updateInterval = 0;
				GetComponent<NetworkView> ().RPC ("SyncPosition", RPCMode.Others,  transform.position, transform.rotation, GetComponent<NavMeshAgent> ().velocity);
			}
		}
		else
		{
			Debug.Log ("Lerping");
			transform.position = Vector3.Lerp (this.transform.position, currentPosition, 0.1f);
			transform.rotation = Quaternion.Lerp (this.transform.rotation, currentRotation, 0.1f);
		}

	}

	[RPC]
	void SyncPosition(Vector3 position, Quaternion rotation, Vector3 velocity)
	{
		if (Network.isServer)
		{
			//return;
		}

		currentPosition = position;
		currentRotation = rotation;
		currentVelocity = velocity;

		Debug.Log (currentVelocity);
		Debug.Log (velocity);

		Debug.Log ("Call SyncPosition RPC");
	
	}


	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		if (stream.isWriting)
		{

		}
		else 
		{

		}


	}
}
