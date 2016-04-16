using UnityEngine;
using System.Collections;

public class LooseCameraFollowVertical : MonoBehaviour {

	[SerializeField] private float m_Radius = 25.0f;	// 1 is perfect follow, larger numbers mean less movement
	private Transform m_Target;

	void Awake(){
		m_Target = GameObject.FindGameObjectWithTag ("Player").transform;
	}


	void LateUpdate () {
		float distance = Mathf.Abs(transform.position.y - m_Target.position.y);

		Vector3 lerped = Vector3.Lerp (transform.position, m_Target.position, distance / m_Radius * 50 * Time.unscaledDeltaTime);
		Vector3 newPos = new Vector3 (transform.position.x, lerped.y, transform.position.z);

		transform.position = newPos;

	}
}
