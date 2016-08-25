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

		Destination _preferredDestination;

		public Destination preferredDestination { get { return _preferredDestination; } }

		public Destination specifiedDestination { get; private set; }

		void Start ()
		{
			int layermask = UnityEngine.LayerMask.NameToLayer ("Interactable");
			gameObject.layer = layermask;
		}

		public void SetPreferredPosition (Vector3 position)
		{
			if (_preferredDestination) {
				Destroy (_preferredDestination);
				_preferredDestination = null;
			}
			_preferredDestination = CreateDestination (position, GhostState.Off, "Preferred");
		}

		// TODO have this called outside EventTriggerType.Drag

		public void UpdateDragPosition (Vector3 newPosition)
		{
			if (specifiedDestination == null) {
				specifiedDestination = CreateDestination (transform.position, GhostState.Dimmed, "Specified");
			} else {
				specifiedDestination.SetGhostState (GhostState.Dimmed);
			}

			Vector3 screenPosition = newPosition;
			screenPosition.z = Camera.main.nearClipPlane;
			Vector3 worldPosition = Camera.main.ScreenToWorldPoint (screenPosition);
			Ray ray = Camera.main.ScreenPointToRay (screenPosition);
			RaycastHit hitInfo;
			int layerMask = Surface.LayerMask;

			Vector3 dragPoint = worldPosition;
			// TODO have ghost animate out of touch point
	
			InteractableGhost ghost = specifiedDestination.ghost;

			if (Physics.Raycast (dragPoint, ray.direction, out hitInfo, Camera.main.farClipPlane, layerMask)) {
				Surface surface = hitInfo.transform.gameObject.GetComponent<Surface> ();
				Vector3 hitPoint = hitInfo.point;
				hitPoint.y = Mathf.Round (hitPoint.y); // Fixes issue with raycast being too precise

				Vector3 size = ghost.GetComponent<Collider> ().bounds.size;
				bool valid = ghost.CollisionWithInteractables == false;
				Vector3 cellCenter = surface.DiscloseCells (hitPoint, valid, size);

				lastSurface = surface;
				specifiedDestination.transform.position = cellCenter;
			} else {
				undiscloseSurface ();
				worldPosition.y = Mathf.Min (Camera.main.farClipPlane, worldPosition.y);
				specifiedDestination.transform.position = worldPosition;
			}

			interactableDelegate.InteractableUpdatedMovement (this, specifiedDestination);
		}

		public void EndDragging ()
		{
			if (specifiedDestination.IsGhostPositionValid ()) {
				CommitSpecifiedDestination ();
			} else {
				DiscardSpecifiedDestination ();
				interactableDelegate.InteractableCancelledMovement (this);
			}
		}

		public Destination CreateDestination (Vector3 position, GhostState ghostState = GhostState.Off, string destinationName = "Destination")
		{
			InteractableGhost ghost = InteractableGhost.Instantiate (gameObject);
			ghost.gameObject.layer = 0;
				
			Destination destination = Destination.Instantiate (position, Vector3.up, ghost);

			destination.SetGhostState (ghostState);

			destination.name = gameObject.name + " (" + destinationName + ")";
			destination.gameObject.layer = 0;

			destination.transform.SetParent (gameObject.transform.parent);

			return destination;
		}

		void CommitSpecifiedDestination ()
		{
			undiscloseSurface ();
			specifiedDestination.SetGhostState (GhostState.Off);
			interactableDelegate.InteractableConfirmedDestination (this, specifiedDestination);
		}

		public void DiscardSpecifiedDestination ()
		{
			undiscloseSurface ();
			Destroy (specifiedDestination);
		}

		void undiscloseSurface ()
		{
			if (lastSurface != null) {
				lastSurface.Undisclose ();
			}
		}

		const float kScorableDistance = 50.0f;

		public float Score ()
		{
			// TODO extract this to allow different types of interactables to be scored individually
			float distance = preferredDestination.Distance (transform.position);
			float score = (kScorableDistance - distance) / kScorableDistance;
			return score;
		}
	}
}

