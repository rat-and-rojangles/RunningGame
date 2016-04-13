using UnityEngine;
using System.Collections;

public class ConstantMovement : MonoBehaviour {

	[SerializeField] Vector3 direction = Vector3.right;
	[SerializeField] float speed = 0.05f;

	private Rigidbody2D rb;

	/*IEnumerator moveNoRB(){
		while (true) {
			transform.Translate (direction * speed);

			yield return new WaitForEndOfFrame ();
			//yield return null;
		}
	}

	IEnumerator moveRB(Rigidbody2D rb){
		while (true) {
			//rb.MovePosition(transform.position + direction * speed);
			Vector3 temp = rb.position;
			temp += (direction * speed);
			rb.position = temp;

			yield return new WaitForEndOfFrame ();
			//yield return null;
		}
	}*/

	void Awake () {
		rb = GetComponent<Rigidbody2D> ();
		/*if (rb != null) {
			StartCoroutine (moveRB (rb));
		}
		else {
			StartCoroutine (moveNoRB ());
		}*/
	}

	void LateUpdate(){
		if (rb == null) {
			transform.Translate (direction * speed);
		}
	}

	void FixedUpdate(){
		if (rb != null) {
			Vector3 temp = rb.position;
			temp += (direction * speed);
			rb.position = temp;
		}
	}

}
