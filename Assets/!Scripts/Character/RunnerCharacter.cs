using System;
using UnityEngine;

using System.Collections;

public class RunnerCharacter : MonoBehaviour
{
    [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
	[SerializeField] private float m_JumpStrength = 15f;                // Upward velocity when the player jumps.
    //[SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

	private bool m_Grounded;			// Whether or not the player is grounded.
    private Animator m_Anim;			// Reference to the player's animator component.
	private Rigidbody m_Rigidbody;
	private BoxCollider m_BoxCollider;

	const int k_MaxJumps = 1;
	private int m_RemainingJumps;

	private AutoMoveLevel aml;
	private Vector3 lastCheckpoint;
	private Transform m_CamOffset;

	private bool groundedLastFrame = true;		//used for smoothing bumps in animator

	[SerializeField] private float m_GravityStrength = 1500.0f;
	private Vector3 m_PersonalGravity;

	private bool m_SidestepMode = false;
	private int rail = 0;					// 1 is left, -1 is right
	private int prevRail = 0;				// 1 is left, -1 is right
	private const float k_RailWidth = 5.0f;
	[SerializeField] private float m_SidestepForce = 150.0f;
	private bool shiftingBetweenRails = false;
	private Transform leftCheck;
	private Transform rightCheck;
	private int m_RailForceSign;

	//coroutines that can be stopped
	private IEnumerator camSidestep;
	private IEnumerator cam2D;
	private IEnumerator railForce;
	private IEnumerator stopStep;

	private const float k_SidestepAnimationLength = 0.1f;

    private void Awake()
    {
        // Setting up references.
		m_Anim = GetComponentInChildren<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
		m_BoxCollider = GetComponent<BoxCollider>();

		aml = GameObject.FindGameObjectWithTag ("GameController").GetComponent<AutoMoveLevel> ();
		m_CamOffset = GameObject.FindGameObjectWithTag ("CameraController").transform.Find ("CamOffset").transform;

		m_Anim.SetFloat ("AutoMoveSpeed", Mathf.Sqrt(aml.speed)/5);

		lastCheckpoint = transform.position;	//first checkpoint is start

		m_Grounded = true;
		m_Rigidbody.useGravity = false;
		m_PersonalGravity = Vector3.down * m_GravityStrength;

		//initializing these so they won't be null when stopped
		camSidestep = CameraSidestepAngle ();
		cam2D = Camera2DAngle ();
		railForce = RailForce ();
		stopStep = StopStepAnimation (k_SidestepAnimationLength);
    }

	public bool GetSidestepMode(){
		return m_SidestepMode;
	}


    private void FixedUpdate()
    {

		//ground check
		Vector3 feet = transform.position;
		feet.y = m_BoxCollider.bounds.min.y;
		if (Physics.CheckSphere (feet + Vector3.down*m_BoxCollider.bounds.extents.x, m_BoxCollider.bounds.extents.x/2)){
			m_Grounded = true;
			m_RemainingJumps = k_MaxJumps;
		} else {
			m_Grounded = false;
		}

		//floating cheat
		if (Input.GetKey (KeyCode.Backspace)) {
			m_Rigidbody.AddForce (-2 * m_PersonalGravity * Time.fixedDeltaTime);	
		}

        // Set the vertical animation
        m_Anim.SetFloat("vSpeed", m_Rigidbody.velocity.y);

		//gravity
		m_Rigidbody.AddForce (m_PersonalGravity * Time.fixedDeltaTime);
    }

	public void Move(bool jump, bool left, bool right) {
		m_Anim.SetBool ("Ground", m_Grounded || groundedLastFrame);		//for smoothing
		groundedLastFrame = m_Grounded;

		m_Anim.SetBool("JumpFire", false);

		// processes input based on movement mode
		float moveX;
		if (m_SidestepMode) {
			moveX = 0.0f;

			//rails
			if (!shiftingBetweenRails) {
				if (left && !right && rail != 1) {
					prevRail = rail;
					rail += 1;

					shiftingBetweenRails = true;

					StopCoroutine (stopStep);	//stops the sidestep animation from cancelling on quickly successive steps
					m_Anim.SetBool ("LeftStep", true);
					m_Anim.SetBool ("RightStep", false);
					railForce = RailForce ();
					StartCoroutine (railForce);
				} 
				else if (right && !left && rail != -1) {
					prevRail = rail;
					rail -= 1;

					shiftingBetweenRails = true;

					StopCoroutine (stopStep);	//stops the sidestep animation from cancelling on quickly successive steps
					m_Anim.SetBool ("RightStep", true);
					m_Anim.SetBool ("LeftStep", false);
					railForce = RailForce ();
					StartCoroutine (railForce);
				}
			}
		}

		// Move the character
		Vector3 tempVel = m_Rigidbody.velocity;
		tempVel.x = aml.speed;

		// If the player should jump...
		if (m_RemainingJumps > 0 && jump) {
			tempVel.y = m_JumpStrength;
			m_RemainingJumps--;

			m_Grounded = false;
			groundedLastFrame = false;
			m_Anim.SetBool ("JumpFire", true);
			m_Anim.SetBool ("Ground", false);
		}

		m_Rigidbody.velocity = tempVel;
	}

	private void RailAlign(){
		shiftingBetweenRails = false;

		Vector3 tempVel = m_Rigidbody.velocity;
		tempVel.z = 0.0f;
		m_Rigidbody.velocity = tempVel;

		Vector3 tempPos = transform.position;
		tempPos.z = rail * k_RailWidth;
		transform.position = tempPos;

		stopStep = StopStepAnimation (k_SidestepAnimationLength);
		StartCoroutine (stopStep);
	}
	private IEnumerator StopStepAnimation(float timeSeconds){
		yield return new WaitForSeconds (timeSeconds);
		m_Anim.SetBool ("LeftStep", false);
		m_Anim.SetBool ("RightStep", false);
	}

	private void StartSidestepMode(){
		m_SidestepMode = true;
		StopCoroutine (cam2D);
		camSidestep = CameraSidestepAngle ();
		StartCoroutine (camSidestep);
	}

	private void Start2DMode(){
		m_SidestepMode = false;
		//rail = 0;
		//RailAlign ();	//sets character back to center
		StopCoroutine (camSidestep);
		cam2D = Camera2DAngle ();
		StartCoroutine (cam2D);

		/*
		//jump back to rail 0
		if (rail == -1) {
			prevRail = rail;
			rail = 0;

			shiftingBetweenRails = true;

			railForce = RailForce ();
			StartCoroutine (railForce);
		} 
		else if (rail == 1) {
			prevRail = rail;
			rail = 0;

			shiftingBetweenRails = true;

			railForce = RailForce ();
			StartCoroutine (railForce);
		}*/

	}

	public void ToggleMovementMode(){
		if (m_SidestepMode) {
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

	private IEnumerator RailForce(){
		m_RailForceSign = Math.Sign (rail * k_RailWidth - transform.position.z);
		bool stillAdjusting = true;
		while (stillAdjusting) {
			m_Rigidbody.AddForce (Vector3.forward * (rail * k_RailWidth - transform.position.z) * m_SidestepForce);
			if (m_RailForceSign != Math.Sign (rail * k_RailWidth - transform.position.z)) {
				stillAdjusting = false;
			}
			yield return new WaitForFixedUpdate ();
		}
		RailAlign ();
		shiftingBetweenRails = false;
	}

	private void ResetAnimation(){
		StopCoroutine(railForce);
		StopCoroutine(stopStep);
		m_Anim.SetBool ("FastFall", false);
		m_Anim.SetBool ("LeftStep", false);
		m_Anim.SetBool ("RightStep", false);
		m_Anim.Play ("Running");
	}

	public void Die(){
		ResetAnimation ();
		m_Rigidbody.velocity = Vector3.zero;
		transform.position = lastCheckpoint;
		rail = 0;
		RailAlign ();

		Transform tempCam = GameObject.FindGameObjectWithTag ("CameraController").transform;

		Vector3 tempCamPos = tempCam.position;
		tempCamPos.x = lastCheckpoint.x;
		tempCam.position = tempCamPos;
	}

	/*void OnCollisionEnter(Collision c){
		Vector3 center = m_BoxCollider.bounds.center;
		foreach (ContactPoint p in c.contacts) {			//detects ground
			Vector3 flex = p.point - center;
			float angle = Mathf.Atan2 (flex.y, flex.x) * Mathf.Rad2Deg;
			//if (angle < -40 && angle > -140){
			if (angle < -k_MaxGroundCollisionAngle && angle > k_MaxGroundCollisionAngle-180){
				m_Grounded = true;
				m_RemainingJumps = k_MaxJumps;
			}
		}
	}
	void OnCollisionStay(Collision c){
		Vector3 center = m_BoxCollider.bounds.center;
		foreach (ContactPoint p in c.contacts) {			//detects ground
			Vector3 flex = p.point - center;
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
	}*/


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

	public void SideTriggerCollide(int direction){
		if (shiftingBetweenRails) {
			if (prevRail + direction == rail) {		//if moving in the correct direction

				//switches previous rail and current rail
				int temp = rail;
				rail = prevRail;
				prevRail = temp;

				if (rail > prevRail) {
					m_Anim.SetBool ("LeftStep", true);
					m_Anim.SetBool ("RightStep", false);
				}
				else {
					m_Anim.SetBool ("LeftStep", false);
					m_Anim.SetBool ("RightStep", true);
				}

				m_RailForceSign = m_RailForceSign * -1;
			}
		}
	}
}
