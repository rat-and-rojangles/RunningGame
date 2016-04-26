using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class PauseControl : MonoBehaviour {

	private bool paused = false;
	private AudioLowPassFilter musicLowPass;
	[SerializeField] private float musicMinFQ = 450.0f;

	private Vector3 camLastEuler;

	private Transform camTrans;

	// Use this for initialization
	void Awake () {
		musicLowPass = GameObject.FindGameObjectWithTag ("MusicController").GetComponent<AudioLowPassFilter> ();
		camTrans = GameObject.FindGameObjectWithTag ("CameraController").transform.Find ("CamOffset").transform;
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

		camLastEuler = camTrans.rotation.eulerAngles;

		StopAllCoroutines ();
		StartCoroutine (fadeOut ());
	}
	public void Unpause(){
		SetCameraEuler (camLastEuler);

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

	void Update(){
		if (paused) {	//krazykam
			Vector3 tempEuler = camTrans.rotation.eulerAngles;
			tempEuler += Vector3.up * CrossPlatformInputManager.GetAxisRaw("Horizontal") * Time.unscaledDeltaTime * 100;
			tempEuler += Vector3.right * CrossPlatformInputManager.GetAxisRaw("Vertical") * Time.unscaledDeltaTime * 100;
			SetCameraEuler (tempEuler);
			//camTrans.Rotate (new Vector3 (CrossPlatformInputManager.GetAxisRaw ("Vertical") * Time.unscaledDeltaTime * 100, CrossPlatformInputManager.GetAxisRaw ("Horizontal") * Time.unscaledDeltaTime * 100));
		}
	}

	private void SetCameraEuler(Vector3 euler){
		Quaternion tempQ = camTrans.rotation;
		tempQ.eulerAngles = euler;
		camTrans.rotation = tempQ;
	}
}
