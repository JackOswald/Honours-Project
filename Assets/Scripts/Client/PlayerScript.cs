#pragma warning disable 618

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour {

	public int score;
	public int gunDamage;

	int currentAmmo;
	int reserveAmmo;
	const int gunCapacity = 30;

	public float fireRate = 0.5f;
	public float lastShot = 0;

	public Text currentAmmoText;

	public bool isShooting = false;
	public bool shootingLocked = false;

	public AudioClip gunFireSound;
	public AudioClip gunEmptySound;
	public AudioClip gunReloadSound;
	public AudioSource audioSource;

	public GameObject gunGameObject;

	public GameObject enemyScript;
	public EnemyHealthScript enemyHealthScript;

	public Camera playerCanmera;

	// Use this for initialization
	void Start () 
	{	
		currentAmmo = gunCapacity;	
		reserveAmmo = 300;
		shootingLocked = false;

		audioSource = GetComponent<AudioSource> ();

	}

	// Update is called once per frame
	void Update () 
	{
		Debug.DrawRay (transform.position, transform.forward, Color.green);

		currentAmmoText.text = currentAmmo.ToString() + " | " + reserveAmmo;

		if (Input.GetButton ("Fire1") && shootingLocked == false) 
		{
			if (currentAmmo > 0) 
			{
				if (Time.time > fireRate + lastShot)
				{
					Fire ();
					lastShot = Time.time;
				}
			} 
			else 
			{
				audioSource.PlayOneShot (gunEmptySound);	
			}
		}

		if (Input.GetKeyDown (KeyCode.R)) 
		{
			if (currentAmmo != gunCapacity && reserveAmmo >= 1) 
			{
				audioSource.PlayOneShot (gunReloadSound);
				gunGameObject.transform.localEulerAngles = new Vector3 (25.0f, 6.19f, 0.0f);
				gunGameObject.transform.localPosition = new Vector3 (0.84f, -1.14f, 0.8909998f);
				shootingLocked = true;
				StartCoroutine (ReloadGun ());

			}
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

		if (Input.GetKey (KeyCode.LeftShift)) 
		{
			gunGameObject.transform.localEulerAngles = new Vector3 (40.0f, -35.0f, 0.0f);
			shootingLocked = true;
		}

		if (Input.GetKeyUp (KeyCode.LeftShift)) 
		{
			gunGameObject.transform.localEulerAngles = new Vector3 (0.0f, 6.19f, 0.0f);
			shootingLocked = false;
		}
	}

	/*void FixedUpdate()
	{
		if (isShooting) 
		{
			if (Network.isServer) 
			{
				Debug.Log ("Fire");
				Fire ();
			} 
			else 
			{
				Debug.Log ("RPC FIRE");
				GetComponent<NetworkView> ().RPC ("Fire", RPCMode.Server);
			}
		}
	}*/

	void Fire()
	{
		Debug.Log ("Client fire");

		//currentAmmo--;

		GetComponent<NetworkView> ().RPC ("ServerFire", RPCMode.Server);

		audioSource.PlayOneShot (gunFireSound, 0.5f);

		//Ray ray = Camera.main.ScreenPointToRay (new Vector3 (Screen.width / 2, Screen.height / 2, 0));
		/*RaycastHit hit = new RaycastHit ();
		if (Physics.Raycast (playerCanmera.transform.position, playerCanmera.transform.forward, out hit))
		{
			if (hit.collider.gameObject.tag == "Enemy") 
			{
				//Destroy (hit.collider.transform.parent.gameObject);
				Debug.Log(hit.transform.gameObject.name);
				enemyScript = hit.collider.transform.parent.gameObject;
				enemyHealthScript = enemyScript.GetComponentInChildren<EnemyHealthScript> ();
				enemyHealthScript.TakeDamage (gunDamage);
			} 
		}*/
	}

	IEnumerator ReloadGun()
	{
		yield return new WaitForSeconds (gunReloadSound.length);
		gunGameObject.transform.localEulerAngles = new Vector3 (0.0f, 6.19f, 0.0f);
		gunGameObject.transform.localPosition = new Vector3 (0.84f, -0.92f, 0.8909998f);
		shootingLocked = false;
	}

	[RPC]
	void ServerFire()
	{
		isShooting = false;
		GetComponent<NetworkView>().RPC("UpdateAmmo", RPCMode.Server);
		Debug.Log ("Server Fire");
		RaycastHit hit = new RaycastHit ();
		if (Physics.Raycast (playerCanmera.transform.position, playerCanmera.transform.forward, out hit))
		{
			if (hit.collider.gameObject.tag == "Enemy") 
			{
				//Destroy (hit.collider.transform.parent.gameObject);
				Debug.Log(hit.transform.gameObject.name);
				enemyScript = hit.collider.transform.parent.gameObject;
				enemyHealthScript = enemyScript.GetComponentInChildren<EnemyHealthScript> ();
				enemyHealthScript.DamageRPCCall ();
			} 
		}
	}

	[RPC]
	void UpdateAmmo()
	{
		currentAmmo--;
	}

}