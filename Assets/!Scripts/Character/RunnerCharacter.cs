using System;
using UnityEngine;

using System.Collections;

public class RunnerCharacter : MonoBehaviour
{
	[SerializeField] private float m_JumpStrength = 15f;                // Upward velocity when the player jumps.
    //[SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

	private bool m_Grounded;			// Whether or not the player is grounded.
    private Animator m_Anim;			// Reference to the player's animator component.
	private Rigidbody m_Rigidbody;
	private BoxCollider m_BoxCollider;

	const int k_MaxJumps = 2;
	private int m_RemainingJumps;
	private bool fallen = false;
	private bool groundedLastFrame;

	private AutoMoveLevel aml;
	private Vector3 lastCheckpoint;
	private Transform m_CamOffset;
	private Transform m_CamZoom;
	private const float k_CamZoomPosition = -16f;

	private Transform perfectPosition;

	[SerializeField] private float m_GravityStrength = 1500.0f;
	private Vector3 m_PersonalGravity;

	private bool m_SidestepMode = false;
	private int rail = 0;					// 1 is left, -1 is right
	private int prevRail = 0;				// 1 is left, -1 is right
	private const float k_RailWidth = 5.0f;
	[SerializeField] private float m_SidestepForce = 150.0f;
	private bool shiftingBetweenRails = false;
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
		m_CamZoom = GameObject.FindGameObjectWithTag ("CameraController").transform.Find ("CamOffset/CamZoom").transform;

		perfectPosition = GameObject.FindGameObjectWithTag ("CameraController").transform.Find ("CharacterPosition").transform;

		//m_Anim.SetFloat ("AutoMoveSpeed", Mathf.Sqrt(aml.Speed)/5);

		lastCheckpoint = transform.position;	//first checkpoint is start

		m_Grounded = true;
		groundedLastFrame = true;
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

	private void GroundCheck(){
		Vector3 feet = transform.position;
		feet.y = m_BoxCollider.bounds.min.y;
		//if (Physics.CheckSphere (feet + Vector3.down*m_BoxCollider.bounds.extents.x, m_BoxCollider.bounds.extents.x/2)){
		if (Physics.CheckSphere (feet + Vector3.down*0.01f, 0.005f)){
			m_Grounded = true;
			m_RemainingJumps = k_MaxJumps;
		} else {
			m_Grounded = false;
		}
	}

	private void FixedUpdate(){
		//floating cheat
		if (Input.GetKey (KeyCode.Backspace)) {
			m_Rigidbody.AddForce (-2 * m_PersonalGravity * Time.fixedDeltaTime);	
		}

		//gravity
		m_Rigidbody.AddForce (m_PersonalGravity * Time.fixedDeltaTime);

		//perfect position
		if(transform.position.x != perfectPosition.position.x){
			m_Rigidbody.AddForce (Vector3.right * 100 * (perfectPosition.position.x - transform.position.x));
		}
    }

	public void Move(bool jump, bool left, bool right) {
		
		if (!fallen) {
			GroundCheck ();
			m_Anim.SetBool ("Ground", m_Grounded || groundedLastFrame);
			m_Anim.SetBool ("JumpFire", false);

			groundedLastFrame = m_Grounded;

			// Move the character
			Vector3 tempVel = m_Rigidbody.velocity;
			tempVel.x = aml.Speed;

			// processes input based on movement mode
			if (m_SidestepMode) {
				//rails
				if (!shiftingBetweenRails) {
					if (left && !right) {
						RailJumpLeft ();
					} else if (right && !left) {
						RailJumpRight ();
					}
				}
			} else {
				// If the player should jump...
				if (m_RemainingJumps > 0 && jump) {
					tempVel.y = m_JumpStrength;
					m_RemainingJumps--;

					m_Grounded = false;
					m_Anim.SetBool ("JumpFire", true);
					m_Anim.SetBool ("Ground", false);
				}
			}

			m_Rigidbody.velocity = tempVel;
		}
		else if (jump) {
			Restart ();
		}
	}

	public void RailJumpLeft(){
		if (rail != 1) {
			prevRail = rail;
			rail += 1;

			shiftingBetweenRails = true;

			StopCoroutine (stopStep);	//stops the sidestep animation from cancelling on quickly successive steps
			m_Anim.SetBool ("LeftStep", true);
			m_Anim.SetBool ("RightStep", false);
			railForce = RailForce ();
			StartCoroutine (railForce);
		}
	}
	public void RailJumpRight(){
		if (rail != -1) {
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
		if (!fallen) {
			if (m_SidestepMode) {
				Start2DMode ();
			}
			else {
				StartSidestepMode ();
			}
		}
	}

	private IEnumerator CameraSidestepAngle(){
		Quaternion tempQ = m_CamOffset.rotation;
		Vector3 tempRot = m_CamOffset.rotation.eulerAngles;
		Vector3 tempPos = m_CamZoom.localPosition;
		float rate = 10.0f;
		while (m_CamOffset.rotation.eulerAngles.y < 90f && m_CamZoom.localPosition.z < k_CamZoomPosition){
			tempRot.y = Mathf.Lerp (m_CamOffset.rotation.eulerAngles.y, 90f, Time.deltaTime * rate);
			tempQ.eulerAngles = tempRot;
			m_CamOffset.rotation = tempQ;

			tempPos = m_CamZoom.localPosition;
			tempPos.z = Mathf.Lerp (m_CamZoom.localPosition.z, k_CamZoomPosition, Time.deltaTime * rate);
			m_CamZoom.localPosition = tempPos;

			rate += Time.deltaTime;
			yield return null;
		}
	}
	private IEnumerator Camera2DAngle(){
		Quaternion tempQ = m_CamOffset.rotation;
		Vector3 tempRot = m_CamOffset.rotation.eulerAngles;
		Vector3 tempPos = m_CamZoom.localPosition;
		float rate = 10.0f;
		while (m_CamOffset.rotation.eulerAngles.y > 0f && m_CamZoom.localPosition.z > -20f){
			tempRot.y = Mathf.Lerp (m_CamOffset.rotation.eulerAngles.y, 0f, Time.deltaTime * rate);
			tempQ.eulerAngles = tempRot;
			m_CamOffset.rotation = tempQ;

			tempPos = m_CamZoom.localPosition;
			tempPos.z = Mathf.Lerp (m_CamZoom.localPosition.z, -20f, Time.deltaTime * rate);
			m_CamZoom.localPosition = tempPos;

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
		m_Anim.SetBool ("LeftStep", false);
		m_Anim.SetBool ("RightStep", false);
		m_Anim.SetBool ("BackFall", false);
		m_Anim.Play ("Running");
	}


	public void Restart(){
		m_Rigidbody.drag = 0;
		fallen = false;
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

	void OnTriggerEnter(Collider other){
		if (other.tag.Equals ("Respawn")) {
			lastCheckpoint = transform.position;
		}
		else if (other.tag.Equals ("Deadly")) {
			Restart ();
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

	public void Crash(){
		fallen = true;
		m_Rigidbody.velocity = Vector3.zero;
		m_Anim.SetBool ("Ground", false);
		m_Anim.SetBool ("JumpFire", false);
		m_Anim.SetBool ("LeftStep", false);
		m_Anim.SetBool ("RightStep", false);
		m_Anim.SetBool ("BackFall", true);
		m_Rigidbody.drag = 5;
		m_Rigidbody.AddForce (Vector3.left*1000);

	}
}
