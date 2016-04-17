using UnityEngine;
using System.Collections;

public class MoveTheWholeDamnLevel : MonoBehaviour {

	[SerializeField] private Vector3 direction = Vector3.right;
	[SerializeField] private float speed = 20.0f;

	/*private Rigidbody2D[] allBodies;

	void Awake(){
		allBodies = GetComponentsInChildren<Rigidbody2D> ();
	}

	void FixedUpdate(){
		//transform.Translate (-direction * speed * Time.deltaTime);
		foreach(Rigidbody2D r in allBodies){
			r.position -= (Vector2)direction * speed * Time.fixedDeltaTime;
		}
	}*/

	void LateUpdate(){
		transform.Translate (-direction * speed * Time.deltaTime);
	}
}
