using UnityEngine;
using System.Collections;

public class GroundCheck : MonoBehaviour {

	private PlayerControl2D player;

	void Awake () {
		player = GetComponentInParent<PlayerControl2D> ();
	}

	void OnTriggerEnter2D(Collider2D col){
		player.grounded = true;
		player.remainingJumps = 2;
	}

	void OnTriggerStay2D(Collider2D col){
		player.grounded = true;
	}

	void OnTriggerExit2D(Collider2D col){
		player.grounded = false;
	}
}
