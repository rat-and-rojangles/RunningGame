using UnityEngine;
using System.Collections;

public class DieIfOffscreen : MonoBehaviour {

	private Camera mainCam;

	private GameObject player;

	[SerializeField] float margin = 0.075f; //remember this is viewport space, so it's this percent of a screen

	void Awake () {
		mainCam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();

		player = GameObject.FindGameObjectWithTag ("Player");
	}


	void FixedUpdate () {
		float xViewport = mainCam.WorldToViewportPoint (transform.position).x;
		//if(xViewport < -margin || xViewport > 1+margin){
		if (xViewport < -margin) {
			player.GetComponent<PlatformerCharacter2D> ().Die ();
		}
		/*else if (xViewport > 1 + margin) {
			Vector3 jj = new Vector3(mainCam.ViewportToWorldPoint()
			//player.GetComponent<Rigidbody2D> ().position = mainCam.ViewportToWorldPoint()
		}*/
	}
}
