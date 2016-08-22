using UnityEngine;
using System.Collections.Generic;

namespace CleanKit
{
	public interface InteractableDelegate
	{
		void InteractableUpdatedMovement (Interactable interactable, Destination destination);

		void InteractableCancelledMovement (Interactable interactable);

		void InteractableConfirmedDestination (Interactable interactable, Destination destination);
	}

	public partial class Interactable: MonoBehaviour
	{
		public InteractableDelegate interactableDelegate;

		public static int LayerMask { get { return 1 << UnityEngine.LayerMask.NameToLayer ("Interactable"); } }

		Surface lastSurface;

		void Start ()
		{
			int layermask = UnityEngine.LayerMask.NameToLayer ("Interactable");
			gameObject.layer = layermask;
		}

		// Instructions

		public bool IsGhostVisible ()
		{
			return destination != null && destination.IsGhostVisible ();
		}

		public void SetGhostVisible (bool visible, bool highlighted = false)
		{
			destination.SetGhostVisible (visible, highlighted);
		}

		// TODO have this called outside EventTriggerType.Drag

		public void UpdateDragPosition (Vector3 newPosition)
		{
			if (destination == null) {
				createDestination ();
			}

			Vector3 screenPosition = newPosition;
			screenPosition.z = Camera.main.nearClipPlane;
			Vector3 worldPosition = Camera.main.ScreenToWorldPoint (screenPosition);
			Ray ray = Camera.main.ScreenPointToRay (screenPosition);
			RaycastHit hitInfo;
			int layerMask = Surface.LayerMask;

			Vector3 dragPoint = worldPosition;
			// TODO have ghost animate out of touch point
	
			InteractableGhost ghost = destination.ghost;

			if (Physics.Raycast (dragPoint, ray.direction, out hitInfo, Camera.main.farClipPlane, layerMask)) {
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
				worldPosition.y = Mathf.Min (Camera.main.farClipPlane, worldPosition.y);
				ghost.SetDraggingTransform (worldPosition);
			}

			interactableDelegate.InteractableUpdatedMovement (this, destination);
		}

		public void EndDragging ()
		{
			if (destination.IsGhostPositionValid ()) {
				undiscloseSurface ();
				destination.SetGhostVisible (false);
				interactableDelegate.InteractableConfirmedDestination (this, destination);
			} else {
				discardDestination ();
				interactableDelegate.InteractableCancelledMovement (this);
			}
		}

		void createDestination (bool highlight = false)
		{
			InteractableGhost ghost = InteractableGhost.Instantiate (gameObject);
			ghost.gameObject.layer = 0;
				
			destination = Destination.Instantiate (transform.position, Vector3.up, ghost);
			destination.transform.SetParent (transform.parent);

			destination.SetGhostVisible (true, false);

			destination.name = gameObject.name + " (Destination)";
			destination.gameObject.layer = 0;
		}

		void discardDestination ()
		{
			undiscloseSurface ();
			Destroy (destination);
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

