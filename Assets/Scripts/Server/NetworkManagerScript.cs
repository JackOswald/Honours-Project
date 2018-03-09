#pragma warning disable 618

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerScript : MonoBehaviour {

	public Text networkText;
	public GameObject serverCamera;
	public Transform playerPrefab;
	public GameObject enemyPrefab;

	public ArrayList playerScripts = new ArrayList();

	// Use this for initialization
	void Start () 
	{
		if (!Network.isServer) 
		{
			networkText.GetComponent<Text> ().text = "Client";
			serverCamera.SetActive (false);
		}
		if (Network.isServer) 
		{
			networkText.GetComponent<Text> ().text = "Server";
			serverCamera.SetActive (true);
			for (int i = 0; i < 2; i++) 
			{
				float xPos = UnityEngine.Random.Range (10.0f, 100.0f);
				float zPos = UnityEngine.Random.Range (-40.0f, -10.0f);
				Network.Instantiate (enemyPrefab, new Vector3 (xPos, 0.5f, zPos), Quaternion.identity, 0);  //new Vector3 (10.0f, 0.5f, -20.0f)
			}
		}
	}

	void OnConnectedToServer()
	{
		//Network.Instantiate (playerPrefab, new Vector3 (0.0f, 1.5f, 0.0f), Quaternion.identity, 0);
		SpawnPlayer(Network.player, true);
	}

	void OnServerInitialized()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void SpawnPlayer(NetworkPlayer player, bool isPlayer)
	{
		string tempPlayerString = player.ToString ();
		int playerNumber = Convert.ToInt32 (tempPlayerString);

		if (isPlayer) 
		{
			Transform myTransform = (Transform)Network.Instantiate (playerPrefab, 
				                       new Vector3 (0.0f, 1.5f, 0.0f), 
				                       Quaternion.identity, 
				                       playerNumber);
			playerScripts.Add (myTransform.GetComponent ("PlayerNetworkTestScript"));
			NetworkView theNetworkView = myTransform.GetComponent<NetworkView>();
			theNetworkView.RPC ("SetPlayer", RPCMode.AllBuffered, player);
		}
	}
}
