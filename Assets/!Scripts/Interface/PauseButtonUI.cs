using UnityEngine;
using System.Collections;
using TouchScript;
using TouchScript.Gestures;

public class PauseButtonUI : MonoBehaviour {

	private TapGesture tapGesture;
	private LongPressGesture lpGesture;
	private PauseControl pauseControl;

	void Awake () {
		tapGesture = GetComponent<TapGesture>();
		lpGesture = GetComponent<LongPressGesture>();
		pauseControl = GameObject.FindGameObjectWithTag ("GameController").GetComponent<PauseControl> ();
	}

	void OnEnable () {
		tapGesture.Tapped += OnTap;
		lpGesture.LongPressed += OnLP;
	}
	private void OnDisable()
	{
		tapGesture.Tapped -= OnTap;
		lpGesture.LongPressed -= OnLP;
	}

	private void OnTap(object sender, System.EventArgs e){
		pauseControl.TogglePause();
	}
	private void OnLP(object sender, System.EventArgs e){
		GameObject.FindGameObjectWithTag ("Player").GetComponent<RunnerCharacter> ().Restart ();
	}
}
