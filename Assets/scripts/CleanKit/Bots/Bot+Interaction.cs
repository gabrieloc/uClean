using UnityEngine;
using System;

namespace CleanKit
{
	public partial class Bot
	{
		private Instruction instruction;

		private void moveTowardsInteractable ()
		{
			bool canInteract = canPerformInteraction ();
			ignoreRelocationPoint = !canInteract;

			if (!canInteract) {
				Collider collider = instruction.interactable.GetComponent<Collider> ();
				Vector3 closestPoint = collider.ClosestPointOnBounds (transform.position);
				moveTowardsPoint (closestPoint);
			}
		}

		// Actor

		public Vector3 PrimaryContactPoint ()
		{
			return transform.position;
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

		public void SetInstruction (Instruction instruction)
		{
			this.instruction = instruction;
			cell.SetInteraction (instruction.interactionType);
		}

		public bool IsEmployed ()
		{
			return instruction != null;
		}

		public void Employ (Instruction instruction)
		{
			SetInstruction (instruction);
		}

		public bool IsDivisible ()
		{
			return false;
		}

		public Actor Bisect ()
		{
			return null;
		}

		// Interactions

		public float kLiftStrength = 1.5f;

		// Bot cannot lift object more than half it's size
		private float kMaximumLiftableSizeMultiple = 0.5f;

		private bool canPerformInteraction ()
		{
			switch (instruction.interactionType) {
			case InteractionType.Move:
				return canMoveInteractable ();
			case InteractionType.Clean:
				return canCleanInteractable ();
			}
			return false;
		}

		private void performInteraction ()
		{
			switch (instruction.interactionType) {
			case InteractionType.Move:
				moveInteractable ();
				break;
			case InteractionType.Clean:
				break;
			}
		}

		private void prepareForInteraction ()
		{
			switch (instruction.interactionType) {
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
			Vector3 contactPoint;
			if (interactableIsLiftable () && rayCastAtInteractable (out contactPoint)) {
				Vector3 lContact = transform.InverseTransformPoint (contactPoint);
				float angle = Mathf.Abs (Vector3.Angle (transform.position, lContact));
				bool belowInteractable = angle > 45.0f;
				return belowInteractable;
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
			if (destination == null) {
				return;
			}

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
			return true;

//			Vector3 botSize = GetComponent<Collider> ().bounds.size;
//			Vector3 interactableSize = instruction.interactable.GetComponent<Collider> ().bounds.size;
//			return botSize.magnitude / interactableSize.magnitude > kMaximumLiftableSizeMultiple;
		}

		void prepareForLiftingInteractable ()
		{
			Interactable interactable = instruction.interactable;
			Vector3 anchor = Vector3.down * interactable.GetComponent<Collider> ().bounds.size.y;

			if (isLookingAtInteractable ()) {
				Rigidbody interactableRigidBody = interactable.GetComponent<Rigidbody> ();
				FixedJoint holdJoint = GetComponent<FixedJoint> ();
				holdJoint.connectedBody = interactableRigidBody;
				holdJoint.connectedAnchor = anchor;
				
				interactableRigidBody.isKinematic = true;
			} else {
				moveTowardsInteractable ();
			}

//			bool attemptLift = isLookingAtInteractable ();
//			NavMeshObstacle obstacle = interactable.GetComponent<NavMeshObstacle> ();
//			obstacle.enabled = !attemptLift;
//			Rigidbody rigidBody = interactable.GetComponent<Rigidbody> ();
//			rigidBody.isKinematic = attemptLift;
//
//			if (attemptLift) {
//				Vector3 c = interactableContactPoint;
//
//				Quaternion r0 = interactable.transform.rotation;
//				Vector3 axis = transform.TransformDirection (Vector3.right);
//				Quaternion r1 = Quaternion.AngleAxis (50.0f, axis);
//				Quaternion r = Quaternion.Lerp (r0, r1, 0.5f * Time.deltaTime);
//
//				interactable.transform.rotation = r;
//
//				Debug.DrawLine (new Vector3 (c.x, c.y, c.z), new Vector3 (c.x, c.y + 4, c.z), Color.red);
//				Debug.DrawLine (new Vector3 (c.x, c.y + 4, c.z), new Vector3 (c.x - 1, c.y + 3, c.z), Color.red);
//				Debug.DrawLine (new Vector3 (c.x, c.y + 4, c.z), new Vector3 (c.x + 1, c.y + 3, c.z), Color.red);
//			} else {
//				Vector3 closestPoint = interactable.GetComponent<Collider> ().ClosestPointOnBounds (transform.position);
//				Debug.DrawLine (transform.position, closestPoint, Color.yellow);
//				if (Vector3.Distance (transform.position, closestPoint) > 1.0f) {
//					moveTowardsInteractable ();
//				}
//			}
		}

		void liftInteractableToRelocationPoint ()
		{
//			FixedJoint holdJoint = gameObject.GetComponent<FixedJoint> ();
//			if (holdJoint == null) {
//				holdJoint = gameObject.AddComponent<FixedJoint> ();
//				holdJoint.connectedBody = instruction.interactable.GetComponent<Rigidbody> ();
//			}

			moveTowardsRelocationPoint ();
		}

		// Pushing

		void prepareForPushingInteractable ()
		{
			// move to push point
			bool destinationExists = destination != null;
			NavMeshObstacle obstacle = instruction.interactable.GetComponent<NavMeshObstacle> ();
			obstacle.enabled = destinationExists;

			if (destinationExists) {
				Vector3 p = pushPosition ();
				moveTowardsPoint (p);
				Debug.DrawLine (transform.position, p, Color.yellow);
			} else {
				moveTowardsInteractable ();
			}
		}

		public float PushSpeed = 5.0f;

		void pushInteractableToRelocationPoint ()
		{
			// TODO reduce speed
			moveTowardsRelocationPoint ();
			Debug.DrawLine (transform.position, destination.transform.position, Color.cyan);
		}

		Vector3 pushPosition ()
		{
			Interactable interactable = instruction.interactable;
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
			bool looking = rayCastAtInteractable (out contactPoint);
			interactableContactPoint = contactPoint;
			Debug.DrawLine (transform.position, interactableContactPoint, looking ? Color.green : Color.red);
			return looking;
		}

		Vector3 interactableContactPoint;

		private bool rayCastAtInteractable (out Vector3 contactPoint)
		{
			contactPoint = new Vector3 ();
			
			if (instruction == null) {
				return false;
			}

			Vector3 lPos = transform.InverseTransformPoint (instruction.interactable.transform.position);
			lPos = lPos.normalized;
			Vector3 direction = lPos;

			Ray ray = new Ray (transform.position, transform.TransformDirection (direction));
			RaycastHit hit;
			Collider interactableCollider = instruction.interactable.gameObject.GetComponent<Collider> ();
			float distance = 2.0f;

			bool cast = interactableCollider.Raycast (ray, out hit, distance);

			contactPoint = ray.GetPoint (distance);
			return cast;
		}
	}
}

