#pragma warning disable 618

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkManagerScript : MonoBehaviour {

	public PlayerScript playerScript;
	public UnityStandardAssets.Characters.FirstPerson.FirstPersonController controller;

	public NetworkPlayer theOwner;
	public float lastClientHInput = 0f;
	public float lastClientVInput = 0f;
	public float serverCurrentHInput = 0f;
	public float serverCurrentVInput = 0f;
	//public float HInput;
	//public float VInput;
	public Vector3 moveDir = Vector3.zero; 

	void Awake()
	{
		//if (Network.isClient)
		//{
		//	enabled = false;
		//}
	}

	// Use this for initialization
	void Start () 
	{
		if (GetComponent<NetworkView> ().isMine) 
		{
			controller.enabled = true;
			playerScript.enabled = true;
			MonoBehaviour[] componenets = GetComponents<MonoBehaviour> ();
			foreach (MonoBehaviour m in componenets) 
			{
				m.enabled = true;
			}
			foreach (Transform t in transform)
			{
				t.gameObject.SetActive (true);
			}
		}
		
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if (theOwner != null && Network.player == theOwner) 
		{
			float HInput = controller.horizontal;
			float VInput = controller.vertical;

			controller.Simulate ();

			if (lastClientHInput != HInput || lastClientVInput != VInput) 
			{
				lastClientHInput = HInput;
				lastClientVInput = VInput;

				if (Network.isServer) 
				{
					SendMovementInput (HInput, VInput);
				} 
				else if (!Network.isServer)
				{
					GetComponent<NetworkView> ().RPC ("SendMovementInput", RPCMode.Server, HInput, VInput);
				}
			}
		}
			
		if (Network.isServer) 
		{
			//Debug.Log ("Correct player state");
			moveDir = new Vector3 (serverCurrentHInput, 0, serverCurrentVInput);
			controller.desiredMove = moveDir;
			//transform.Translate(5 * moveDir * Time.deltaTime);
			//Debug.Log (moveDir);
		}
	}

	[RPC]
	void SetPlayer(NetworkPlayer player)
	{
		theOwner = player;
		if (player == Network.player) 
		{
			enabled = true;
		}
	}

	[RPC]
	void SendMovementInput(float HorizInput, float VertInput)
	{
		serverCurrentHInput = HorizInput;
		serverCurrentVInput = VertInput;
	}

	void OnSerializeNetworkView (BitStream stream, NetworkMessageInfo info)
	{
		if (stream.isWriting) 
		{
			Vector3 pos = transform.position;
			stream.Serialize (ref pos);
		} 
		else 
		{
			Vector3 posRecieve = Vector3.zero;
			stream.Serialize (ref posRecieve);
			transform.position = posRecieve;
		}
	}

}
