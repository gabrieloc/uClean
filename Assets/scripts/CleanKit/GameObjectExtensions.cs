using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace CleanKit
{
	public static class GameObjectExtensions
	{
		public static void SetSelected (this GameObject gameObject, bool selected)
		{
			Color color = selected ? Color.blue : Color.gray;
			Renderer renderer = gameObject.GetComponent<Renderer> ();
			if (renderer) {
				renderer.material.color = color;
			} else if (gameObject.GetComponent<Image> ()) {
				gameObject.GetComponent<Image> ().color = color;
			}
		}

		public static GameObject HUDCanvas ()
		{
			return GameObject.Find ("/HUDCanvas"); 
		}

		public static GameObject AvatarContainer ()
		{
			return GameObject.Find ("AvatarContainer");
		}

		public static GameObject[] AvatarObjects ()
		{
			return GameObject.FindGameObjectsWithTag ("avatar");
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
