using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ServerUtilityScript : MonoBehaviour {

	string typeName = "JacksGame";
	string gameName = "JacksRoom"; 

	HostData[] hostList;

	public string ipAddress = "192.168.0.14"; //192.168.0.14

	public InputField ipInputField;

	// Use this for initialization
	void Start () 
	{
		MasterServer.ipAddress = ipAddress;
	}
	
	// Update is called once per frame
	void Update () 
	{
		//ipAddress = ipInputField.GetComponent<InputField>().text;
	}

	public void StartServer()
	{
		if (!Network.isServer && !Network.isClient) 
		{
			Network.InitializeServer (4, 25000, !Network.HavePublicAddress ());
			MasterServer.RegisterHost (typeName, gameName);
			//SceneManager.LoadScene ("Level 1");
			NetworkLevelLoader.Instance.LoadLevel("Level 1");
		}
	}

	void OnServerInitialized()
	{
		Debug.Log ("Server initialized");
	}

	public void RefreshHostList()
	{
		MasterServer.RequestHostList (typeName);
	}

	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
		{
			hostList = MasterServer.PollHostList ();
			foreach (HostData hd in hostList) 
			{
				if (hd.gameName == gameName)
				{
					Network.Connect (hd);
					NetworkLevelLoader.Instance.LoadLevel("Level 1");
				}
			}
		}
	}

	void OnConnectedToServer()
	{
		Debug.Log ("Server joined");
	}
}
