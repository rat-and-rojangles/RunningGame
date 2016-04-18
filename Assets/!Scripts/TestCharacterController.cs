using UnityEngine;
using System.Collections;

public class TestCharacterController : MonoBehaviour {

	private CharacterController controller;

	// Use this for initialization
	void Awake () {
		controller = GetComponent<CharacterController> ();
	}
	
	// Update is called once per frame
	void Update () {
		controller.Move ((Vector3.right + (Vector3.down * 5)) * 10f * Time.deltaTime);
	}
}
