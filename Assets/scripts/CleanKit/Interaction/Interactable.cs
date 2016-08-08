using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace CleanKit
{
	public interface InteractableDelegate
	{
		void InstructionCreated (Interactable interactable, Instruction instruction);
	}

	public partial class Interactable: MonoBehaviour
	{
		public InteractableDelegate interactableDelegate;

		public static int LayerMask { get { return 1 << UnityEngine.LayerMask.NameToLayer ("Interactable"); } }

		InteractableGhost ghost;

		Surface lastSurface;

		Instruction currentInstruction;

		void Start ()
		{
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
			screenPosition.z = Camera.main.nearClipPlane;
			Vector3 worldPosition = Camera.main.ScreenToWorldPoint (screenPosition);

			Ray ray = Camera.main.ScreenPointToRay (screenPosition);
			RaycastHit hitInfo;
			int layerMask = Surface.LayerMask;

			Vector3 dragPoint = worldPosition;
			// TODO have ghost animate out of touch point
	
			if (Physics.Raycast (dragPoint, ray.direction, out hitInfo, 100.0f, layerMask)) {
				Surface surface = hitInfo.transform.gameObject.GetComponent<Surface> ();
				Vector3 hitPoint = hitInfo.point;
				hitPoint.y = Mathf.Round (hitPoint.y); // Fixes issue with raycast being too precise

				Vector3 size = ghost.GetComponent<Collider> ().bounds.size;
				bool valid = ghost.CollisionWithInteractables == false;
				Vector3 cellCenter = surface.DiscloseCells (hitPoint, valid, size);

				lastSurface = surface;
				ghost.transform.position = cellCenter;
			} else {
				undiscloseSurface ();
				ghost.SetDraggingTransform (worldPosition);
			}
		}

		void dragEnded (BaseEventData data)
		{
			EventSystem.current.SetSelectedGameObject (null);
			bool validPosition = ghost.CollisionWithInteractables == false;
			destroyGhost ();

			if (validPosition) {
				Instruction instruction = new Instruction ();
				instruction.assignee = this;
				instruction.interactionType = InteractionType.Move;
				currentInstruction = instruction;
				interactableDelegate.InstructionCreated (this, instruction);
			}
		}

		void createGhost ()
		{
			if (ghost != null) {
				destroyGhost ();
			}

			ghost = InteractableGhost.Instantiate (gameObject.GetComponent<Interactable> ());
			ghost.transform.SetParent (transform);
		}

		void destroyGhost ()
		{
			if (ghost != null) {
				undiscloseSurface ();
				Destroy (ghost.gameObject);
				ghost = null;
			}
		}

		void undiscloseSurface ()
		{
			if (lastSurface != null) {
				lastSurface.Undisclose ();
			}
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

