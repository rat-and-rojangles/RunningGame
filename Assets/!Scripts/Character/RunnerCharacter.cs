using System;
using UnityEngine;

using System.Collections;

public class RunnerCharacter : MonoBehaviour
{
    [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
	[SerializeField] private float m_JumpVelocity = 15f;                // Upward velocity when the player jumps.
    //[SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

	private bool m_Grounded;			// Whether or not the player is grounded.
    private Animator m_Anim;			// Reference to the player's animator component.
	private Rigidbody m_Rigidbody;
	private CapsuleCollider m_Capsule;

	const int k_MaxJumps = 1;
	private int m_RemainingJumps = 500;

	private AutoMoveLevel aml;
	private Vector3 lastCheckpoint;
	private Transform m_CamOffset;

	private bool groundedThisFrame = true;		//used for smoothing the animator

	[SerializeField] private float m_GravityStrength = 1500.0f;
	private Vector3 m_PersonalGravity;

	private bool sidestepMode = false;
	private int rail = 0;					// 1 is left, -1 is right
	private const float k_RailWidth = 5.0f;

	private bool canChangeRail = true;

	public Transform perfectPosition;

	//coroutines
	private IEnumerator camSidestep;
	private IEnumerator cam2D;

    private void Awake()
    {
        // Setting up references.
		m_Anim = GetComponentInChildren<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
		m_Capsule = GetComponent<CapsuleCollider>();

		aml = GameObject.FindGameObjectWithTag ("GameController").GetComponent<AutoMoveLevel> ();
		m_CamOffset = GameObject.FindGameObjectWithTag ("CamOffset").transform;

		lastCheckpoint = transform.position;	//first checkpoint is start

		m_Grounded = true;
		m_Rigidbody.useGravity = false;
		m_PersonalGravity = Vector3.down * m_GravityStrength;

		camSidestep = CameraSidestepAngle ();
		cam2D = Camera2DAngle ();
    }


    private void FixedUpdate()
    {
		/*if (Input.GetKey (KeyCode.Backspace)) {								//floating cheat
			m_Rigidbody.AddForce (-2 * m_PersonalGravity * Time.fixedDeltaTime);	
		}*/

        // Set the vertical animation
        m_Anim.SetFloat("vSpeed", m_Rigidbody.velocity.y);

		m_Rigidbody.AddForce (m_PersonalGravity * Time.fixedDeltaTime);		//gravity

		if (sidestepMode) {
			//Forward backward stuff
			if (transform.position.x > perfectPosition.position.x) {
				m_Rigidbody.AddForce (Vector3.left * (transform.position.x - perfectPosition.position.x) * 150);
			}
			else if (transform.position.x < perfectPosition.position.x) {
				m_Rigidbody.AddForce (Vector3.right * (perfectPosition.position.x - transform.position.x) * 150);
			}

			if (transform.position.z > rail * k_RailWidth) {
				//m_Rigidbody.AddForce (Vector3.back * (transform.position.z - rail * k_RailWidth) * 150);
				m_Rigidbody.AddForce (Vector3.back * (transform.position.z - rail * k_RailWidth) * 100);
			}
			else if (transform.position.z < rail * k_RailWidth) {
				//m_Rigidbody.AddForce (Vector3.forward * (rail * k_RailWidth - transform.position.b) * 150);
				m_Rigidbody.AddForce (Vector3.forward * (rail * k_RailWidth - transform.position.z) * 100);
			}
		}
    }
		

	public void Move(float hAxis, float vAxis, bool jump, bool left, bool right) {
		m_Anim.SetBool ("Ground", m_Grounded || groundedThisFrame);		//for smoothing
		groundedThisFrame = m_Grounded;

		m_Anim.SetBool("JumpFire", false);

		// processes input based on movement mode
		float moveX, moveZ;
		if (sidestepMode) {
			moveX = vAxis;
			moveZ = -hAxis;

			//rails
			if (canChangeRail) {
				if (left && !right && rail != 1) {
					rail += 1;
					canChangeRail = false;
					m_Anim.SetBool ("LeftStep", true);
				} 
				else if (right && !left && rail != -1) {
					rail -= 1;
					canChangeRail = false;
					m_Anim.SetBool ("RightStep", true);
				}
			}
		}
		else {
			moveX = hAxis;
			moveZ = 0.0f;
		}
			


		// Move the character
		Vector3 tempVel = m_Rigidbody.velocity;
		tempVel.x = moveX * m_MaxSpeed + aml.speed;
		//tempVel.z = moveZ * m_MaxSpeed;

		// If the player should jump...
		if (m_RemainingJumps > 0 && jump) {
			tempVel.y = m_JumpVelocity;
			m_RemainingJumps--;

			m_Grounded = false;
			groundedThisFrame = false;
			m_Anim.SetBool ("JumpFire", true);
			m_Anim.SetBool ("Ground", false);
		} 

		m_Rigidbody.velocity = tempVel;
	}

	private void RailAlign(){
		Vector3 tempVel = m_Rigidbody.velocity;
		tempVel.z = 0.0f;
		m_Rigidbody.velocity = tempVel;

		Vector3 tempPos = transform.position;
		tempPos.z = rail * k_RailWidth;
		transform.position = tempPos;

		canChangeRail = true;
		StartCoroutine (StopStep (0.2f));
	}
	private IEnumerator StopStep(float timeSeconds){
		yield return new WaitForSeconds (timeSeconds);
		m_Anim.SetBool ("LeftStep", false);
		m_Anim.SetBool ("RightStep", false);
	}


	private void StartSidestepMode(){
		sidestepMode = true;
		StopCoroutine (cam2D);
		StartCoroutine (camSidestep);
		//m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
	}

	private void Start2DMode(){
		sidestepMode = false;
		rail = 0;
		RailAlign ();	//sets character back to center
		StopCoroutine(camSidestep);
		StartCoroutine (cam2D);
		//m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
		m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
	}

	private IEnumerator CameraSidestepAngle(){
		Quaternion tempQ = m_CamOffset.rotation;
		Vector3 tempRot = m_CamOffset.rotation.eulerAngles;
		int rate = 10;
		while (m_CamOffset.rotation.eulerAngles.y < 90){
			tempRot.y = Mathf.Lerp (m_CamOffset.rotation.eulerAngles.y, 90, Time.deltaTime * rate);
			tempQ.eulerAngles = tempRot;
			m_CamOffset.rotation = tempQ;
			rate++;
			yield return null;
		}
	}
	private IEnumerator Camera2DAngle(){
		Quaternion tempQ = m_CamOffset.rotation;
		Vector3 tempRot = m_CamOffset.rotation.eulerAngles;
		int rate = 10;
		while (m_CamOffset.rotation.eulerAngles.y > 0){
			tempRot.y = Mathf.Lerp (m_CamOffset.rotation.eulerAngles.y, 0, Time.deltaTime * rate);
			tempQ.eulerAngles = tempRot;
			m_CamOffset.rotation = tempQ;
			rate++;
			yield return null;
		}
	}

	public void Die(){
		m_Anim.Play ("Running");
		m_Rigidbody.velocity = Vector3.zero;
		transform.position = lastCheckpoint;
		rail = 0;
		RailAlign ();

		Transform tempCam = GameObject.FindGameObjectWithTag ("CameraController").transform;

		Vector3 tempCamPos = tempCam.position;
		tempCamPos.x = lastCheckpoint.x;
		tempCam.position = tempCamPos;
	}

	void OnCollisionEnter(Collision c){
		Vector2 center = m_Capsule.bounds.center;
		foreach (ContactPoint p in c.contacts) {			//detects ground
			Vector2 flex = (Vector2) p.point - (Vector2) center;
			float angle = Mathf.Atan2 (flex.y, flex.x) * Mathf.Rad2Deg;
			if (angle < -40 && angle > -140){
				m_Grounded = true;
				m_RemainingJumps = k_MaxJumps;
			}
		}
	}
	void OnCollisionStay(Collision c){
		Vector2 center = m_Capsule.bounds.center;
		foreach (ContactPoint p in c.contacts) {			//detects ground
			Vector2 flex = (Vector2) p.point - (Vector2) center;
			float angle = Mathf.Atan2 (flex.y, flex.x) * Mathf.Rad2Deg;
			if (angle < -40 && angle > -140){
				m_Grounded = true;
				m_RemainingJumps = k_MaxJumps;
			}
		}
	}
	void OnCollisionExit(Collision c){
		m_Grounded = false;
	}


	void OnTriggerEnter(Collider other){
		if (other.tag.Equals ("Respawn")) {
			lastCheckpoint = transform.position;
		} else if (other.tag.Equals ("Deadly")) {
			Die ();
		} else if (other.tag.Equals ("Collectible")) {
			Destroy (other.gameObject);
		} else if (other.tag.Equals ("SidestepMode")) {
			StartSidestepMode ();
		}
		else if (other.tag.Equals ("SidestepMode")) {
			StartSidestepMode ();
		}
		else if (other.tag.Equals ("2DMode")) {
			Start2DMode ();
		}
		else if (other.tag.Equals ("Rail") && sidestepMode) {
			RailAlign ();
		}
	}
}
