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
			Network.Instantiate (enemyPrefab, new Vector3 (10.0f, 0.5f, -20.0f), Quaternion.identity, 0);
		}
	}

	void OnConnectedToServer()
	{
		Network.Instantiate (playerPrefab, new Vector3 (0.0f, 1.5f, 0.0f), Quaternion.identity, 0);
		//SpawnPlayer(Network.player);
	}

	void OnServerInitialized()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void SpawnPlayer(NetworkPlayer player)
	{
		string tempPlayerString = player.ToString ();
		int playerNumber = Convert.ToInt32 (tempPlayerString);
		Transform myTransform =  (Transform)Network.Instantiate (playerPrefab, 
																new Vector3 (0.0f, 1.5f, 0.0f), 
																Quaternion.identity, 
																0);

		NetworkView theNetworkView = myTransform.GetComponent<NetworkView>();
		theNetworkView.RPC ("SetPlayer", RPCMode.AllBuffered, player);
	
	}
}
