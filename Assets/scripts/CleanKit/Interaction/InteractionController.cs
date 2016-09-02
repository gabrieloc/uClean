using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using System;
using System.Linq;
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

		public List<Interactable> EvaluatableInstructables {
			get {
				return interactables.FindAll (i => i.CanBeEvaluated ());
			}
		}

		List<Interactable> interactables = new List<Interactable> ();

		Interactable selectedInteractable;
		EventTrigger eventTrigger;
		CameraController cameraController;
		public BotController botController;

		void Start ()
		{
			interactables =	new List<Interactable> (GameObject.FindObjectsOfType<Interactable> ());

			eventTrigger = GetComponent<EventTrigger> ();
			cameraController = Camera.main.GetComponent<CameraController> ();

			EventAdditions.RegisterEvent (eventTrigger, EventTriggerType.PointerClick, pointerClicked);
			EventAdditions.RegisterEvent (eventTrigger, EventTriggerType.BeginDrag, draggingBegan);
			EventAdditions.RegisterEvent (eventTrigger, EventTriggerType.Drag, draggingUpdated);
			EventAdditions.RegisterEvent (eventTrigger, EventTriggerType.EndDrag, draggingEnded);
			EventAdditions.RegisterEvent (eventTrigger, EventTriggerType.Scroll, scrollUpdated);
		}

		void pointerClicked (BaseEventData eventData)
		{
			PointerEventData pointerData = eventData as PointerEventData;
			Vector3 screenPosition = pointerData.position;
			Vector3 worldPosition = Camera.main.ScreenToWorldPoint (screenPosition);
			Ray ray = Camera.main.ScreenPointToRay (screenPosition);
			RaycastHit hitInfo;
			if (Physics.Raycast (worldPosition, ray.direction, out hitInfo, Camera.main.farClipPlane)) {

				GameObject hitObject = hitInfo.collider.gameObject;
				Vector3 hitPoint = hitInfo.point;
				int hitLayer = (1 << hitObject.layer);

				if (Surface.LayerMask == (Surface.LayerMask | hitLayer)) {
					botController.RelocateToPosition (hitPoint, hitInfo.normal);
				} else if (Bot.LayerMask == (Bot.LayerMask | hitLayer)) {
					botController.SelectBot (hitObject.GetComponentInParent<Bot> ());
				} else if (Interactable.LayerMask == (Interactable.LayerMask | hitLayer)) {
					Interactable interactable = hitObject.GetComponent<Interactable> ();
					interactable.RevealPreferredDestination ();
					// TODO: should toggle
				}
			}
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

		void scrollUpdated (BaseEventData eventData)
		{
			PointerEventData pointerData = eventData as PointerEventData;
			float scroll = pointerData.scrollDelta.y;
			cameraController.UpdateZoomValue (scroll);
			print (scroll);
		}

		#if !UNITY_EDITOR
		
		void Update ()
		{
			// TODO consider rebuilding this to use event system?

			bool zoomInputExists = Input.touchCount == 2;
			eventTrigger.enabled = !zoomInputExists;
			if (zoomInputExists) {

				Touch firstTouch = Input.GetTouch (0);
				Touch secondTouch = Input.GetTouch (1);

				Vector2 firstTouchDelta = firstTouch.position - firstTouch.deltaPosition;
				Vector2 secondTouchDelta = secondTouch.position - secondTouch.deltaPosition;

				float previousMagnitude = (firstTouchDelta - secondTouchDelta).magnitude;
				float magnitude = (firstTouch.position - secondTouch.position).magnitude;

				float deltaMagnitude = previousMagnitude - magnitude;
				cameraController.UpdateZoomValue (deltaMagnitude * -0.1f);
			}
		}

		#endif
	}
}

