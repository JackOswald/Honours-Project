#pragma warning disable 618

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkManagerScript : MonoBehaviour {

	public PlayerScript playerScript;
	public UnityStandardAssets.Characters.FirstPerson.FirstPersonController controller;

	public float dist;

	// Move commands sent to the server
	public struct move
	{
		public float HorizontalAxis;
		public float VerticalAxis;
		public double TimeStamp;

		public move (float horiz, float vert, double timestamp)
		{
			this.HorizontalAxis = horiz;
			this.VerticalAxis = vert;
			this.TimeStamp = timestamp;
		}
	}
		

	// History of move commands sent from the client to the server
	public List<move> moveHistory = new List<move>();


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
		
	}

	void FixedUpdate()
	{
		if (GetComponent<NetworkView> ().isMine)
		{	
			move moveState = new move (controller.horizontal, controller.vertical, Network.time);
			moveHistory.Insert (0, moveState);
			if (moveHistory.Count > 50) 
			{
				moveHistory.RemoveAt (moveHistory.Count - 1);
			}

			controller.Simulate ();
			 
			// Send current state to the server
			GetComponent<NetworkView> ().RPC ("ProcessInput", 
				RPCMode.Server, 
				moveState.HorizontalAxis, 
				moveState.VerticalAxis, 
				this.transform.position);
		}

		dist = Vector3.Distance (controller.currentPos, this.transform.position);
	}

	[RPC]
	void ProcessInput(float horizAxis, float vertAxis, Vector3 position, NetworkMessageInfo info)
	{
		if (GetComponent<NetworkView>().isMine)
		{
			return;
		}

		if (!Network.isServer)
		{
			return;
		}

		Debug.Log ("Process Inputs");
		controller.horizontal = horizAxis;
		controller.vertical = vertAxis;
		controller.Simulate();

		// Compare position results
		if (Vector3.Distance (controller.currentPos, position) > 0.001f)  // > 0.1f
		{
			GetComponent<NetworkView> ().RPC ("CorrectState", info.sender, transform.position);
		}
	}

	[RPC]
	void CorrectState(Vector3 correctPos, NetworkMessageInfo info)
	{
		Debug.Log ("Correct State");
		int pastState = 0;

		for (int i = 0; i < moveHistory.Count; i++)
		{
			if (moveHistory [i].TimeStamp <= info.timestamp) 
			{
				pastState = i;
				break;
			}
		}

		// Rewind state
		this.transform.position = correctPos;

		for (int i = pastState; i >= 0; i--) 
		{
			controller.horizontal = moveHistory [i].HorizontalAxis;
			controller.vertical = moveHistory [i].VerticalAxis;
			controller.Simulate ();
		}

		moveHistory.Clear ();
	}

	/*void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		Vector3 position = Vector3.zero;

		if (stream.isWriting) 
		{
			Debug.Log ("Writing");
			position = transform.position;
			stream.Serialize (ref position);
		} 
		else 
		{
			Debug.Log ("Else");
			stream.Serialize (ref position);
			transform.position = position;
		}
	}*/
}
