using System;
using UnityEngine;

public class PlatformerCharacter2D : MonoBehaviour
{
    [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
    [SerializeField] private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
    [SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

    private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
    const float k_GroundedRadius = .1f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    private Animator m_Anim;            // Reference to the player's animator component.
    private Rigidbody2D m_Rigidbody2D;

	const int k_ExtraJumps = 1;
	private int m_RemainingJumps;

	public Vector3 lastCheckpoint;

    private void Awake()
    {
        // Setting up references.
        m_GroundCheck = transform.Find("GroundCheck");
        //m_Anim = GetComponent<Animator>();
		m_Anim = GetComponentInChildren<Animator>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

		lastCheckpoint = transform.position;	//first checkpoint is start
    }


    private void FixedUpdate()
    {
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
			if (colliders [i].gameObject != gameObject) {
				m_Grounded = true;
				m_RemainingJumps = k_ExtraJumps;
			}
        }
        m_Anim.SetBool("Ground", m_Grounded);

        // Set the vertical animation
        m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);
    }


    public void Move(float move, bool jump)
    {
		m_Anim.SetBool("JumpFire", false);
        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {
            // The Speed animator parameter is set to the value of the horizontal input. CAN BE NEGATIVE
			m_Anim.SetFloat("Speed", move);
			if (move < 0) {
				
			}

            // Move the character
            m_Rigidbody2D.velocity = new Vector2(move*m_MaxSpeed, m_Rigidbody2D.velocity.y);
        }
        // If the player should jump...
		if (m_RemainingJumps > 0 && jump)
        {
            // Add a vertical force to the player.
            m_Grounded = false;
            m_Anim.SetBool("Ground", false);
            
			Vector2 tempVel = m_Rigidbody2D.velocity;
			tempVel.y = m_JumpForce;
			m_Rigidbody2D.velocity = tempVel;
			m_RemainingJumps--;
			m_Anim.SetBool("JumpFire", true);
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
}
