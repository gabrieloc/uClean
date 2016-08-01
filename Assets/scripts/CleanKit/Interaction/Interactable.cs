using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace CleanKit
{
	public partial class Interactable: MonoBehaviour
	{
		public static int LayerMask { get { return 1 << UnityEngine.LayerMask.NameToLayer ("interactable"); } }

		void Start ()
		{
			// TODO: Set destination

			EventTrigger eventTrigger = gameObject.AddComponent<EventTrigger> ();
			registerTriggerEntry (eventTrigger, EventTriggerType.BeginDrag, dragBegan);
			registerTriggerEntry (eventTrigger, EventTriggerType.Drag, dragUpdated);
			registerTriggerEntry (eventTrigger, EventTriggerType.EndDrag, dragEnded);
		}

		delegate void eventCallback (BaseEventData data);

		void registerTriggerEntry (EventTrigger eventTrigger, EventTriggerType type, eventCallback callback)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry ();
			entry.eventID = type;
			UnityAction<BaseEventData> action = new UnityAction<BaseEventData> (callback);
			entry.callback.AddListener (action);
			eventTrigger.triggers.Add (entry);
		}

		void dragBegan (BaseEventData data)
		{
			EventSystem.current.SetSelectedGameObject (gameObject);
		}

		void dragUpdated (BaseEventData data)
		{
			PointerEventData pointerData = data as PointerEventData;
			Vector3 screenPosition = pointerData.position;
			Vector3 worldPosition = Camera.main.ScreenToWorldPoint (screenPosition);
//			print ("point: " + screenPosition + " viewport: " + worldPosition);
			transform.position = worldPosition;
		}

		void dragEnded (BaseEventData data)
		{
			EventSystem.current.SetSelectedGameObject (null);
		}

		// TODO: Refactor all this to use grid-based approach

		const float kMinimumDistance = 50.0f;

		public float Score ()
		{
			// TODO extract this to allow different types of interactables to be scored individually
//			float distance = destination.Distance (transform.position);
//			float score = (kMinimumDistance - distance) / kMinimumDistance;
//			return score;
			return 0.5f;
		}
	}
}

