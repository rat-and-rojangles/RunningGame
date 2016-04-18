using System;
using UnityEngine;

using System.Collections;

public class PlatformerCharacter2D : MonoBehaviour
{
    [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
    [SerializeField] private float m_JumpForce = 15f;                   // Amount of force added when the player jumps.
    [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

    private bool m_Grounded;            // Whether or not the player is grounded.
    private Animator m_Anim;            // Reference to the player's animator component.
    private Rigidbody2D m_Rigidbody2D;
	private CircleCollider2D m_CircleCollider;

	const int k_MaxJumps = 2;
	private int m_RemainingJumps;

	private AutoMoveLevel aml;
	public Vector3 lastCheckpoint;

	public bool groundedThisFrame;

    private void Awake()
    {
        // Setting up references.
        //m_Anim = GetComponent<Animator>();
		m_Anim = GetComponentInChildren<Animator>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
		m_CircleCollider = GetComponent<CircleCollider2D>();

		aml = GameObject.FindGameObjectWithTag ("GameController").GetComponent<AutoMoveLevel> ();

		lastCheckpoint = transform.position;	//first checkpoint is start

		m_Grounded = false;
    }


    private void FixedUpdate()
    {
		//m_Rigidbody2D.gravityScale = m_Grounded ? 100.0f : 3.0f;
        // Set the vertical animation
        m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);
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
		m_Rigidbody2D.velocity = new Vector2(move*m_MaxSpeed + aml.speed, m_Rigidbody2D.velocity.y);

        // If the player should jump...
		if (m_RemainingJumps > 0 && jump) {
			// Add a vertical force to the player.
			Vector2 tempVel = m_Rigidbody2D.velocity;
			tempVel.y = m_JumpForce;
			m_Rigidbody2D.velocity = tempVel;
			m_RemainingJumps--;

			m_Anim.SetBool ("JumpFire", true);
		} 
    }

	public void Die(){
		m_Rigidbody2D.position = lastCheckpoint;

		Transform tempCam = GameObject.FindGameObjectWithTag ("CameraController").transform;

		Vector3 tempCamPos = tempCam.position;
		tempCamPos.x = lastCheckpoint.x;
		tempCam.position = tempCamPos;
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag.Equals ("Respawn")) {
			lastCheckpoint = other.transform.position;
		} else if (other.tag.Equals ("Deadly")) {
			Die ();
		}
	}

	void OnCollisionEnter2D(Collision2D c){
		Vector2 center = m_CircleCollider.bounds.center;
		foreach (ContactPoint2D p in c.contacts) {			//detects ground
			Vector2 flex = p.point - center;
			float angle = Mathf.Atan2 (flex.y, flex.x) * Mathf.Rad2Deg;
			if (angle < -40 && angle > -140){
				m_Grounded = true;
				m_RemainingJumps = k_MaxJumps;
			}
		}
	}
	void OnCollisionStay2D(Collision2D c){
		Vector2 center = m_CircleCollider.bounds.center;
		foreach (ContactPoint2D p in c.contacts) {			//detects ground
			Vector2 flex = p.point - center;
			float angle = Mathf.Atan2 (flex.y, flex.x) * Mathf.Rad2Deg;
			if (angle < -40 && angle > -140){
				m_Grounded = true;
				m_RemainingJumps = k_MaxJumps;
			}
		}
	}
	void OnCollisionExit2D(Collision2D c){
		m_Grounded = false;
	}
}
