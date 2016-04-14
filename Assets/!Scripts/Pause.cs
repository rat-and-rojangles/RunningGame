using UnityEngine;
using System.Collections;

public class Pause : MonoBehaviour {

	private bool paused = false;
	//[SerializeField] private AudioSource music;
	[SerializeField] private AudioLowPassFilter musicLowPass;

	void Update () {
		if (Input.GetButtonDown ("Pause")) {
			paused = !paused;
		}

		if (paused) {
			Time.timeScale = 0.0f;
			musicLowPass.enabled = true;
		}
		else {
			Time.timeScale = 1.0f;
			musicLowPass.enabled = false;
		}
	}


}
