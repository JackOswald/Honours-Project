using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerScript : MonoBehaviour {

	public Text networkText;
	public GameObject serverCamera;
	public GameObject playerPrefab;

	// Use this for initialization
	void Start () 
	{
		//if (!Network.isServer) 
		//{
			//networkText.GetComponent<Text> ().text = "Client";
			//serverCamera.SetActive (false);
		//}
		if (Network.isServer) 
		{
			networkText.GetComponent<Text> ().text = "Server";
			serverCamera.SetActive (true);
		}
		else
		{
			networkText.GetComponent<Text> ().text = "Client";
			serverCamera.SetActive (false);
			//Network.Instantiate (playerPrefab, new Vector3 (0.0f, 1.5f, 0.0f), Quaternion.identity, 0);

		}
	}

	void OnConnectedToServer()
	{
		//networkText.GetComponent<Text> ().text = "Client";
		//serverCamera.SetActive (false);
		Network.Instantiate (playerPrefab, new Vector3 (0.0f, 1.5f, 0.0f), Quaternion.identity, 0);
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
}
