using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class PauseControl : MonoBehaviour {

	private bool paused = false;
	private AudioLowPassFilter musicLowPass;
	[SerializeField] private float musicMinFQ = 450.0f;

	private IEnumerator fadeIn;
	private IEnumerator fadeOut;

	// Use this for initialization
	void Awake () {
		musicLowPass = GameObject.FindGameObjectWithTag ("MusicController").GetComponent<AudioLowPassFilter> ();

		fadeIn = FadeIn ();
		fadeOut = FadeOut ();
	}

	public bool IsPaused(){
		return paused;
	}

	public void TogglePause(){
		if (paused) {
			Unpause ();
		}
		else {
			Pause ();
		}
	}

	public void Pause(){
		paused = true;
		Time.timeScale = 0.0f;

		StopCoroutine (fadeIn);
		fadeOut = FadeOut ();
		StartCoroutine (fadeOut);
	}
	public void Unpause(){
		paused = false;
		Time.timeScale = 1.0f;

		StopCoroutine (fadeOut);
		fadeIn = FadeIn ();
		StartCoroutine (fadeIn);
	}

	private IEnumerator FadeIn(){
		while (musicLowPass.cutoffFrequency < 21999) {
			musicLowPass.cutoffFrequency = Mathf.Lerp (musicLowPass.cutoffFrequency, 22000, 3 * Time.unscaledDeltaTime);
			yield return null;
		}
		musicLowPass.cutoffFrequency = 22000f;
	}

	private IEnumerator FadeOut(){
		while (musicLowPass.cutoffFrequency > musicMinFQ+1) {
			musicLowPass.cutoffFrequency = Mathf.Lerp (musicLowPass.cutoffFrequency, musicMinFQ, 6 * Time.unscaledDeltaTime);
			yield return null;
		}
		musicLowPass.cutoffFrequency = musicMinFQ;
	}

}
