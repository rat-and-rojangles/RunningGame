using UnityEngine;
using System.Collections;

public class DieIfOffscreen : MonoBehaviour {

	private Camera mainCam;

	private PlatformerCharacter2D player;

	[SerializeField] float margin = 0.15f; //remember this is viewport space, so it's this percent of a screen

	void Awake () {
		mainCam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();

		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlatformerCharacter2D> ();
	}


	void FixedUpdate () {
		float xViewport = mainCam.WorldToViewportPoint (transform.position).x;
		if(xViewport < -margin || xViewport > 1+margin){
			print ("u gone boi");
			player.Die ();

		}
	}
}
