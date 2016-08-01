using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace CleanKit
{
	public partial class Interactable: MonoBehaviour
	{
		public static int LayerMask { get { return 1 << UnityEngine.LayerMask.NameToLayer ("Interactable"); } }

		float kHoverDistance = 2.0f;

		GameObject ghost;

		void Start ()
		{
			// TODO: Set destination

			int layermask = UnityEngine.LayerMask.NameToLayer ("Interactable");
			gameObject.layer = layermask;

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
			createGhost ();
			dragUpdated (data);
		}

		void dragUpdated (BaseEventData data)
		{
			PointerEventData pointerData = data as PointerEventData;
			Vector3 screenPosition = pointerData.position;
			Vector3 worldPosition = Camera.main.ScreenToWorldPoint (screenPosition);
			ghost.transform.position = worldPosition;

			Ray ray = Camera.main.ScreenPointToRay (screenPosition);
			RaycastHit hitInfo;
			int layerMask = Surface.LayerMask;

			Vector3 dragPoint = ghost.transform.position;
			dragPoint.y -= kHoverDistance;

			if (Physics.Raycast (dragPoint, ray.direction, out hitInfo, 100.0f, layerMask)) {
				Surface surface = hitInfo.transform.gameObject.GetComponent<Surface> ();
				surface.DisclosePoint (hitInfo.point);
			}
		}

		void dragEnded (BaseEventData data)
		{
			EventSystem.current.SetSelectedGameObject (null);
			destroyGhost ();
		}

		void createGhost ()
		{
			if (ghost != null) {
				return;
			}

			ghost = GameObject.Instantiate (gameObject);
			ghost.transform.SetParent (transform);
			ghost.GetComponent<Rigidbody> ().isKinematic = true;

			Shader ghostShader = Shader.Find ("CleanKit/Ghost");
			Material ghostMaterial = new Material (ghostShader);
			Renderer renderer = ghost.GetComponent<Renderer> ();
			renderer.material = ghostMaterial;
		}

		void destroyGhost ()
		{
			if (ghost == null) {
				return;
			}

			Destroy (ghost);
			ghost = null;
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

