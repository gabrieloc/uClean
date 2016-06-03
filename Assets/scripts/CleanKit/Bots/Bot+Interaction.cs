using UnityEngine;

namespace CleanKit
{
	public partial class Bot
	{
		private Interactable interactable = null;

		private InteractionType interaction;

		public void SetInteraction (InteractionType interaction)
		{
			this.interaction = interaction;
			cell.SetInteractionName (interaction.Description ());

		}

		public float kLiftStrength = 1.5f;
		private float kMaximumLiftableSize = 10.0f;
		private Vector3 interactableContactPoint;

		private bool canPerformInteraction ()
		{
			switch (interaction) {
			case InteractionType.Move:
				return canMoveInteractable ();
			case InteractionType.Clean:
				return canCleanInteractable ();
			}
			return false;
		}

		private void performInteraction ()
		{
			switch (interaction) {
			case InteractionType.Move:
				moveInteractable ();
				break;
			case InteractionType.Clean:
				break;
			}
		}

		private void prepareForInteraction ()
		{
			switch (interaction) {
			case InteractionType.Move:
				prepareForMovingInteractable ();
				break;
			case InteractionType.Clean:
				prepareForCleaningInteractable ();
				break;
			}
		}

		// Moving

		private bool canMoveInteractable ()
		{
			if (interactableIsLiftable ()) {
				Vector3 hitPoint;
				bool isBelowObject = rayCastAtInteractable (transform.TransformDirection (Vector3.up), out hitPoint, 1.0f);
				Debug.DrawLine (transform.position, hitPoint, isBelowObject ? Color.green : Color.red);
				return isBelowObject;
			} else {
				bool canPushInteractable = isLookingAtInteractable ();
				return canPushInteractable;
			}
		}

		private void prepareForMovingInteractable ()
		{
			if (interactableIsLiftable ()) {
				prepareForLiftingInteractable ();
			} else {
				prepareForPushingInteractable ();
			}
		}

		private void moveInteractable ()
		{
			if (interactableIsLiftable ()) {
//				liftInteractableToRelocationPoint ();
			} else {
				pushInteractableToRelocationPoint ();
			}
		}

		// Lifting

		private bool interactableIsLiftable ()
		{
			Bounds bounds = interactable.GetComponent<Collider> ().bounds;
			Vector3 size = bounds.size;
			return size.magnitude < kMaximumLiftableSize;
		}

		private void prepareForLiftingInteractable ()
		{
			if (isLookingAtInteractable () && interactableContactPoint != Vector3.zero) {
				// Lift object
				ForceMode forceMode = ForceMode.Impulse;
				Rigidbody rigidBody = interactable.GetComponent<Rigidbody> ();
				float force = kLiftStrength * rigidBody.mass;
				rigidBody.AddForceAtPosition (new Vector3 (0.0f, force, 0.0f), interactableContactPoint, forceMode);

				Vector3 c = interactableContactPoint;
				Debug.DrawLine (new Vector3 (c.x, c.y, c.z), new Vector3 (c.x, c.y + 4, c.z), Color.red);
				Debug.DrawLine (new Vector3 (c.x, c.y + 4, c.z), new Vector3 (c.x - 1, c.y + 3, c.z), Color.red);
				Debug.DrawLine (new Vector3 (c.x, c.y + 4, c.z), new Vector3 (c.x + 1, c.y + 3, c.z), Color.red);

				interactableContactPoint = Vector3.zero;
			} else {
				// Attempt to go under
				Vector3 newPosition = interactable.transform.position;
				newPosition.y = 0.5f;
				RelocateToPosition (newPosition);
				moveTowardsRelocationPoint ();
			}
		}

		private void liftInteractableToRelocationPoint ()
		{
			Vector3 interactablePosition = relocationPoint;
			interactablePosition.y = 5.5f; // TODO offset properly
			float distanceDelta = kRelocationSpeed * Time.deltaTime;
			interactable.transform.position = Vector3.MoveTowards (transform.position, interactablePosition, distanceDelta);
			moveTowardsRelocationPoint ();
		}

		// Pushing

		private void prepareForPushingInteractable ()
		{
			moveTowardsRelocationPoint ();
		}

		private void pushInteractableToRelocationPoint ()
		{
			// TODO
		}


		// Cleaning

		private bool canCleanInteractable ()
		{
			// TODO
			return false;
		}

		private void prepareForCleaningInteractable ()
		{
			// TODO
		}

		// Conveniences

		private bool isLookingAtInteractable ()
		{
			Vector3 contactPoint;
			bool looking = rayCastAtInteractable (transform.TransformDirection (Vector3.forward), out contactPoint, 1.0f);
			Debug.DrawLine (transform.position, contactPoint, looking ? Color.green : Color.red);
			interactableContactPoint = contactPoint;
			return looking;
		}

		private bool rayCastAtInteractable (Vector3 direction, out Vector3 contactPoint, float distance)
		{
			contactPoint = new Vector3 ();

			if (interactable == null) {
				return false;
			}

			Ray ray = new Ray ();
			ray.direction = direction;
			ray.origin = transform.position;

			RaycastHit hit;
			Collider interactableCollider = interactable.gameObject.GetComponent<Collider> ();

			bool cast = interactableCollider.Raycast (ray, out hit, distance);

			contactPoint = ray.GetPoint (distance);
			return cast;
		}
	}
}

