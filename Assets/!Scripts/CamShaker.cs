using UnityEngine;
using System.Collections;

public class CamShaker : MonoBehaviour {

	[SerializeField] private float duration = 0.25f;
	[SerializeField] private float magnitude = 1.0f;

	private const float k_TimeCompensation = 50.0f;

	private Transform m_CamOffset;

	void Awake(){
		m_CamOffset = GameObject.FindGameObjectWithTag ("CameraController").transform;
	}

	private IEnumerator ShakeCoroutine() {

		float elapsed = 0.0f;
		float xTotal = 0.0f;

		//Vector3 originalCamPos = Camera.main.transform.position;

		while (elapsed < duration) {

			elapsed += Time.deltaTime;          

			float percentComplete = elapsed / duration;         
			float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

			// map value to [-1, 1]
			float x = Random.value * 2.0f - 1.0f;
			float y = Random.value * 2.0f - 1.0f;
			x *= magnitude * damper;
			y *= magnitude * damper;

			xTotal += x;

			//Camera.main.transform.position = new Vector3(x, y, originalCamPos.z);
			m_CamOffset.position += ((Vector3.right * x) + (Vector3.up * y)) * Time.deltaTime * k_TimeCompensation;

			yield return null;
		}

		m_CamOffset.position += (Vector3.left * xTotal);	//undoes x shift

	}

	public void Shake(){
		StartCoroutine (ShakeCoroutine ());
	}
}
