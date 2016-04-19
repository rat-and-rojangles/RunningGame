using System;
using UnityEngine;

using System.Collections;

public class RunnerCharacter : MonoBehaviour
{
    [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
	[SerializeField] private float m_JumpVelocity = 15f;                // Upward velocity when the player jumps.
    [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

	private bool m_Grounded;			// Whether or not the player is grounded.
    private Animator m_Anim;			// Reference to the player's animator component.
	private Rigidbody m_Rigidbody;
	private CapsuleCollider m_Capsule;

	const int k_MaxJumps = 1;
	private int m_RemainingJumps = 500;

	private AutoMoveLevel aml;
	public Vector3 lastCheckpoint;
	private Transform m_CamOffset;

	private bool groundedThisFrame = true;		//used for smoothing the animator

	[SerializeField] private float m_GravityStrength = 1500.0f;
	private Vector3 m_PersonalGravity;

	public bool sidestepMode = false;

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
    }


    private void FixedUpdate()
    {
		if (Input.GetKey (KeyCode.Backspace)) {								//floating cheat
			m_Rigidbody.AddForce (-2 * m_PersonalGravity * Time.fixedDeltaTime);	
		}

        // Set the vertical animation
        m_Anim.SetFloat("vSpeed", m_Rigidbody.velocity.y);

		m_Rigidbody.AddForce (m_PersonalGravity * Time.fixedDeltaTime);		//gravity
    }
		

	public void Move(float hAxis, float vAxis, bool jump) {
		m_Anim.SetBool ("Ground", m_Grounded || groundedThisFrame);		//for smoothing
		groundedThisFrame = m_Grounded;

		m_Anim.SetBool("JumpFire", false);

		// processes input based on movement mode
		float moveX, moveZ;
		if (sidestepMode) {
			moveX = vAxis;
			moveZ = -hAxis;
		} else {
			moveX = hAxis;
			moveZ = 0.0f;
		}

		print (moveZ);

		// Move the character
		Vector3 tempVel = m_Rigidbody.velocity;
		tempVel.x = moveX * m_MaxSpeed + aml.speed;
		tempVel.z = moveZ * m_MaxSpeed;

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

	private void StartSidestepMode(){
		sidestepMode = true;
		//m_CamOffset.rotation = Quaternion.Euler (new Vector3 (0, 90, 0));
		StartCoroutine(TwistCameraSidestep());
		m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
	}

	private IEnumerator TwistCameraSidestep(){
		Quaternion tempQ = m_CamOffset.rotation;
		Vector3 tempRot = m_CamOffset.rotation.eulerAngles;
		while (m_CamOffset.rotation.eulerAngles.y < 90){
			tempRot.y = Mathf.Lerp (m_CamOffset.rotation.eulerAngles.y, 90, Time.deltaTime * 20);
			tempQ.eulerAngles = tempRot;
			m_CamOffset.rotation = tempQ;
			yield return null;
		}
	}

	public void Die(){
		m_Anim.Play ("Running");
		m_Rigidbody.velocity = Vector3.zero;
		m_Rigidbody.position = lastCheckpoint;

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
			print ("collected");
		} else if (other.tag.Equals ("SidestepMode")) {
			StartSidestepMode ();
		}

	}
}
