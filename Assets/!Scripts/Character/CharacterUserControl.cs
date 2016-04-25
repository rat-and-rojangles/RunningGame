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
	private bool m_Switch;

	//pause
	private bool paused = false;
	private AudioLowPassFilter musicLowPass;
	[SerializeField] private float musicMinFQ = 450.0f;

    private void Awake()
    {
        m_Character = GetComponent<RunnerCharacter>();
		musicLowPass = GameObject.FindGameObjectWithTag ("MusicController").GetComponent<AudioLowPassFilter> ();
    }


    private void Update()
    {
		if (CrossPlatformInputManager.GetButtonDown ("Pause")) {	// Pause
			if (paused) {
				Unpause ();
			}
			else{
				Pause ();
			}
		}

		// Read the button inputs in Update so button presses aren't missed.
		if (!paused) {	// Does not queue input while paused
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
			if (!m_Switch) {	
				m_Switch = CrossPlatformInputManager.GetButtonDown ("Switch");
			}
		}

		if (paused) {	// allows for restarting the level while paused
			if (Input.GetKeyUp (KeyCode.Backspace)) {
				m_Character.Die ();
				Unpause ();
			}
		}
    }


    private void FixedUpdate()
    {

        // Read the horizontal input.
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
		float v = CrossPlatformInputManager.GetAxis("Vertical");

        // Pass all parameters to the character control script.
		m_Character.Move(h, v, m_Jump, m_Left, m_Right);
		if (m_Switch) {
			m_Character.ToggleMovementMode ();
		}

        m_Jump = false;
		m_Left = false;
		m_Right = false;
		m_Switch = false;
    }

	private void Pause(){
		paused = true;
		Time.timeScale = 0.0f;
		StopAllCoroutines ();
		StartCoroutine (fadeOut ());
	}
	private void Unpause(){
		paused = false;
		Time.timeScale = 1.0f;
		StopAllCoroutines ();
		StartCoroutine (fadeIn ());
	}

	private IEnumerator fadeIn(){
		while (musicLowPass.cutoffFrequency < 21999) {
			musicLowPass.cutoffFrequency = Mathf.Lerp (musicLowPass.cutoffFrequency, 22000, 3 * Time.unscaledDeltaTime);
			yield return null;
		}
		musicLowPass.cutoffFrequency = 22000f;
	}

	private IEnumerator fadeOut(){
		while (musicLowPass.cutoffFrequency > musicMinFQ+1) {
			musicLowPass.cutoffFrequency = Mathf.Lerp (musicLowPass.cutoffFrequency, musicMinFQ, 6 * Time.unscaledDeltaTime);
			yield return null;
		}
		musicLowPass.cutoffFrequency = musicMinFQ;
	}
}
