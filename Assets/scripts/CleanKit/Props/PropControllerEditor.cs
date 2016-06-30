using UnityEngine;
using UnityEditor;

namespace CleanKit
{
	[CustomEditor (typeof(PropController))]
	public class PropControllerEditor : Editor
	{

		public override void OnInspectorGUI ()
		{
			DrawDefaultInspector ();
			PropController propController = (PropController)target;
			if (GUILayout.Button ("Add Prop")) {
				propController.SpawnRandomProp ();
			}
		}
	}
}
