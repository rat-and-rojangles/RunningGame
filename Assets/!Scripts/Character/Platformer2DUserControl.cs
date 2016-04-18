using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;

[RequireComponent(typeof (PlatformerCharacter2D))]
public class Platformer2DUserControl : MonoBehaviour
{
    private PlatformerCharacter2D m_Character;
    private bool m_Jump;

	//pause
	private bool paused = false;
	private AudioLowPassFilter musicLowPass;
	private const float minFQ = 450.0f;

    private void Awake()
    {
        m_Character = GetComponent<PlatformerCharacter2D>();
		musicLowPass = GameObject.FindGameObjectWithTag ("MusicController").GetComponent<AudioLowPassFilter> ();
    }


    private void Update()
    {
		if (CrossPlatformInputManager.GetButtonDown ("Pause")) {
			paused = !paused;

			if (paused) {
				Time.timeScale = 0.0f;
				StopAllCoroutines ();
				StartCoroutine (fadeOut ());
			}
			else{
				Time.timeScale = 1.0f;
				StopAllCoroutines ();
				StartCoroutine (fadeIn ());
			}
		}

		if (!m_Jump && !paused) {
			// Read the jump input in Update so button presses aren't missed.
			m_Jump = CrossPlatformInputManager.GetButtonDown ("Jump");
		}
    }


    private void FixedUpdate()
    {
        // Read the horizontal input.
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        // Pass all parameters to the character control script.
        m_Character.Move(h, m_Jump);
        m_Jump = false;
    }

	private IEnumerator fadeIn(){
		//while (musicLowPass.cutoffFrequency < 22000) {
		while (musicLowPass.cutoffFrequency < 21999) {
			//print ("i");
			musicLowPass.cutoffFrequency = Mathf.Lerp (musicLowPass.cutoffFrequency, 22000, 3 * Time.unscaledDeltaTime);
			yield return null;
		}
		musicLowPass.cutoffFrequency = 22000;
	}

	private IEnumerator fadeOut(){
		while (musicLowPass.cutoffFrequency > minFQ+1) {
			//print ("o");
			musicLowPass.cutoffFrequency = Mathf.Lerp (musicLowPass.cutoffFrequency, minFQ, 6 * Time.unscaledDeltaTime);
			yield return null;
		}
		musicLowPass.cutoffFrequency = minFQ-1;
	}
}
