using UnityEngine;
using System.Collections;

public class AutoMoveLevel : MonoBehaviour {

	[SerializeField] Vector3 direction = Vector3.right;
	[SerializeField] float speed = 0.05f;

	private Rigidbody2D player;
	//private Transform player;
	private GameObject mainCam;

	void Awake () {
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Rigidbody2D>();
		//player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform>();
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

		//player.MovePosition ((Vector2)player.position + (Vector2)direction * speed);

	}

}
