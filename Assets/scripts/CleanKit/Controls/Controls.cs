using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

namespace CleanKit
{
	public static class Controls
	{
		public	delegate void EventCallback (BaseEventData data);

		public static void RegisterEvent (EventTrigger eventTrigger, EventTriggerType type, EventCallback callback)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry ();
			entry.eventID = type;
			UnityAction<BaseEventData> action = new UnityAction<BaseEventData> (callback);
			entry.callback.AddListener (action);
			eventTrigger.triggers.Add (entry);
		}

		public static bool InputExists ()
		{
			return CurrentInput ().HasValue && !InputOverUI ();
		}

		static Vector3? CurrentInput ()
		{
			if (pointerInputAvailable ()) {
				return Input.mousePosition;
			} else if (touchInputAvailable ()) {
				return Input.GetTouch (0).position;
			}
			return null;
		}

		public static bool InputOverUI ()
		{
			bool selectedNotNull = EventSystem.current.currentSelectedGameObject != null;
			bool overGameObject = false;
			if (pointerInputAvailable ()) {
				overGameObject = EventSystem.current.IsPointerOverGameObject ();
			} else if (touchInputAvailable ()) {
				overGameObject = EventSystem.current.IsPointerOverGameObject (Input.GetTouch (0).fingerId);
			}
			return overGameObject && selectedNotNull;
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
			return interactingObject ? gameObjects.Contains (interactingObject) : false;
		}
	}
}