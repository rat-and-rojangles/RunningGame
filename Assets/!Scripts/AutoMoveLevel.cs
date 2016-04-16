using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class AutoMoveLevel : MonoBehaviour {

	[SerializeField] private Vector3 direction = Vector3.right;
	[SerializeField] private float speed = 20.0f;

	private Rigidbody2D playerRB;
	private GameObject mainCam;

	void Awake () {
		/*GameObject[] remaining = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject clone in remaining) {
			//if(clone.name.Equals( "CharacterRobotBoy(Clone)")){
			if(clone.name.Contains("Clone")){
				GameObject.Destroy(clone);
			}
		}*/

		playerRB = GameObject.FindGameObjectWithTag ("Player").GetComponent<Rigidbody2D>();
		mainCam = GameObject.FindGameObjectWithTag ("CameraController");
	}
	
	void LateUpdate(){		//for camera
		mainCam.transform.Translate (direction * speed * Time.deltaTime);
	}

	void FixedUpdate(){		//for player (not FixedUpdate so it's once per frame)
		Vector3 temp = playerRB.position;
		temp += (direction * speed * Time.fixedDeltaTime);
		playerRB.position = temp;


		//playerRB.MovePosition ((Vector2)playerRB.position + (Vector2)direction * speed);

	}

}
