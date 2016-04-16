using UnityEngine;
using System.Collections;

public class DieIfOffscreen : MonoBehaviour {

	private Camera mainCam;

	private PlatformerCharacter2D player;
	private Transform camTrans;

	[SerializeField] float margin = 0.05f; //remember this is viewport space

	void Awake () {
		mainCam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();
		camTrans = GameObject.FindGameObjectWithTag ("CameraController").transform;

		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlatformerCharacter2D> ();
	}


	void FixedUpdate () {
		float xViewport = mainCam.WorldToViewportPoint (transform.position).x;
		if(xViewport < -margin || xViewport > 1+margin){
			print ("u gone boi");
			player.Die ();
			Vector3 tempCamPos = camTrans.position;
			tempCamPos.x = player.lastCheckpoint.x;
			camTrans.position = tempCamPos;
		}
	}
}
