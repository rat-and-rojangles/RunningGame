using UnityEngine;
using System.Collections;

public class LooseCameraFollow : MonoBehaviour {

	[SerializeField] private float m_Radius = 10.0f;	// 1 is perfect follow, larger numbers mean less movement
	private Transform m_Target;

	void Awake(){
		m_Target = GameObject.FindGameObjectWithTag ("Player").transform;
	}
		
	void LateUpdate () {
		float distance = Vector2.Distance(transform.position, m_Target.position);

		Vector3 lerped = Vector3.Lerp (transform.position, m_Target.position, distance / m_Radius);
		Vector3 newPos = new Vector3 (lerped.x, lerped.y, transform.position.z);

		transform.position = newPos;
	}
}
