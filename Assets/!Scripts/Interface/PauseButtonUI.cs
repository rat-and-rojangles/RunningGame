using UnityEngine;
using System.Collections;
using TouchScript;
using TouchScript.Gestures;

public class PauseButtonUI : MonoBehaviour {

	private TapGesture tapGesture;
	private PauseControl pauseControl;

	void Awake () {
		tapGesture = GetComponent<TapGesture>();
		pauseControl = GameObject.FindGameObjectWithTag ("GameController").GetComponent<PauseControl> ();
	}

	void OnEnable () {
		tapGesture.Tapped += OnTap;
	}
	private void OnDisable()
	{
		tapGesture.Tapped -= OnTap;
	}

	private void OnTap(object sender, System.EventArgs e){
		pauseControl.TogglePause();
	}
}
