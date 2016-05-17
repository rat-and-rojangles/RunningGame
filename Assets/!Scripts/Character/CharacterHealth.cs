using UnityEngine;
using System.Collections;

public class CharacterHealth : MonoBehaviour {

	private int maxHealth = 4;
	private int health;

	private const float untouchableTime = 0.2f;

	public int Health{
		get { return health; }
	}

	public void Heal(){
		health = Mathf.Clamp (health + 1, 0, maxHealth);
	}
	public void Damage(){
		health = Mathf.Clamp (health - 1, 0, maxHealth);
	}


}
