using System;
using UnityEngine;

using System.Collections;

public class ControllerTest : MonoBehaviour
{
    [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
	[SerializeField] private float m_JumpVelocity = 15f;                // Amount of force added when the player jumps.
    [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

    private Animator m_Anim;            // Reference to the player's animator component.

	const int k_ExtraJumps = 1;
	private int m_RemainingJumps;

	private AutoMoveLevel aml;

	public Vector3 lastCheckpoint;

	private CharacterController m_Controller;

	private float yVelocity;
	public float gravityStrength = 1.0f;

    private void Awake()
    {
        // Setting up references.
        //m_Anim = GetComponent<Animator>();
		m_Anim = GetComponentInChildren<Animator>();
		m_Controller = GetComponent<CharacterController>();

		//aml = GameObject.FindGameObjectWithTag ("GameController").GetComponent<AutoMoveLevel> ();

		lastCheckpoint = transform.position;	//first checkpoint is start

		yVelocity = 0.0f;
    }


    private void FixedUpdate()
    {

		if (m_Controller.isGrounded) {
			m_RemainingJumps = k_ExtraJumps;
		}


		m_Anim.SetBool("Ground", m_Controller.isGrounded);

        // Set the vertical animation
        //m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);	//think about it
    }

	/*void Update(){
		yVelocity = yVelocity - gravityStrength * Time.deltaTime;
	}*/


    public void Move(float move, bool jump)
    {

		m_Anim.SetBool("JumpFire", false);

        // The Speed animator parameter is set to the value of the horizontal input. CAN BE NEGATIVE
		m_Anim.SetFloat("Speed", move);

        // Move the character
		//m_Rigidbody2D.velocity = new Vector2(move*m_MaxSpeed + aml.speed, m_Rigidbody2D.velocity.y);	//have aml do it

		float xShift = move*m_MaxSpeed*Time.deltaTime;

        // If the player should jump...
		if (m_RemainingJumps > 0 && jump)
        {
            // Add a vertical force to the player.
            m_Anim.SetBool("Ground", false);
            
			yVelocity = m_JumpVelocity;

			m_RemainingJumps--;
			m_Anim.SetBool("JumpFire", true);
        }

		yVelocity = yVelocity - gravityStrength * Time.deltaTime;

		print ("doing something");

		m_Controller.Move(new Vector3(xShift, yVelocity, 0.0f));
    }



	/*public void Die(){
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
	}*/
		
}
