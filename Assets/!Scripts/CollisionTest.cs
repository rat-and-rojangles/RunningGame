using UnityEngine;
using System.Collections;

public class CollisionTest : MonoBehaviour {

	private RunnerCharacter character;
	[Range(-1,1)] [SerializeField] private int direction;	//cannot be zero, left is -1, right is 1

	void Awake(){
		character = transform.parent.gameObject.GetComponent<RunnerCharacter> ();
	}

	void OnTriggerEnter(Collider other){
		print ("enter");
		character.SideTriggerCollide (direction);
	}
	void OnTriggerStay(Collider other){
		print ("stay");
	}
}
