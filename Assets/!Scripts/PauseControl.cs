using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class PauseControl : MonoBehaviour {

	private bool paused = false;
	private AudioLowPassFilter musicLowPass;
	[SerializeField] private float musicMinFQ = 450.0f;

	private Transform character;
	private const float k_CamYOffset = 3.0f;

	private Vector3 camLastEuler;
	private Transform camTrans;
	private Transform camZoom;
	private bool currentlyResettingPauseCam = false;

	private bool hEnabled = false;

	private float zoom;

	private IEnumerator fadeIn;
	private IEnumerator fadeOut;

	// Use this for initialization
	void Awake () {
		character = GameObject.FindGameObjectWithTag ("Player").transform;

		musicLowPass = GameObject.FindGameObjectWithTag ("MusicController").GetComponent<AudioLowPassFilter> ();
		camTrans = GameObject.FindGameObjectWithTag ("CameraController").transform.Find ("CamOffset").transform;
		camZoom = GameObject.FindGameObjectWithTag ("MainCamera").transform;

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

		//zoom = camZoom.position.z;
		//camTrans.position = new Vector3 (character.position.x, character.position.y - k_CamYOffset, camTrans.position.z);

		StopCoroutine (fadeIn);
		fadeOut = FadeOut ();
		StartCoroutine (fadeOut);
	}
	public void Unpause(){
		hEnabled = false;
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
		currentlyResettingPauseCam = true;

		float rate = 10.0f;
		while (!AlmostEquals(camLastEuler.y, camTrans.rotation.eulerAngles.y)) {
			SetCameraEuler (Vector3.Lerp (camTrans.rotation.eulerAngles, camLastEuler, rate * Time.unscaledDeltaTime));
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
			if(CrossPlatformInputManager.GetButtonDown("Left") || CrossPlatformInputManager.GetButtonDown("Right")){
				hEnabled = true;
			}

			if (hEnabled) {
				Vector3 tempEuler = camTrans.rotation.eulerAngles;
				tempEuler += Vector3.down * CrossPlatformInputManager.GetAxisRaw ("Horizontal") * Time.unscaledDeltaTime * 100;
				SetCameraEuler (tempEuler);
			}

			zoom = Mathf.Clamp (zoom + CrossPlatformInputManager.GetAxisRaw ("Vertical"), -2, -50);
			//SetZoom (zoom);
		}
	}

	private void SetCameraEuler(Vector3 euler){
		Quaternion tempQ = camTrans.rotation;
		tempQ.eulerAngles = euler;
		camTrans.rotation = tempQ;
	}
	private void SetZoom(float z){
		Vector3 tempZoom = camZoom.position;
		tempZoom.z = z;
		camZoom.position = tempZoom;
	}

	private bool AlmostEquals(float a, float b){
		return Mathf.Abs (a - b) < 0.05;
	}
}
