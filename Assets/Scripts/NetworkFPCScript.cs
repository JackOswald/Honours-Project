using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkFPCScript : MonoBehaviour {

	public PlayerScript playerScript;
	public UnityStandardAssets.Characters.FirstPerson.FirstPersonController controller;

	// Use this for initialization
	void Start () 
	{
		if (GetComponent<NetworkView> ().isMine) 
		{
			MonoBehaviour[] componenets = GetComponents<MonoBehaviour> ();
			foreach (MonoBehaviour m in componenets) 
			{
				Debug.Log (m);
				m.enabled = true;
			}
			foreach (Transform t in transform)
			{
				Debug.Log (t);
				t.gameObject.SetActive (true);
			}
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
