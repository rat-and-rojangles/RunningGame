using UnityEngine;
using System.Collections;

public class LooseCameraFollow : MonoBehaviour {

	private const float m_Radius = 25.0f;	// 1 is perfect follow, larger numbers mean less movement
	private Transform m_Target;

	//private bool followX = true;
	//private bool followY = true;
	//private bool followZ = true;

	[SerializeField] private float m_Speed = 50.0f;

	void Awake(){
		m_Target = GameObject.FindGameObjectWithTag ("Player").transform;
		transform.position = m_Target.position;
	}
		
	void LateUpdate () {
		float distance = Mathf.Abs(transform.position.y - m_Target.position.y);
		//float distance = Vector2.Distance(transform.position, m_Target.position);


		//float factor = Mathf.Clamp01(distance / m_Radius * m_Speed * Time.unscaledDeltaTime) -0.01f;
		//Vector3 lerped = Vector3.Lerp (transform.position, m_Target.position, factor);
		Vector3 lerped = Vector3.Lerp (transform.position, m_Target.position, distance / m_Radius * m_Speed * Time.unscaledDeltaTime);

		//Vector3 newPos = new Vector3 (m_Target.position.x, lerped.y, transform.position.z);
		Vector3 newPos = new Vector3 (transform.position.x, lerped.y, transform.position.z);

		transform.position = newPos;
	}
}
