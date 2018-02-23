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

	//public GameObject camera;

	// Use this for initialization
	void Start () 
	{
		maxHealth = 100;
		currentHealth = maxHealth;
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		//playerCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ().gameObject;
	
		CheckHealth ();
		UpdateHealth ();

		//transform.LookAt (playerCamera.transform);
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

	public void TakeDamage(int amount)
	{
		currentHealth -= amount;
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
