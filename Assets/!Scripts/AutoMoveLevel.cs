using UnityEngine;
using System.Collections;

public class AutoMoveLevel : MonoBehaviour {

	[SerializeField] private Vector3 direction = Vector3.right;
	public float speed = 20.0f;		//the character controller gets this and handles it on its own

	private GameObject mainCam;

	void Awake () {
		mainCam = GameObject.FindGameObjectWithTag ("CameraController");
	}
	
	void LateUpdate(){		//for camera
		mainCam.transform.Translate (direction * speed * Time.deltaTime);
	}


}
