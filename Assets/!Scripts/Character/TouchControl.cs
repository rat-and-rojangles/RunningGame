using UnityEngine;
using UnityEngine.UI;			//for debug messages on phone
using System.Collections;
using TouchScript;
using TouchScript.Gestures;

public class TouchControl : MonoBehaviour {

	private const float k_MaxTapTime = 0.25f;
	private const float k_MinSwipeSpeed = 2000f;		//pixels per second, i think

	private float startTime;

	private TapGesture tapGesture;
	private ScreenTransformGesture stGesture;

	private CharacterUserControl m_Control;

	private Text debugText;

	void Awake(){
		tapGesture = GetComponent<TapGesture>();
		stGesture = GetComponent<ScreenTransformGesture>();

		debugText = GameObject.FindGameObjectWithTag ("DebugMessage").GetComponent<Text> ();
		m_Control = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterUserControl> ();
	}

	void OnEnable () {
		tapGesture.Tapped += OnTap;
		stGesture.Transformed += OnTransform;
		stGesture.TransformCompleted += OnTransformEnd;
	}
	private void OnDisable()
	{
		tapGesture.Tapped -= OnTap;
		stGesture.Transformed -= OnTransform;
	}
	
	private void OnTap(object sender, System.EventArgs e){
		Write ("tapped " + RDigit ());
		m_Control.Tapped ();
	}

	private void OnTransformStart(object sender, System.EventArgs e){
		Write ("-start transform " + RDigit ());
	}
	private void OnTransformEnd(object sender, System.EventArgs e){
		Write ("!end transform " + RDigit ());
	}
	private void OnTransform(object sender, System.EventArgs e){
		Write ("+transforming " + RDigit ());
		if (Mathf.Abs (stGesture.DeltaPosition.x / Time.deltaTime) > k_MinSwipeSpeed) {
			if (stGesture.DeltaPosition.x < 0) {
				Write ("-left swipe " + RDigit ());
				m_Control.SwipeLeft ();
			}
			else if (stGesture.DeltaPosition.x > 0) {
				Write ("-right swipe " + RDigit ());
				m_Control.SwipeRight ();
			}
		}
	}

	private void Write(string message){
		Text o;
		debugText.text += "\n" + message;
	}
	private string RDigit(){
		return ((int)Random.Range (0, 10)).ToString ();
	}
}
