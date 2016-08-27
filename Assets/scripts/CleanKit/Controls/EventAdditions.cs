using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace CleanKit
{
	public static class EventAdditions
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
	}
}