using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

namespace CleanKit
{
	public static class Controls
	{
		public static bool RelocationInputExists ()
		{
			return RelocationInput () != Vector3.zero && InputOverUI () == false;
		}

		public static Vector3 RelocationInput ()
		{
			if (pointerInputAvailable ()) {
				return Input.mousePosition;
			} else if (touchInputAvailable ()) {
				return Input.GetTouch (0).position;
			}
			return Vector3.zero;
		}

		public static bool InputOverUI ()
		{
			if (pointerInputAvailable ()) {
				return EventSystem.current.IsPointerOverGameObject ();
			} else if (touchInputAvailable ()) {
				return EventSystem.current.IsPointerOverGameObject (Input.GetTouch (0).fingerId);
			}
			return false;
		}

		public static bool InputExists ()
		{
			return pointerInputAvailable () || touchInputAvailable ();	
		}

		private static bool pointerInputAvailable ()
		{
			return Input.GetMouseButton (0);
		}

		private static bool touchInputAvailable ()
		{
			return Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began;	
		}

		public static bool InteractingWithScene ()
		{
			EventSystem eventSystem = EventSystem.current;
			GameObject interactingObject = eventSystem.currentSelectedGameObject;
			return interactingObject != null;
		}

		public static bool InteractingWithObjects (List<GameObject> gameObjects)
		{
			EventSystem eventSystem = EventSystem.current;
			GameObject interactingObject = eventSystem.currentSelectedGameObject;
			return gameObjects.Contains (interactingObject);
		}
	}
}