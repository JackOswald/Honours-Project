using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour {

	public int score;

	int currentAmmo;
	int reserveAmmo;
	const int gunCapacity = 30;

	public Text currentAmmoText;

	public bool isShooting = false;


	// Use this for initialization
	void Start () 
	{	
		currentAmmo = gunCapacity;	
		reserveAmmo = 300;
	}

	// Update is called once per frame
	void Update () 
	{
		currentAmmoText.text = currentAmmo.ToString() + " | " + reserveAmmo;

		if (Input.GetButton ("Fire1") && currentAmmo >0) 
		{
			isShooting = true;	
			//Debug.Log ("Firing");
		}

		if (Input.GetKeyDown (KeyCode.R)) 
		{
			int bulletRefill = gunCapacity - currentAmmo;

			if (reserveAmmo > bulletRefill) 
			{
				currentAmmo = gunCapacity;
				reserveAmmo -= bulletRefill;
			} 
			else 
			{
				currentAmmo += reserveAmmo;
				reserveAmmo = 0;
			}
		}

		CheckAmmo ();
	}

	void FixedUpdate()
	{
		if (isShooting) 
		{
			isShooting = false;

			currentAmmo--;

			Ray ray = Camera.main.ScreenPointToRay (new Vector3 (Screen.width / 2, Screen.height / 2, 0));
			RaycastHit hit = new RaycastHit ();
			if (Physics.Raycast (ray, out hit))
			{
				if (hit.collider.gameObject.tag == "Enemy") 
				{
					Debug.Log ("Hit an enemy");
					Destroy (hit.collider.gameObject);
				}
			}

		}
	}

	void CheckAmmo()
	{
		if (currentAmmo < 0) 
		{
			currentAmmo = 0;
		}
	}

}