using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeTestScript : MonoBehaviour {

	public float moveSpeed = 5.0f;
	public float horizAxis = 0f;
	public float vertAxis = 0f;
	private CharacterController controller;

	// Use this for initialization
	void Start () 
	{
		//if (Network.isServer) 
		//{
			controller = GetComponent <CharacterController>();
		//}
	}

	// Update is called once per frame
	void Update () 
	{
		horizAxis = Input.GetAxis ("Horizontal");
		vertAxis = Input.GetAxis ("Vertical");

		controller.Move (new Vector3 (vertAxis * moveSpeed * Time.deltaTime, 0, horizAxis * moveSpeed * Time.deltaTime));


	}
}
