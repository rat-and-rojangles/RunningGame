using UnityEngine;
using System;
using System.Collections;

public class CharacterHealth : MonoBehaviour {

	[SerializeField] private int maxHealth = 4;
	[NonSerialized] public int health;

	private const float maxUntouchableTime = 0.5f;
	private float untouchableTime = 0.0f;

	private RunnerCharacter player;

	void Awake(){
		health = maxHealth;
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<RunnerCharacter> ();
	}

	private void Update(){
		if (untouchableTime > 0.0f) {
			untouchableTime -= Time.deltaTime;
			if (untouchableTime <= 0.0f) {	//flipped
				GetComponent<Light>().enabled = false;
			}
		}
		print (health);
	}

	public void Heal(){
		// do something if not full
		// something else if full
		health = Mathf.Clamp (health + 1, 0, maxHealth);
	}
	public void Damage(){
		if (untouchableTime <= 0.0f) {	//no damage until invinciblty wears off
			if (health == 1) {
				player.Restart ();
			} else {
				health -= 1;
				untouchableTime = maxUntouchableTime;	//start invincibility
				GetComponent<Light>().enabled = true;
			}
		}
	}

	public void Refill(){
		untouchableTime = 0.0f;
		health = maxHealth;
	}


}
