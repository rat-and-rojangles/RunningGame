using UnityEngine;
using System.Collections;

public class FadeBehind : MonoBehaviour {

	private Transform camTrans;
	private Transform playerTrans;
	private Material m_Mat;

	private const float k_Offset = 0.0f;
	private const float k_MinFade = 0.1f;
	private const float k_PauseFadeRate = 5.0f;

	private IEnumerator fadeIn;
	private IEnumerator fadeOut;

	private bool forgotten = false;
	private bool forgottenLast;

	private AutoMoveLevel autoMover;

	private const float k_CameraDistance = 10.0f;

	private RunnerCharacter player;
	private PauseControl pause;

	private Mesh m_Mesh;

	void Awake() {
		fadeIn = FadeIn ();
		fadeOut = FadeOut ();

		autoMover = GameObject.FindGameObjectWithTag ("GameController").GetComponent<AutoMoveLevel> ();
		m_Mat = GetComponent<Renderer> ().material;

		camTrans = GameObject.FindGameObjectWithTag ("MainCamera").transform;

		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<RunnerCharacter> ();
		playerTrans = GameObject.FindGameObjectWithTag ("Player").transform;
		pause = GameObject.FindGameObjectWithTag ("GameController").GetComponent<PauseControl> ();

		m_Mesh = GetComponent<MeshFilter> ().mesh;

		RefreshForgotten ();
		forgottenLast = forgotten;
	}

	private float FadeRate(){
		if (pause.IsPaused ()) {
			return k_PauseFadeRate;
		}
		else {
			return autoMover.speed / 4;
		}
	}

	void Update() {
		RefreshForgotten ();
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

	void RefreshForgotten() {
		if (ForgottenCondition ()) {
			forgotten = true;
		} 
		else {
			forgotten = false;
		}
	}

	private bool ForgottenCondition(){
		if (pause.IsPaused ()) {
			return Vector3.Distance (camTrans.position, transform.position) < k_CameraDistance;	//distance to camera
		}
		else if (player.GetSidestepMode ()) {
			return playerTrans.position.x > transform.position.x + (m_Mesh.bounds.extents.x)*transform.lossyScale.x - k_Offset;	//player is past
		}
		else {
			return false;
		}
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
