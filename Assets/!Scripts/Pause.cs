using UnityEngine;
using System.Collections;

public class Pause : MonoBehaviour {

	private bool paused = false;
	private AudioLowPassFilter musicLowPass;

	private const float minFQ = 450.0f;

	// Use this for initialization
	void Awake () {
		musicLowPass = GameObject.FindGameObjectWithTag ("MusicController").GetComponent<AudioLowPassFilter> ();
	}

	void Update () {
		if (Input.GetButtonDown ("Pause")) {
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
