using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;

[RequireComponent(typeof (RunnerCharacter))]
public class CharacterUserControl : MonoBehaviour
{
    private RunnerCharacter m_Character;
    private bool m_Jump;
	private bool m_Left;
	private bool m_Right;
	private bool m_Down;
	private bool m_Switch;

	private PauseControl pauseControl;

    private void Awake()
    {
        m_Character = GetComponent<RunnerCharacter>();
		pauseControl = GameObject.FindGameObjectWithTag ("GameController").GetComponent<PauseControl> ();
    }


    private void Update()
    {
		if (CrossPlatformInputManager.GetButtonDown ("Pause")) {	// Pause
			pauseControl.TogglePause();
		}

		// Read the button inputs in Update so button presses aren't missed.
		if (!pauseControl.IsPaused()) {	// Does not queue input while paused
			if (!m_Jump) {	
				m_Jump = CrossPlatformInputManager.GetButtonDown ("Jump");
			}
			if (!m_Left) {	
				m_Left = CrossPlatformInputManager.GetButtonDown ("Left");
				//m_Left = CrossPlatformInputManager.GetButton ("Left");
			}
			if (!m_Right) {	
				m_Right = CrossPlatformInputManager.GetButtonDown ("Right");
				//m_Right = CrossPlatformInputManager.GetButton ("Right");
			}
			if (!m_Down) {	
				m_Down = CrossPlatformInputManager.GetButtonDown ("Down");
				//m_Right = CrossPlatformInputManager.GetButton ("Right");
			}
			if (!m_Switch) {	
				m_Switch = CrossPlatformInputManager.GetButtonDown ("Switch");
			}
		}

		if (pauseControl.IsPaused()) {	// allows for restarting the level while paused
			if (Input.GetKeyUp (KeyCode.Backspace)) {
				m_Character.Die ();
				pauseControl.Unpause ();
			}
		}
    }


    private void FixedUpdate()
    {

        // Read the horizontal input.
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
		float v = CrossPlatformInputManager.GetAxis("Vertical");

        // Pass all parameters to the character control script.
		m_Character.Move(h, v, m_Jump, m_Left, m_Right, m_Down);
		if (m_Switch) {
			m_Character.ToggleMovementMode ();
		}

        m_Jump = false;
		m_Left = false;
		m_Right = false;
		m_Down = false;
		m_Switch = false;
    }
}
