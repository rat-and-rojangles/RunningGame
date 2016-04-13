using UnityEngine;
using System.Collections;

public class DieIfOffscreen : MonoBehaviour {

	private Camera mainCam;

	[SerializeField] float margin = 0.01f; //remember this is viewport space

	void Awake () {
		mainCam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();
	}


	void FixedUpdate () {
		float xViewport = mainCam.WorldToViewportPoint (transform.position).x;
		if(xViewport < -margin || xViewport > 1+margin){
			print ("u gone boi");
		}

		//print (xViewport);
	}
}
