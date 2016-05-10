using UnityEngine;
using UnityEngine.EventSystems;

// TODO interprate touch controls too
public static class Controls
{
		public static bool RelocationInputExists() 
		{
			return RelocationInput() != Vector3.zero && InputOverUI() == false;
		}

		public static Vector3 RelocationInput() 
		{
			if (PointerInputAvailable()) {
				return Input.mousePosition;
			}
			else if (TouchInputAvailable()) {
				return Input.GetTouch(0).position;
			}
			return Vector3.zero;
		}

		public static bool InputOverUI()
		{
			if (PointerInputAvailable()) {
				return EventSystem.current.IsPointerOverGameObject();
			}
			else if (TouchInputAvailable()) {
				return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
			}
			return false;
		}

		private static bool PointerInputAvailable() 
		{
			return Input.GetMouseButtonDown(0);
		}

		private static bool TouchInputAvailable()
		{
			return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;	
		}
	}