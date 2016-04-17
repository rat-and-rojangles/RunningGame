using UnityEngine;
using System.Collections;

public class MoveTheWholeDamnLevel : MonoBehaviour {

	[SerializeField] private Vector3 direction = Vector3.right;
	[SerializeField] private float speed = 20.0f;

	void LateUpdate(){		//for camera
		transform.Translate (-direction * speed * Time.deltaTime);
	}
}
