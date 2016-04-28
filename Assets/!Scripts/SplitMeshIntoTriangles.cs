// C#
// SplitMeshIntoTriangles.cs
using UnityEngine;
using System.Collections;

public class SplitMeshIntoTriangles : MonoBehaviour
{
	[SerializeField] private float m_Force = 100.0f;
	[SerializeField] private float m_Radius = 30.0f;

	private void SplitMesh () {
		//MeshFilter MF = GetComponent<MeshFilter>();
		MeshFilter MF = GetComponentInChildren<MeshFilter>();
		MeshRenderer MR = GetComponentInChildren<MeshRenderer>();
		Mesh M = MF.mesh;
		Vector3[] verts = M.vertices;
		Vector3[] normals = M.normals;
		Vector2[] uvs = M.uv;
		for (int submesh = 0; submesh < M.subMeshCount; submesh++)
		{
			int[] indices = M.GetTriangles(submesh);
			for (int i = 0; i < indices.Length; i += 3)
			{
				Vector3[] newVerts = new Vector3[3];
				Vector3[] newNormals = new Vector3[3];
				Vector2[] newUvs = new Vector2[3];
				for (int n = 0; n < 3; n++)
				{
					int index = indices[i + n];
					newVerts[n] = verts[index];
					//newUvs[n] = uvs[index];
					newNormals[n] = normals[index];
				}
				Mesh mesh = new Mesh();
				mesh.vertices = newVerts;
				mesh.normals = newNormals;
				//mesh.uv = newUvs;

				mesh.triangles = new int[] { 0, 1, 2, 2, 1, 0 };

				GameObject GO = new GameObject("Triangle " + (i / 3));
				GO.transform.position = transform.position;
				GO.transform.rotation = transform.rotation;
				GO.transform.localScale = transform.localScale;
				GO.AddComponent<MeshRenderer>().material = MR.materials[submesh];
				GO.AddComponent<MeshFilter>().mesh = mesh;
				GO.AddComponent<BoxCollider>();
				GO.AddComponent<Rigidbody>().AddExplosionForce(m_Force, transform.position, m_Radius);

				Destroy(GO, 5 + Random.Range(0.0f, 5.0f));
			}
		}
		MR.enabled = false;
		print ("end");
		Destroy(gameObject);
	}
	//void OnMouseDown()
	void Update()
	{
		if (Input.GetMouseButtonDown(0)) {
			print ("start");
			//StartCoroutine (SplitMesh ());
			SplitMesh();
		}
	}
}
