using System;
using UnityEngine;

using System.Collections;

public class PlatformerCharacter2D : MonoBehaviour
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

	private bool groundedThisFrame = true;		//used for smoothing the animator

	[SerializeField] private float m_GravityStrength = 1500.0f;
	private Vector3 m_PersonalGravity;

    private void Awake()
    {
        // Setting up references.
		m_Anim = GetComponentInChildren<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
		m_Capsule = GetComponent<CapsuleCollider>();

		aml = GameObject.FindGameObjectWithTag ("GameController").GetComponent<AutoMoveLevel> ();

		lastCheckpoint = transform.position;	//first checkpoint is start

		m_Grounded = true;
		m_Rigidbody.useGravity = false;
		m_PersonalGravity = Vector3.down * m_GravityStrength;
    }


    private void FixedUpdate()
    {
        // Set the vertical animation
        m_Anim.SetFloat("vSpeed", m_Rigidbody.velocity.y);

		m_Rigidbody.AddForce (m_PersonalGravity * Time.fixedDeltaTime);		//gravity
    }
		
	private void LateUpdate(){
		m_Anim.SetBool ("Ground", m_Grounded || groundedThisFrame);		//for smoothing

		groundedThisFrame = m_Grounded;
		//m_Grounded = groundedThisFrame;
	}


    public void Move(float move, bool jump)
    {
		m_Anim.SetBool("JumpFire", false);

        // The Speed animator parameter is set to the value of the horizontal input. CAN BE NEGATIVE
		m_Anim.SetFloat("Speed", move);

        // Move the character
		//m_Rigidbody.velocity = new Vector2(move*m_MaxSpeed + aml.speed, m_Rigidbody.velocity.y);
		Vector3 tempVel = m_Rigidbody.velocity;
		tempVel.x = move * m_MaxSpeed + aml.speed;

        // If the player should jump...
		if (m_RemainingJumps > 0 && jump) {
			// Add a vertical force to the player.
			tempVel.y = m_JumpVelocity;
			m_RemainingJumps--;

			m_Anim.SetBool ("JumpFire", true);
		} 

		m_Rigidbody.velocity = tempVel;
    }

	public void Die(){
		m_Rigidbody.position = lastCheckpoint;

		Transform tempCam = GameObject.FindGameObjectWithTag ("CameraController").transform;

		Vector3 tempCamPos = tempCam.position;
		tempCamPos.x = lastCheckpoint.x;
		tempCam.position = tempCamPos;
	}

	void OnTriggerEnter(Collider other){
		if (other.tag.Equals ("Respawn")) {
			lastCheckpoint = other.transform.position;
		} else if (other.tag.Equals ("Deadly")) {
			Die ();
		}
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
}
