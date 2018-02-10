using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerUtilityScript : MonoBehaviour {

	string typeName = "JacksGame";
	string gameName = "JacksRoom"; 

	HostData[] hostList;

	string ipAddress = "192.168.0.14";

	// Use this for initialization
	void Start () 
	{
		MasterServer.ipAddress = ipAddress;
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void StartServer()
	{
		if (!Network.isServer && !Network.isClient) 
		{
			Network.InitializeServer (4, 25000, !Network.HavePublicAddress ());
			MasterServer.RegisterHost (typeName, gameName);
		}
	}

	void OnServerInitialized()
	{
		Debug.Log ("Server initialized");
	}
}
