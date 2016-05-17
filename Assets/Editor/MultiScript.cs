using UnityEngine;
using UnityEditor;
using System.Collections;

public class MultiScript : EditorWindow
{
	private MonoScript s;
	private string c;
	private bool addToChildren = false;

	[MenuItem("Window/MultiScript")]
	static void Init()
	{
		MultiScript window = (MultiScript)EditorWindow.GetWindow(typeof(MultiScript));
	}

	void OnGUI()
	{
		s = EditorGUILayout.ObjectField("Script to add", s, typeof(MonoScript)) as MonoScript;

		c = "";
		if (Selection.gameObjects.Length > 0)
		{
			for (int i = 0; i < Mathf.Min(10, Selection.gameObjects.Length); i++)
			{
				c += Selection.gameObjects[i].name + "\n";
			}

			if (Selection.gameObjects.Length > 10)
				c += "(" + (Selection.gameObjects.Length - 10) + " more objects)";
		}

		if (s != null)
		{
			addToChildren = GUILayout.Toggle(addToChildren, "Add to all children (recursive add)");
			GUILayout.Label("Objects to receive " + s.name + ":\n\n" + c);
			if (GUILayout.Button("Add " + s.name + " to all " + Selection.gameObjects.Length + " objects")) addScriptToObjects();
		}
		else
			GUILayout.Label("Select a script first");
	}

	void OnInspectorUpdate()
	{
		Repaint();
	}

	private void addScriptToObjects()
	{
		if (Selection.gameObjects.Length > 0)
		{
			foreach (GameObject o in Selection.gameObjects)
			{
				UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(o, "Assets/Editor/MultiScript.cs (54,5)", s.name);

				if (addToChildren)
				{
					recursiveAdd(o);
				}
			}
		}
	}

	private void recursiveAdd(GameObject o)
	{
		foreach (Transform t in o.transform)
		{
			UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(t.gameObject, "Assets/Editor/MultiScript.cs (68,4)", s.name);

			if (t.childCount > 0)
				recursiveAdd(t.gameObject);
		}
	}
}