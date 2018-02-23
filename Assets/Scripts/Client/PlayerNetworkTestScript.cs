#pragma warning disable 618

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkTestScript : MonoBehaviour {

	public PlayerScript playerScript;
	public UnityStandardAssets.Characters.FirstPerson.FirstPersonController controller;

	public NetworkPlayer theOwner;
	float lastClientHInput = 0f;
	float lastClietnVInput = 0f;
	float serverCurrentHInput = 0f;
	float serverCurrentVInput = 0f;

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
	void Update () 
	{
		if (theOwner != null && Network.player == theOwner) 
		{
			float HInput = controller.horizontal;
			float VInput = controller.vertical;

			if (lastClientHInput != HInput || lastClietnVInput != VInput) 
			{
				lastClientHInput = HInput;
				lastClietnVInput = VInput;

				if (Network.isServer) 
				{
					SendMovementInput (HInput, VInput);
				} 
				else if (!Network.isServer)
				{
					GetComponent<NetworkView> ().RPC ("SendMovementInput", RPCMode.Server, HInput, VInput);
					controller.Simulate ();
				}
			}

			if (Network.isServer) 
			{
				Vector3 moveDir = new Vector3 (serverCurrentHInput, 0, serverCurrentVInput);
				controller.m_MoveDir = moveDir;

			}
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
	void SendMovementInput(float HInput, float VInput)
	{
		serverCurrentHInput = HInput;
		serverCurrentVInput = VInput;
	}

}
