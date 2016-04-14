using UnityEngine;
using System.Collections;

public class ParallaxSky : MonoBehaviour {

	private Transform player;
	[SerializeField] private Transform skyboxCameraTransform;
	[SerializeField] private Camera levelBoundaries;

	void Awake () {
		player = GameObject.FindGameObjectWithTag ("Player").transform;
	}

	void Update () {
		//float xRatio = level.WorldToViewportPoint (transform.position).x;
		/*Quaternion newRot = skyboxCameraTransform.rotation;
		newRot.x = 360 * levelBoundaries.WorldToViewportPoint (player.position).x;
		skyboxCameraTransform.rotation = newRot;*/

		Vector3 rotEuler = skyboxCameraTransform.eulerAngles;
		rotEuler.y = 180 * levelBoundaries.WorldToViewportPoint (player.position).x;
		skyboxCameraTransform.rotation = Quaternion.Euler (rotEuler);

		//print (newRot.x);
	}
}
