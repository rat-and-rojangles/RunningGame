using UnityEngine;
using System.Collections;

public class Helmet : MonoBehaviour {

	public Transform head;

	// Use this for initialization
	void Awake () {
		transform.parent = head;
	}
}
