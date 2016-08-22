using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/*
 * Manages interaction with objects in scene
 * Interprets events and sends elsewhere
 * eg. tells BotController to pick up a liftable object, or to relocate bots
 */

namespace CleanKit
{

	public class InteractionController : MonoBehaviour
	{
		List<Interactable> availableInteractables = new List<Interactable> ();

		public List<Interactable> Interactables { get; private set; }

		Interactable selectedInteractable;
		EventTrigger eventTrigger;
		CameraController cameraController;

		void Start ()
		{
			Interactables =	new List<Interactable> (GameObject.FindObjectsOfType<Interactable> ());

			eventTrigger = GetComponent<EventTrigger> ();
			cameraController = Camera.main.GetComponent<CameraController> ();

			Controls.RegisterEvent (eventTrigger, EventTriggerType.PointerClick, pointerClicked);
			Controls.RegisterEvent (eventTrigger, EventTriggerType.BeginDrag, draggingBegan);
			Controls.RegisterEvent (eventTrigger, EventTriggerType.Drag, draggingUpdated);
			Controls.RegisterEvent (eventTrigger, EventTriggerType.EndDrag, draggingEnded);
		}

		void Update ()
		{
			layoutIndicatorsIfNecessary ();
		}

		void pointerClicked (BaseEventData eventData)
		{
			// forward to botController
		}

		void draggingBegan (BaseEventData eventData)
		{
			// TODO only select interactable if outside delay period
			PointerEventData pointerData = eventData as PointerEventData;
			Vector3 screenPosition = pointerData.position;

			Vector3 worldPosition = Camera.main.ScreenToWorldPoint (screenPosition);
			Ray ray = Camera.main.ScreenPointToRay (screenPosition);
			RaycastHit hitInfo;
			int layerMask = Interactable.LayerMask;
			if (Physics.Raycast (worldPosition, ray.direction, out hitInfo, Camera.main.farClipPlane, layerMask)) {
				GameObject hitObject = hitInfo.collider.gameObject;
				Interactable interactable = hitObject.GetComponent<Interactable> ();
				EventSystem.current.SetSelectedGameObject (interactable.gameObject);
			} else {
				cameraController.BeginPanning (screenPosition);
			}
		}

		void draggingUpdated (BaseEventData eventData)
		{
			PointerEventData pointerData = eventData as PointerEventData;
			Vector3 screenPosition = pointerData.position;

			GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
			if (currentSelected) {
				Interactable interactable = currentSelected.GetComponent<Interactable> ();
				if (interactable) {
					interactable.UpdateDragPosition (screenPosition);
				}
			} else {
				cameraController.UpdatePanPosition (screenPosition);
			}
		}

		void draggingEnded (BaseEventData eventData)
		{
			GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
			if (currentSelected) {
				Interactable interactable = currentSelected.GetComponent<Interactable> ();
				if (interactable) {
					EventSystem.current.SetSelectedGameObject (null);
					interactable.EndDragging ();
				}
			}
		}

		void layoutIndicatorsIfNecessary ()
		{
			if (availableInteractables.Count > 0) {
				foreach (Interactable interactable in availableInteractables) {
					interactable.LayoutIndicators ();
				}	
			}
		}

		public void SetInteractableAvailable (Interactable interactable, Actor actor, bool available)
		{
			if (availableInteractables.Contains (interactable) == false && available) {
				availableInteractables.Add (interactable);

				interactable.BecomeAvailableForActor (actor);
				foreach (InteractableIndicator indicator in interactable.indicators) {
					indicator.gameObject.transform.SetParent (transform);
				}
			} else if (availableInteractables.Contains (interactable) == true && !available) {
				availableInteractables.Remove (interactable);
				interactable.BecomeUnavailable ();
			}
		}
	}
}

