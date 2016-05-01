using UnityEngine;
using System.Collections;

public class FadeBehind : MonoBehaviour {

	private Transform camTrans;
	private Material m_Mat;

	private const float k_Offset = 0.5f;
	private const float k_MinFade = 0.05f;

	private IEnumerator fadeIn;
	private IEnumerator fadeOut;

	private bool forgotten = false;
	private bool forgottenLast;

	private AutoMoveLevel autoMover;

	private const float k_DisappearDistance = 13.0f;

	private RunnerCharacter player;
	private PauseControl pause;

	void Awake() {
		fadeIn = FadeIn ();
		fadeOut = FadeOut ();

		autoMover = GameObject.FindGameObjectWithTag ("GameController").GetComponent<AutoMoveLevel> ();
		m_Mat = GetComponent<Renderer> ().material;

		camTrans = GameObject.FindGameObjectWithTag ("MainCamera").transform;

		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<RunnerCharacter> ();
		pause = GameObject.FindGameObjectWithTag ("GameController").GetComponent<PauseControl> ();

		CheckForgotten ();
		forgottenLast = forgotten;
	}

	private float FadeRate(){
		return autoMover.speed / 4;
	}

	void Update() {
		CheckForgotten ();
	}

	void LateUpdate(){
		if (forgotten != forgottenLast) {
			if (forgotten) {
				Disappear ();
			}
			else {
				Reappear ();
			}
		}
		forgottenLast = forgotten;
	}

	void CheckForgotten() {
		if (Vector3.Distance(camTrans.position, transform.position) < k_DisappearDistance && DisappearEnabled ()) {
			forgotten = true;
		} 
		else {
			forgotten = false;
		}
	}

	private bool DisappearEnabled(){
		return player.GetSidestepMode () || pause.IsPaused ();
	}

	public void Reappear(){
		fadeIn = FadeIn ();
		StopCoroutine (fadeOut);
		StartCoroutine (fadeIn);
	}

	public void Disappear(){
		fadeOut = FadeOut ();
		StopCoroutine (fadeIn);
		StartCoroutine (fadeOut);
	}

	public bool IsForgotten(){
		return forgotten;
	}

	private IEnumerator FadeIn(){
		while (m_Mat.color.a < 1) {
			Color temp = m_Mat.color;
			temp.a += Time.unscaledDeltaTime * FadeRate ();
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
			temp.a -= Time.unscaledDeltaTime * FadeRate ();
			m_Mat.color = temp;
			yield return null;
		}
		Color tempFinal = m_Mat.color;
		tempFinal.a = k_MinFade;
		m_Mat.color = tempFinal;
	}
}
