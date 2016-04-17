using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class AutoMoveLevel : MonoBehaviour {

	[SerializeField] private Vector3 direction = Vector3.right;
	public float speed = 20.0f;

	private GameObject mainCam;

	void Awake () {
		mainCam = GameObject.FindGameObjectWithTag ("CameraController");
	}
	
	void LateUpdate(){		//for camera
		mainCam.transform.Translate (direction * speed * Time.deltaTime);
	}

}
