using UnityEngine;
using UnityEngine.UI;			//for debug messages on phone
using System.Collections;
using TouchScript;
using TouchScript.Gestures;

public class TouchControlCharacter : MonoBehaviour {

	private const float k_MaxTapTime = 0.25f;
	private const float k_MinSwipeSpeed = 2000f;		//pixels per second, i think

	private float startTime;

	private TapGesture tapGesture;
	private ScreenTransformGesture stGesture;

	private CharacterUserControl m_Control;

	void Awake(){
		tapGesture = GetComponent<TapGesture>();
		stGesture = GetComponent<ScreenTransformGesture>();
		m_Control = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterUserControl> ();
	}

	void OnEnable () {
		tapGesture.Tapped += OnTap;
		stGesture.Transformed += OnTransform;
	}
	private void OnDisable()
	{
		tapGesture.Tapped -= OnTap;
		stGesture.Transformed -= OnTransform;
	}
	
	private void OnTap(object sender, System.EventArgs e){
		m_Control.Tapped (tapGesture.NormalizedScreenPosition.x);
	}


	private void OnTransform(object sender, System.EventArgs e){
		if (Mathf.Abs (stGesture.DeltaPosition.x / Time.deltaTime) > k_MinSwipeSpeed) {
			if (stGesture.DeltaPosition.x < 0) {
				m_Control.SwipeLeft ();
			}
			else if (stGesture.DeltaPosition.x > 0) {
				m_Control.SwipeRight ();
			}
		}
	}
}
