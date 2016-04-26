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
	private int m_RemainingJumps;

	private AutoMoveLevel aml;
	private Vector3 lastCheckpoint;
	private Transform m_CamOffset;

	private bool groundedThisFrame = true;		//used for smoothing bumps in animator

	private float k_MaxGroundCollisionAngle = 20.0f;

	[SerializeField] private float m_GravityStrength = 1500.0f;
	private Vector3 m_PersonalGravity;

	private bool sidestepMode = false;
	private int rail = 0;					// 1 is left, -1 is right
	private const float k_RailWidth = 5.0f;
	[SerializeField] private float m_SidestepForce = 150.0f;

	private bool shiftingBetweenRails = false;

	private Transform sidestepPositionFromCamera;

	//coroutines that can be stopped
	private IEnumerator camSidestep;
	private IEnumerator cam2D;
	private IEnumerator railForceLeft;
	private IEnumerator railForceRight;
	private IEnumerator stopStep;

	private const float sidestepAnimationLength = 0.1f;

    private void Awake()
    {
        // Setting up references.
		m_Anim = GetComponentInChildren<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
		m_Capsule = GetComponent<CapsuleCollider>();

		aml = GameObject.FindGameObjectWithTag ("GameController").GetComponent<AutoMoveLevel> ();
		m_CamOffset = GameObject.FindGameObjectWithTag ("CameraController").transform.Find ("CamOffset").transform;

		sidestepPositionFromCamera = GameObject.FindGameObjectWithTag ("CameraController").transform.Find ("SidestepPosition").transform;

		lastCheckpoint = transform.position;	//first checkpoint is start

		m_Grounded = true;
		m_Rigidbody.useGravity = false;
		m_PersonalGravity = Vector3.down * m_GravityStrength;

		//initializing these so they won't be null when stopped
		camSidestep = CameraSidestepAngle ();
		cam2D = Camera2DAngle ();
		railForceLeft = RailForceLeft ();
		railForceRight = RailForceRight ();
		stopStep = StopStepAnimation (sidestepAnimationLength);
    }

	public bool GetSidestepMode(){
		return sidestepMode;
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
			//keeping perfect distance from camera
			if (transform.position.x > sidestepPositionFromCamera.position.x) {
				m_Rigidbody.AddForce (Vector3.left * (transform.position.x - sidestepPositionFromCamera.position.x) * 150);
			}
			else if (transform.position.x < sidestepPositionFromCamera.position.x) {
				m_Rigidbody.AddForce (Vector3.right * (sidestepPositionFromCamera.position.x - transform.position.x) * 150);
			}
		}
    }
		

	public void Move(float hAxis, float vAxis, bool jump, bool left, bool right) {
		m_Anim.SetBool ("Ground", m_Grounded || groundedThisFrame);		//for smoothing
		groundedThisFrame = m_Grounded;

		m_Anim.SetBool("JumpFire", false);

		// processes input based on movement mode
		float moveX;
		if (sidestepMode) {
			moveX = 0.0f;

			//rails
			if (!shiftingBetweenRails) {
				if (left && !right && rail != 1) {
					rail += 1;

					shiftingBetweenRails = true;

					StopCoroutine (stopStep);	//stops the sidestep animation from cancelling on quickly successive steps
					m_Anim.SetBool ("LeftStep", true);
					m_Anim.SetBool ("RightStep", false);
					railForceLeft = RailForceLeft ();
					StartCoroutine (railForceLeft);
				} 
				else if (right && !left && rail != -1) {
					rail -= 1;

					shiftingBetweenRails = true;

					StopCoroutine (stopStep);	//stops the sidestep animation from cancelling on quickly successive steps
					m_Anim.SetBool ("RightStep", true);
					m_Anim.SetBool ("LeftStep", false);
					railForceRight = RailForceRight ();
					StartCoroutine (railForceRight);
				}
			}
		}
		else {
			moveX = hAxis;
		}

		// Move the character
		Vector3 tempVel = m_Rigidbody.velocity;
		tempVel.x = moveX * m_MaxSpeed + aml.speed;

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

		if (vAxis >= 0.9f) {
			m_Anim.SetBool ("LyingDown", false);
		}
		else if (vAxis <= -0.9f) {
			m_Anim.SetBool ("LyingDown", true);
		}
	}


	private void RailAlign(){
		shiftingBetweenRails = false;

		Vector3 tempVel = m_Rigidbody.velocity;
		tempVel.z = 0.0f;
		m_Rigidbody.velocity = tempVel;

		Vector3 tempPos = transform.position;
		tempPos.z = rail * k_RailWidth;
		transform.position = tempPos;

		stopStep = StopStepAnimation (sidestepAnimationLength);
		StartCoroutine (stopStep);
	}
	private IEnumerator StopStepAnimation(float timeSeconds){
		yield return new WaitForSeconds (timeSeconds);
		m_Anim.SetBool ("LeftStep", false);
		m_Anim.SetBool ("RightStep", false);
	}


	private void StartSidestepMode(){
		sidestepMode = true;
		StopCoroutine (cam2D);
		camSidestep = CameraSidestepAngle ();
		StartCoroutine (camSidestep);
	}

	private void Start2DMode(){
		sidestepMode = false;
		//rail = 0;
		//RailAlign ();	//sets character back to center
		StopCoroutine (camSidestep);
		cam2D = Camera2DAngle ();
		StartCoroutine (cam2D);
	}

	public void ToggleMovementMode(){
		if (sidestepMode) {
			Start2DMode ();
		}
		else {
			StartSidestepMode ();
		}
	}

	private IEnumerator CameraSidestepAngle(){
		Quaternion tempQ = m_CamOffset.rotation;
		Vector3 tempRot = m_CamOffset.rotation.eulerAngles;
		float rate = 10.0f;
		while (m_CamOffset.rotation.eulerAngles.y < 90){
			tempRot.y = Mathf.Lerp (m_CamOffset.rotation.eulerAngles.y, 90, Time.deltaTime * rate);
			tempQ.eulerAngles = tempRot;
			m_CamOffset.rotation = tempQ;
			rate += Time.deltaTime;
			yield return null;
		}
	}
	private IEnumerator Camera2DAngle(){
		Quaternion tempQ = m_CamOffset.rotation;
		Vector3 tempRot = m_CamOffset.rotation.eulerAngles;
		float rate = 10.0f;
		while (m_CamOffset.rotation.eulerAngles.y > 0){
			tempRot.y = Mathf.Lerp (m_CamOffset.rotation.eulerAngles.y, 0, Time.deltaTime * rate);
			tempQ.eulerAngles = tempRot;
			m_CamOffset.rotation = tempQ;
			rate += Time.deltaTime;
			yield return null;
		}
	}

	private IEnumerator RailForceLeft(){
		int sign = Math.Sign (rail * k_RailWidth - transform.position.z);
		bool stillAdjusting = true;
		while (stillAdjusting) {
			m_Rigidbody.AddForce (Vector3.forward * (rail * k_RailWidth - transform.position.z) * m_SidestepForce);
			if (sign != Math.Sign (rail * k_RailWidth - transform.position.z)) {
				stillAdjusting = false;
			}
			yield return new WaitForFixedUpdate ();
		}
		RailAlign ();
		shiftingBetweenRails = false;
	}
	private IEnumerator RailForceRight(){
		int sign = Math.Sign (rail * k_RailWidth - transform.position.z);
		bool stillAdjusting = true;
		while (stillAdjusting) {
			m_Rigidbody.AddForce (Vector3.forward * (rail * k_RailWidth - transform.position.z) * m_SidestepForce);
			if (sign != Math.Sign (rail * k_RailWidth - transform.position.z)) {
				stillAdjusting = false;
			}
			yield return new WaitForFixedUpdate ();
		}
		RailAlign ();
		shiftingBetweenRails = false;
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
			//if (angle < -40 && angle > -140){
			if (angle < -k_MaxGroundCollisionAngle && angle > k_MaxGroundCollisionAngle-180){
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
			//if (angle < -40 && angle > -140){
			if (angle < -k_MaxGroundCollisionAngle && angle > k_MaxGroundCollisionAngle-180){
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
		}
		else if (other.tag.Equals ("Deadly")) {
			Die ();
		} 
		else if (other.tag.Equals ("Collectible")) {
			Destroy (other.gameObject);
		} 
	}
}
