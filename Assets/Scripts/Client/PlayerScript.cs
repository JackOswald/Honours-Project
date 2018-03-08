#pragma warning disable 618

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour {

	public int score;
	public int gunDamage;

	public int currentAmmo;
	public int reserveAmmo;
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

	void Awake()
	{
		currentAmmo = gunCapacity;	
		reserveAmmo = 300;
		shootingLocked = false;
	}

	// Use this for initialization
	void Start () 
	{	
		audioSource = GetComponent<AudioSource> ();
	}

	// Update is called once per frame
	void Update () 
	{
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

			GetComponent<NetworkView> ().RPC ("ReloadWeapon", RPCMode.Server);
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

	void Fire()
	{
		Debug.Log ("Client fire");

		GetComponent<NetworkView> ().RPC ("ServerFire", RPCMode.Server);

		audioSource.PlayOneShot (gunFireSound, 0.15f);

		//Ray ray = Camera.main.ScreenPointToRay (new Vector3 (Screen.width / 2, Screen.height / 2, 0));
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
		if (!Network.isServer) 
		{
			return;
		}
		isShooting = false;
		GetComponent<NetworkView>().RPC("UpdateAmmo", RPCMode.All);
		RaycastHit hit = new RaycastHit ();
		if (Physics.Raycast (playerCanmera.transform.position, playerCanmera.transform.forward, out hit))
		{
			if (hit.collider.gameObject.tag == "Enemy") 
			{
				//Destroy (hit.collider.transform.parent.gameObject);
				//Debug.Log(hit.transform.gameObject.name);
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

	[RPC]
	void ReloadWeapon()
	{
		if (!Network.isServer) 
		{
			return;
		}
			
		int bulletRefill = gunCapacity - currentAmmo;

		if (reserveAmmo > bulletRefill) 
		{
			GetComponent<NetworkView> ().RPC ("UpdateAmmo1", RPCMode.All, bulletRefill);
			//currentAmmo = gunCapacity;
			//reserveAmmo -= bulletRefill;
		} 
		else 
		{
			GetComponent<NetworkView> ().RPC ("UpdateAmmo2", RPCMode.All);
			//currentAmmo += reserveAmmo;
			//reserveAmmo = 0;
		}

	}

	[RPC]
	void UpdateAmmo1(int bRefil)
	{
		currentAmmo = gunCapacity;
		reserveAmmo -= bRefil;
	}

	[RPC]
	void UpdateAmmo2()
	{
		currentAmmo += reserveAmmo;
		reserveAmmo = 0;
	}

}