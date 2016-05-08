using UnityEngine;
using System.Collections;

public class ForwardCrash : MonoBehaviour {

	private RunnerCharacter character;

	void Awake(){
		character = transform.parent.gameObject.GetComponent<RunnerCharacter> ();
	}

	void OnTriggerEnter(Collider other){
		character.Crash ();
	}
}
