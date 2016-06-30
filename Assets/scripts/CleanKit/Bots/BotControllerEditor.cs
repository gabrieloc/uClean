using UnityEngine;
using UnityEditor;

namespace CleanKit
{
	[CustomEditor (typeof(BotController))]

	public class BotControllerEditor : Editor
	{
		public override void OnInspectorGUI ()
		{
			DrawDefaultInspector ();
			BotController botController = (BotController)target;
			if (GUILayout.Button ("Add Bot")) {
				botController.AddBot ();			
			}
		}
	}
}