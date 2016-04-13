using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class AutoMoveLevel : MonoBehaviour {

	[SerializeField] Vector3 direction = Vector3.right;
	[SerializeField] float speed = 0.05f;

	private Rigidbody2D player;
	private GameObject mainCam;

	void Awake () {
		/*GameObject[] remaining = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject clone in remaining) {
			//if(clone.name.Equals( "CharacterRobotBoy(Clone)")){
			if(clone.name.Contains("Clone")){
				GameObject.Destroy(clone);
			}
		}*/

		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Rigidbody2D>();
		mainCam = GameObject.FindGameObjectWithTag ("CameraController");
	}
	
	void LateUpdate(){		//for camera
		mainCam.transform.Translate (direction * speed * Time.deltaTime);
		//player.MovePosition ((Vector2)player.position + (Vector2)direction * speed);
	}

	void FixedUpdate(){		//for player (not FixedUpdate so it's once per frame)
		Vector3 temp = player.position;
		temp += (direction * speed * Time.fixedDeltaTime);
		player.position = temp;


		print (temp.x);

		//player.MovePosition ((Vector2)player.position + (Vector2)direction * speed);

	}

}
