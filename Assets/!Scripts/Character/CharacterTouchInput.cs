using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharacterTouchInput : MonoBehaviour {
	private Vector2 startPos;
	private float startTime;

	private const float k_MaxTapTime = 0.33f;
	private const float k_MaxTapDistance = 10f;

	private const float k_MinSwipeSpeed = 2000f;		//pixels per second, i think

	private float m_DragDistance;

	private CharacterUserControl control;
	RunnerCharacter m_Character;
	//private Text message;

	public void Awake(){
		//Input.multiTouchEnabled = false;
		control = GetComponent<CharacterUserControl> ();
		m_Character = GameObject.FindGameObjectWithTag ("Player").GetComponent<RunnerCharacter> ();
		//message = GameObject.FindGameObjectWithTag ("DebugMessage").GetComponent<Text> ();
	}

	void Update () {
		if (Input.touchCount > 0) {		//basically "if touched" with multitouch disabled
			Touch touch = Input.GetTouch (0);
			if (touch.phase == TouchPhase.Began) {
				startPos = touch.rawPosition;
				startTime = Time.timeSinceLevelLoad;
			} else if (touch.phase == TouchPhase.Ended) {
				if (Time.timeSinceLevelLoad - startTime < k_MaxTapTime && Mathf.Abs(touch.rawPosition.x - startPos.x) < k_MaxTapDistance) {
					control.Tapped ();
					DebugWrite ("Tap" + touch.rawPosition.ToString () +RDigit());
				}
			} else if (touch.phase == TouchPhase.Moved) {
				if (Mathf.Abs(touch.deltaPosition.x/touch.deltaTime) > k_MinSwipeSpeed) {
					if (touch.deltaPosition.x < 0) {
						control.SwipeLeft ();
						DebugWrite ("LeftSwipe" + touch.deltaPosition.ToString ());
					}
					else if (touch.deltaPosition.x > 0) {
						control.SwipeRight ();
						DebugWrite ("RightSwipe" + touch.deltaPosition.ToString ());
					}
				}
			}
		}

		bool mouseEnabled = false;
		if (mouseEnabled) {
			if (Input.GetMouseButtonDown (0)) {
				DebugWrite ("click down");
				startPos = Input.mousePosition;
				startTime = Time.timeSinceLevelLoad;
			} else if (Input.GetMouseButtonUp (0)) {
				if (Time.timeSinceLevelLoad - startTime < k_MaxTapTime) {
					DebugWrite ("click up fake tap");
					control.Tapped ();
				}
			}
		}
	}


	private void DebugWrite(string message){
		Text o = GameObject.FindGameObjectWithTag ("DebugMessage").GetComponent<Text> ();
		o.text += "\n"+ message;
	}
	private string RDigit(){
		return ((int)Random.Range (0, 10)).ToString ();
	}
}
