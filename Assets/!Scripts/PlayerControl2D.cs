using UnityEngine;
using System.Collections;

public class PlayerControl2D : MonoBehaviour {

	private bool jumpInput = false;
	private float horizontalInput = 0.0f;

	public float jumpStrength = 5.0f;
	public float moveStrength = 10.0f;

	private float width;
	private float height;

	public bool grounded = false;
	public int remainingJumps = 0;


	void Awake () {
		width = GetComponent<BoxCollider2D> ().size.x;
		height = GetComponent<BoxCollider2D> ().size.y;
	}

	void Update () {
		jumpInput = (Input.GetButtonDown ("Jump") && remainingJumps >= 1 );
		horizontalInput = Input.GetAxis ("Horizontal");

		//Debug.DrawLine(bottomLeftCorner(), bottomLeftCorner() - (Vector2.up * 0.05f), Color.white, 0.0f, false);


		print ( remainingJumps );


	}

	void FixedUpdate () {
		Vector2 newVelocity = GetComponent<Rigidbody2D> ().velocity;

		if (jumpInput) {
			newVelocity.y = jumpStrength;
			remainingJumps--;
		}
			
		newVelocity.x = horizontalInput * moveStrength;

		GetComponent<Rigidbody2D> ().velocity = newVelocity;
	}

	private Vector2 bottomLeftCorner(){
		return new Vector2 (transform.position.x - width / 2, transform.position.y - height / 2);
	}
	private Vector2 bottomRightCorner(){
		return new Vector2 (transform.position.x + width / 2, transform.position.y - height / 2);
	}

	//private bool isGrounded(){ }

}
