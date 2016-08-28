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

		Destination preferredDestination {
			get {
				return _preferredDestination;
			}
			set {
				if (_preferredDestination != null) {
					Destroy (_preferredDestination);
					_preferredDestination = null;
				}
				_preferredDestination = value;
			}
		}

		Destination specifiedDestination;

		void Start ()
		{
			int layermask = UnityEngine.LayerMask.NameToLayer ("Interactable");
			gameObject.layer = layermask;
		}

		void Update ()
		{
			if (transform.position.y < 0) {
				Vector3 position = transform.position;
				position.y = 1;
				transform.position = position;
			}	
		}

		public void SetPreferredPosition (Vector3 position)
		{
			preferredDestination = CreateDestination (position, GhostState.Off, "Preferred");
		}

		public void RevealPreferredDestination ()
		{
			preferredDestination.SetGhostState (GhostState.Bright);
		}

		// TODO have this called outside EventTriggerType.Drag

		public void UpdateDragPosition (Vector3 newPosition)
		{
			if (specifiedDestination == null) {
				specifiedDestination = CreateDestination (transform.position, GhostState.Dimmed, "Specified");
			} else {
				specifiedDestination.SetGhostState (GhostState.Dimmed);
			}

			preferredDestination.SetGhostState (GhostState.Bright);

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

		public bool HasSpecifiedDestination ()
		{
			return specifiedDestination != null;
		}

		void CommitSpecifiedDestination ()
		{
			undiscloseSurface ();
			specifiedDestination.SetGhostState (GhostState.Dimmed);
			interactableDelegate.InteractableConfirmedDestination (this, specifiedDestination);
		}

		public void DiscardSpecifiedDestination ()
		{
			undiscloseSurface ();
			Destroy (specifiedDestination.gameObject);
			preferredDestination.SetGhostState (GhostState.Off);
		}

		void undiscloseSurface ()
		{
			if (lastSurface != null) {
				lastSurface.Undisclose ();
			}
		}

		const float kScorableDistance = 50.0f;

		public bool CanBeEvaluated ()
		{
			// TODO extend this to allow other means of evaluation

			return preferredDestination != null;
		}

		public float Score ()
		{
			// TODO extract this to allow different types of interactables to be scored individually
			float distance = preferredDestination.Distance (transform.position);
			float score = (kScorableDistance - distance) / kScorableDistance;
			return score;
		}
	}
}

