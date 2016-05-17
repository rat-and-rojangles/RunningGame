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

	private Camera mainCam;
	private PauseControl pauseControl;

    private void Awake()
    {
        m_Character = GetComponent<RunnerCharacter>();
		pauseControl = GameObject.FindGameObjectWithTag ("GameController").GetComponent<PauseControl> ();
		mainCam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();
    }

	public void Tapped(float screenPosX){
		if (!pauseControl.IsPaused ()) {
			m_Jump = true;
			if (m_Character.GetSidestepMode ()) {
				if (mainCam.WorldToViewportPoint (transform.position).x > screenPosX) {
					m_Left = true;
				} else if (mainCam.WorldToViewportPoint (transform.position).x < screenPosX) {
					m_Right = true;
				}
			}
		}
	}
	public void SwipeLeft(){
		if (m_Character.GetSidestepMode () && !pauseControl.IsPaused ()) {
			m_Switch = true;
		}
	}
	public void SwipeRight(){
		if (!m_Character.GetSidestepMode () && !pauseControl.IsPaused ()) {
			m_Switch = true;
		}
	}

    private void FixedUpdate()
	{
        // Pass all parameters to the character control script, but not if it's paused
		if (!pauseControl.IsPaused ()) {
			m_Character.Move (m_Jump, m_Left, m_Right);
			if (m_Switch) {
				m_Character.ToggleMovementMode ();
			}
		}

        m_Jump = false;
		m_Left = false;
		m_Right = false;
		m_Switch = false;
    }
}
