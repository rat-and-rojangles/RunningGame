using UnityEngine;
using System.Collections;

public class CamShaker : MonoBehaviour {

	[SerializeField] private float duration = 0.25f;
	//[SerializeField] private float magnitude = 1.0f;

	private const float k_TimeCompensation = 50.0f;

	private Transform m_CamOffset;

	void Awake(){
		m_CamOffset = GameObject.FindGameObjectWithTag ("CameraController").transform;
	}

	private IEnumerator ShakeCoroutine(float magnitude) {

		float elapsed = 0.0f;
		float xTotal = 0.0f;
		float zTotal = 0.0f;

		while (elapsed < duration) {

			elapsed += Time.unscaledDeltaTime;          

			float percentComplete = elapsed / duration;         
			float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

			// map value to [-1, 1]
			float x = Random.value * 2.0f - 1.0f;
			float y = Random.value * 2.0f - 1.0f;
			float z = Random.value * 2.0f - 1.0f;
			x *= magnitude * damper;
			y *= magnitude * damper;
			z *= magnitude * damper;

			xTotal += x;
			zTotal += z;

			m_CamOffset.position += ((Vector3.right * x) + (Vector3.up * y) + (Vector3.forward * z)) * Time.unscaledDeltaTime * k_TimeCompensation;

			yield return null;
		}

		int undoSegments = 3;
		for (int counter = 0; counter < undoSegments; counter++) {
			m_CamOffset.position += (Vector3.left * xTotal/undoSegments);	//undoes x shift
			m_CamOffset.position += (Vector3.back * zTotal/undoSegments);	//undoes z shift
			yield return null;
		}

		//m_CamOffset.position += (Vector3.left * xTotal);	//undoes x shift
		//m_CamOffset.position += (Vector3.back * zTotal);	//undoes z shift
	}

	public void Shake(float mag){
		StartCoroutine (ShakeCoroutine (mag));
	}
}

