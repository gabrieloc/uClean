using UnityEngine;
using UnityEngine.EventSystems;
using System;

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

		private static bool inputAvailable ()
		{
			return pointerInputAvailable () || touchInputAvailable ();	
		}

		private static bool pointerInputAvailable ()
		{
			return Input.GetMouseButtonDown (0);
		}

		private static bool touchInputAvailable ()
		{
			return Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began;	
		}

		public static bool PanInputExists (out GameObject panObject)
		{
			EventSystem eventSystem = EventSystem.current;
			panObject = eventSystem.currentSelectedGameObject;
			return eventSystem.alreadySelecting;
		}
	}
}