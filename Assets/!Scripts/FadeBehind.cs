using UnityEngine;
using System.Collections;

public class FadeBehind : MonoBehaviour {

	//private Transform playerTrans;
	private Transform perfectPos;

	private RunnerCharacter playerControl;

	private Material m_Mat;

	private const float k_FadeRate = 5.0f;
	private const float k_MinFade = 0.05f;

	private IEnumerator fadeIn;
	private IEnumerator fadeOut;

	private bool forgotten = false;
	private bool forgottenLast;


	void Awake() {
		fadeIn = FadeIn ();
		fadeOut = FadeOut ();

		//playerTrans = GameObject.FindGameObjectWithTag ("Player").transform;
		playerControl = GameObject.FindGameObjectWithTag ("Player").GetComponent<RunnerCharacter>();
		perfectPos = GameObject.FindGameObjectWithTag ("CameraController").transform.Find ("SidestepPosition").transform;
		m_Mat = GetComponent<Renderer> ().material;

		CheckBehind ();
		forgottenLast = forgotten;
	}

	void Update() {
		CheckBehind ();
	}

	void LateUpdate(){
		if (forgotten != forgottenLast) {
			if (forgotten) {
				print ("4gotten");
				fadeOut = FadeOut ();
				StopCoroutine (fadeIn);
				StartCoroutine (fadeOut);
			}
			else {
				print ("5gotten");
				fadeIn = FadeIn ();
				StopCoroutine (fadeOut);
				StartCoroutine (fadeIn);
			}
		}
		forgottenLast = forgotten;
	}

	void CheckBehind() {
		//if (playerTrans.position.x > perfectPos.position.x && playerControl.GetSidestepMode ()) {
		if (perfectPos.position.x > transform.position.x && playerControl.GetSidestepMode ()) {
			forgotten = true;
		} 
		else {
			forgotten = false;
		}
	}

	private IEnumerator FadeIn(){
		while (m_Mat.color.a < 1) {
			Color temp = m_Mat.color;
			temp.a += Time.deltaTime * k_FadeRate;
			m_Mat.color = temp;
			yield return null;
		}
		Color tempFinal = m_Mat.color;
		tempFinal.a = 1.0f;
		m_Mat.color = tempFinal;
	}

	private IEnumerator FadeOut(){
		while (m_Mat.color.a > k_MinFade) {
			Color temp = m_Mat.color;
			temp.a -= Time.deltaTime * k_FadeRate;
			m_Mat.color = temp;
			yield return null;
		}
		Color tempFinal = m_Mat.color;
		tempFinal.a = k_MinFade;
		m_Mat.color = tempFinal;
	}
}
