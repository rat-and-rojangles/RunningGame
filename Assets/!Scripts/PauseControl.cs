using UnityEngine;
using System.Collections;

public class PauseControl : MonoBehaviour {

	private bool paused = false;
	private AudioLowPassFilter musicLowPass;
	[SerializeField] private float musicMinFQ = 450.0f;

	// Use this for initialization
	void Awake () {
		musicLowPass = GameObject.FindGameObjectWithTag ("MusicController").GetComponent<AudioLowPassFilter> ();
	}

	public bool IsPaused(){
		return paused;
	}

	public void TogglePause(){
		if (paused) {
			Unpause ();
		}
		else{
			Pause ();
		}
	}

	public void Pause(){
		paused = true;
		Time.timeScale = 0.0f;
		StopAllCoroutines ();
		StartCoroutine (fadeOut ());
	}
	public void Unpause(){
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
