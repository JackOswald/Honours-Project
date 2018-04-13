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

	List<string> levels = new List<string>() {"Level 1", "Level 2", "Level 3"};

	public Dropdown dropdown;
	public Text dropdownText;
	public Button startServerButton;
	public Button joinServerButton;

	public bool level1Selected = false;
	public bool level2Selected = false;
	public bool level3Selected = false;

	// Use this for initialization
	void Start () 
	{
		AddToList ();
		MasterServer.ipAddress = ipAddress;
	}
	
	// Update is called once per frame
	void Update () 
	{
		//ipAddress = ipInputField.GetComponent<InputField>().text;
		if (!Network.isServer) 
		{
			//startServerButton.GetComponent<Button> ().interactable = false;
			//dropdown.interactable = false;
		}

	}

	public void DropDownIndexChanged(int index)
	{
		dropdownText.text = levels [index] + " selected";
		if (index == 0) 
		{
			level1Selected = true;
			level2Selected = false;
			level3Selected = false;
		}
		else if (index == 1) 
		{
			level1Selected = false;
			level2Selected = true;
			level3Selected = false;
		} 
		else if (index == 2) 
		{
			level1Selected = false;
			level2Selected = false;
			level3Selected = true;
		}
	}

	public void StartServer()
	{
		if (!Network.isServer && !Network.isClient) 
		{
			Network.InitializeServer (4, 25000, !Network.HavePublicAddress ());
			MasterServer.RegisterHost (typeName, gameName);
			//SceneManager.LoadScene ("Level 1");
			if (level1Selected) 
			{
				NetworkLevelLoader.Instance.LoadLevel ("Level 1");
			}
			else if (level2Selected) 
			{
				NetworkLevelLoader.Instance.LoadLevel ("Level 2");
			}
			else if (level3Selected) 
			{
				NetworkLevelLoader.Instance.LoadLevel ("Level 3");
			}
		}
	}

	void AddToList()
	{
		dropdown.AddOptions (levels);
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
					if (level1Selected) 
					{
						NetworkLevelLoader.Instance.LoadLevel ("Level 1");
					}
					else if (level2Selected) 
					{
						NetworkLevelLoader.Instance.LoadLevel ("Level 2");
					}
					else if (level3Selected) 
					{
						NetworkLevelLoader.Instance.LoadLevel ("Level 3");
					}
				}
			}
		}
	}

	void OnConnectedToServer()
	{
		Debug.Log ("Server joined");
	}
}
