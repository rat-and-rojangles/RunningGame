using UnityEngine;
using System.Collections;

public class LooseCameraFollow : MonoBehaviour {

	[SerializeField] private float m_Radius = 10.0f;	// 1 is perfect follow, larger numbers mean less movement
	[SerializeField] private GameObject m_Target;


	//void FixedUpdate () {
	void LateUpdate () {
		//Vector3 lerped = Vector3.Lerp (transform.position, m_Target.transform.position, m_Strictness);
		float distance = Vector2.Distance(transform.position, m_Target.transform.position);

		Vector3 lerped = Vector3.Lerp (transform.position, m_Target.transform.position, distance / m_Radius);
		Vector3 newPos = new Vector3 (lerped.x, lerped.y, transform.position.z);

		transform.position = newPos;

		//  distance/radius
		//  perfect (0) = 
		//  outer   (1) = 


		//print (transform.position);
	}
}
