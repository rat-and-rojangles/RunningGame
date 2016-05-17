using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class AutoMoveLevel : MonoBehaviour {

	[SerializeField] private Vector3 direction = Vector3.right;
	public float speed = 20.0f;
	private GameObject mainCam;

	private Animator charAnim;

	public float Speed{
		get { return speed; }
		set {
			speed = value;
			charAnim.SetFloat ("AutoMoveSpeed", Mathf.Sqrt(value)/5);
		}
	}
		

	void Awake () {
		mainCam = GameObject.FindGameObjectWithTag ("CameraController");
		charAnim = GameObject.FindGameObjectWithTag ("Player").GetComponentInChildren<Animator> ();
		charAnim.SetFloat ("AutoMoveSpeed", Mathf.Sqrt(speed)/5);
	}
	
	void LateUpdate(){		//for camera
			mainCam.transform.Translate (direction * speed * Time.deltaTime);
	}

}
