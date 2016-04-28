using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class PauseControl : MonoBehaviour {

	private bool paused = false;
	private AudioLowPassFilter musicLowPass;
	[SerializeField] private float musicMinFQ = 450.0f;

	private Vector3 camLastEuler;
	private Transform camTrans;
	private bool currentlyResettingPauseCam = false;

	private IEnumerator fadeIn;
	private IEnumerator fadeOut;

	// Use this for initialization
	void Awake () {
		musicLowPass = GameObject.FindGameObjectWithTag ("MusicController").GetComponent<AudioLowPassFilter> ();
		camTrans = GameObject.FindGameObjectWithTag ("CameraController").transform.Find ("CamOffset").transform;

		fadeIn = FadeIn ();
		fadeOut = FadeOut ();
	}

	public bool IsPaused(){
		return paused;
	}

	public void TogglePause(){
		if (!currentlyResettingPauseCam) {
			if (paused) {
				Unpause ();
			}
			else {
				Pause ();
			}
		}
	}

	public void Pause(){
		paused = true;
		Time.timeScale = 0.0f;

		camLastEuler = camTrans.rotation.eulerAngles;

		StopCoroutine (fadeIn);
		fadeOut = FadeOut ();
		StartCoroutine (fadeOut);
	}
	/*public void Unpause(){
		SetCameraEuler (camLastEuler);

		paused = false;
		Time.timeScale = 1.0f;

		StopCoroutine (fadeOut);
		fadeIn = FadeIn ();
		StartCoroutine (fadeIn);
	}*/
	public void Unpause(){
		StartCoroutine (UnpauseWithCamReset ());
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

	private IEnumerator UnpauseWithCamReset(){
		//float total = camTrans.rotation.eulerAngles.y - camLastEuler.y;
		/*int undoSegments = 25;
		for (int counter = 0; counter < undoSegments; counter++) {
			Vector3 tempEuler = camTrans.rotation.eulerAngles;
			tempEuler += (Vector3.down * total/undoSegments);	//undoes x shift
			SetCameraEuler (tempEuler);
			yield return null;
		}*/
		currentlyResettingPauseCam = true;

		float rate = 10.0f;
		//while (!camLastEuler.Equals(camTrans.rotation.eulerAngles)) {
		while (!AlmostEquals(camLastEuler.y, camTrans.rotation.eulerAngles.y)) {
			//print (camTrans.rotation.eulerAngles);
			//print (camLastEuler);
			//print(rate * Time.unscaledDeltaTime);

			SetCameraEuler (Vector3.Lerp (camTrans.rotation.eulerAngles, camLastEuler, rate * Time.unscaledDeltaTime));
			//SetCameraEuler(camLastEuler);
			rate += Time.unscaledDeltaTime * 5;
			yield return null;
		}
		SetCameraEuler (camLastEuler);

		////////////
		paused = false;
		Time.timeScale = 1.0f;
		currentlyResettingPauseCam = false;

		StopCoroutine (fadeOut);
		fadeIn = FadeIn ();
		StartCoroutine (fadeIn);
	}

	void Update(){
		if (paused && !currentlyResettingPauseCam) {	//krazykam
			Vector3 tempEuler = camTrans.rotation.eulerAngles;
			tempEuler += Vector3.down * CrossPlatformInputManager.GetAxisRaw("Horizontal") * Time.unscaledDeltaTime * 100;
			//tempEuler += Vector3.right * CrossPlatformInputManager.GetAxisRaw("Vertical") * Time.unscaledDeltaTime * 100;
			SetCameraEuler (tempEuler);
		}
	}

	private void SetCameraEuler(Vector3 euler){
		Quaternion tempQ = camTrans.rotation;
		tempQ.eulerAngles = euler;
		camTrans.rotation = tempQ;
	}

	private bool AlmostEquals(float a, float b){
		return Mathf.Abs (a - b) < 0.05;
	}
}
