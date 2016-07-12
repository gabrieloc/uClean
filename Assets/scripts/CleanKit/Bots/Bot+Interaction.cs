using UnityEngine;

namespace CleanKit
{
	public partial class Bot
	{
		private Interactable interactable = null;

		private InteractionType interaction;

		private void moveTowardsInteractable ()
		{
			moveTowardsInteractable (interactable.transform.position);
		}

		private void moveTowardsInteractable (Vector3 desired)
		{
			int layerMask = 1 << LayerMask.NameToLayer ("Interactable");
			RaycastHit hit;
			Vector3 d = transform.TransformDirection (Vector3.forward);
			if (Physics.SphereCast (transform.position, kMinimumInteractableDistance, d, out hit, kPersonalSpaceRadius, layerMask)) {
				desired = hit.point;
			}
			// TODO make this work
			desired = CalculatePersonalSpace (transform.position, desired);
			desired.y = transform.position.y;
			lookInDirection (desired);

			bool canInteract = canPerformInteraction ();
			ignoreRelocationPoint = !canInteract;

			if (!canInteract) {
				float distanceDelta = kRelocationSpeed * Time.deltaTime;
				transform.position = Vector3.MoveTowards (transform.position, desired, distanceDelta);
			}

			Debug.DrawLine (transform.position, desired, Color.blue);
		}

		// Actor

		public Vector3 PrimaryContactPoint ()
		{
			return transform.position;
		}

		public void IndicatorForInteractableSelected (Interactable interactable, InteractionType interactionType)
		{
			if (this.interactable != null && this.interactable.Equals (interactable) && interaction == interactionType) {
				return;
			}

			this.interactable = interactable;
			SetInteraction (interactionType);
			Debug.Log (name + " will " + interaction.Description () + " " + interactable.name);

			interactable.BecomeUnavailable ();

			CancelRelocation ();
		}

		bool shouldRelocate ()
		{
			return ignoreRelocationPoint == false && (destination != null && destination.ShouldRelocate (transform));
		}

		public void RelocateToDestination (Destination newDestination)
		{
			if (destination != null) {
				CancelRelocation ();
			}
			destination = newDestination;
		}

		public void CancelRelocation ()
		{
			if (destination != null) {
				Destroy (destination.gameObject);
				destination = null;
			}
		}

		public void SetInteraction (InteractionType interaction)
		{
			this.interaction = interaction;
			cell.SetInteraction (interaction);
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
//				Debug.DrawLine (transform.position, hitPoint, isBelowObject ? Color.green : Color.red);
				return isBelowObject;
			} else if (destination != null) {
				float d = Vector3.Distance (transform.position, pushPosition ());
				return d < kMinimumInteractableDistance;
			}
			return false;
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
			ignoreRelocationPoint = false;

			if (interactableIsLiftable ()) {
				liftInteractableToRelocationPoint ();
			} else {
				pushInteractableToRelocationPoint ();
			}
		}

		// Lifting

		bool interactableIsLiftable ()
		{
			Bounds bounds = interactable.GetComponent<Collider> ().bounds;
			Vector3 size = bounds.size;
			return size.magnitude < kMaximumLiftableSize;
		}

		void prepareForLiftingInteractable ()
		{
			FixedJoint holdJoint = gameObject.GetComponent<FixedJoint> ();
			if (holdJoint) {
				Destroy (holdJoint);
			}
			if (isLookingAtInteractable () && interactableContactPoint != Vector3.zero) {
				// Lift object

				// TODO: 	Don't rely on physics engine for lifting, considering applying
				//			additive transform to interactable instead.
				//			Transform should be a quaternion transformation which lerps 
				//			between current quaternion and a desired quaternion

//				ForceMode forceMode = ForceMode.Impulse;
//				Rigidbody rigidBody = interactable.GetComponent<Rigidbody> ();
//				float force = kLiftStrength * rigidBody.mass;
//				rigidBody.AddForceAtPosition (new Vector3 (0.0f, force, 0.0f), interactableContactPoint, forceMode);

				Vector3 c = interactableContactPoint;


				float step = kLiftStrength * Time.deltaTime;
				Quaternion r0 = interactable.transform.rotation;
				Quaternion r1 = Quaternion.AngleAxis (30.0f, Vector3.back);
				Quaternion r = Quaternion.RotateTowards (r0, r1, step);

//				interactable.transform.rotation = Quaternion.Lerp (r0, r, 0.5f);
				interactable.transform.rotation = r;


				Debug.DrawRay (c, Vector3.up, Color.yellow);

//				Debug.DrawLine (new Vector3 (c.x, c.y, c.z), new Vector3 (c.x, c.y + 4, c.z), Color.red);
//				Debug.DrawLine (new Vector3 (c.x, c.y + 4, c.z), new Vector3 (c.x - 1, c.y + 3, c.z), Color.red);
//				Debug.DrawLine (new Vector3 (c.x, c.y + 4, c.z), new Vector3 (c.x + 1, c.y + 3, c.z), Color.red);

//				interactableContactPoint = Vector3.zero;
			} else {
				// Attempt to go under
				moveTowardsInteractable ();
			}
		}

		void liftInteractableToRelocationPoint ()
		{
			FixedJoint holdJoint = gameObject.GetComponent<FixedJoint> ();
			if (holdJoint == null) {
				holdJoint = gameObject.AddComponent<FixedJoint> ();
				holdJoint.connectedBody = interactable.GetComponent<Rigidbody> ();
			}

			float distanceDelta = kRelocationSpeed * 0.5f * Time.deltaTime;
			moveTowardsRelocationPoint (distanceDelta);
		}

		// Pushing

		void prepareForPushingInteractable ()
		{
			// move to push point

			if (destination != null) {
				Vector3 p = pushPosition ();
				moveTowardsInteractable (p);
				Debug.DrawLine (transform.position, p, Color.yellow);
			} else {
				moveTowardsInteractable ();
			}
		}

		public float PushSpeed = 5.0f;

		void pushInteractableToRelocationPoint ()
		{
			Vector3 p = pushPosition ();
			Vector3 f = Vector3.Lerp (p, destination.transform.position, 0.5f);
			float s = PushSpeed * Time.deltaTime;
			Vector3 t = Vector3.MoveTowards (transform.position, f, s);
				
			transform.position = t;

			Debug.DrawLine (transform.position, p, Color.cyan);
		}

		Vector3 pushPosition ()
		{
			Vector3 d = destination.transform.position;
			Vector3 d2 = interactable.transform.InverseTransformPoint (d);
			d2 *= -1.0f;
			d2 = interactable.transform.TransformPoint (d2);

			Collider col = interactable.GetComponent<Collider> ();
			Vector3 c = col.ClosestPointOnBounds (d2);

			c.y = 0.0f;

			return c;
		}

		// Cleaning

		bool canCleanInteractable ()
		{
			// TODO
			return false;
		}

		void prepareForCleaningInteractable ()
		{
			// TODO
			moveTowardsInteractable ();
		}

		// Conveniences

		bool isLookingAtInteractable ()
		{
			Vector3 contactPoint;
			Vector3 p = transform.TransformDirection (Vector3.forward);
			p.y += 0.01f;
			bool looking = rayCastAtInteractable (p, out contactPoint, 1.0f);
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

