using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace CleanKit
{
	public static class GameObjectExtensions
	{
		public static void SetSelected (this GameObject gameObject, bool selected)
		{
			Color color = colorForSelected (selected);
			Renderer renderer = gameObject.GetComponentInChildren<Renderer> ();
			if (renderer) {
				renderer.material.color = color;
			} else if (gameObject.GetComponent<Image> ()) {
				gameObject.GetComponent<Image> ().color = color;
				if (gameObject.GetComponentInChildren<Text> ()) {
					gameObject.GetComponentInChildren<Text> ().color = colorForSelected (!selected);
				}
			}
		}

		static Color colorForSelected (bool selected)
		{
			return selected ? Color.red : Color.white;
		}

		public static GameObject HUDCanvas ()
		{
			return GameObject.Find ("/HUDCanvas"); 
		}

		public static List<GameObject> InteractableIndicators ()
		{
			return ChildGameObjectsForParentNamed ("InteractionController");
		}

		public static List<GameObject> ChildGameObjectsForParentNamed (string parentName)
		{
			GameObject parentObject = GameObject.Find (parentName);
			List<GameObject> objects = new List<GameObject> ();
			foreach (Transform childTransform in parentObject.transform) {
				objects.Add (childTransform.gameObject);
			}
			return objects;
		}
	}
}
