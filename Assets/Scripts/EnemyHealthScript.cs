#pragma warning disable 618
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthScript : MonoBehaviour {

	public int maxHealth;
	public int currentHealth;

	public Image visualHealth;
	public float healthBarSpeed = 3;

	public GameObject playerCamera;

	public AudioClip gunHitEnemySound;
	public AudioSource audioSource;


	//public GameObject camera;

	// Use this for initialization
	void Start () 
	{
		maxHealth = 100;
		currentHealth = maxHealth;
		audioSource = GetComponent<AudioSource> ();
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		//playerCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ().gameObject;
	
		CheckHealth ();
		UpdateHealth ();

		transform.LookAt (playerCamera.transform);
	}

	public void UpdateHealth()
	{
		float currentValue = MapValues (currentHealth, 0, maxHealth, 0, 1);

		visualHealth.fillAmount = Mathf.Lerp (visualHealth.fillAmount, currentValue, Time.deltaTime * healthBarSpeed);

		if (currentHealth > maxHealth / 2)
		{ 
			visualHealth.color = new Color32((byte)MapValues (currentHealth, maxHealth / 2,  maxHealth, 255, 0), 255, 0, 255);
		}
		else
		{
			visualHealth.color = new Color32(255,(byte) MapValues(currentHealth, 0, maxHealth / 2, 0, 255), 0, 255);
		}

		if (currentHealth > maxHealth) 
		{
			currentHealth = maxHealth;
		}
	}

	public void DamageRPCCall()
	{
		audioSource.PlayOneShot (gunHitEnemySound, 0.2f);
		GetComponent<NetworkView> ().RPC ("TakeDamage", RPCMode.All);

	}

	[RPC]
	public void TakeDamage()
	{
		currentHealth -= 10;
	}

	public void CheckHealth()
	{
		if (currentHealth <= 0) 
		{
			Destroy (this.transform.parent.gameObject);
		}
	}

	public float MapValues(float x, float inMin, float inMax, float outMin, float outMax)
	{
		return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin; 
	}
}
