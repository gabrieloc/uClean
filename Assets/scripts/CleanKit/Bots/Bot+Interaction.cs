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
			return ignoreRelocationPoint == false && destination != null;
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

		public bool IsEmployed ()
		{
			return instruction != null;
		}

		public void Employ (Instruction instruction)
		{
			destination = instruction.destination;
			this.instruction = instruction;
			cell.SetInteraction (instruction.interactionType);
		}

		public void EndEmployment ()
		{
			cell.SetInteraction (InteractionType.None);
			actorDelegate.ActorFulfilledInstruction (this, instruction);
			instruction = null;
			destination = null;
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
			Interactable interactable = instruction.interactable;
			FixedJoint holdJoint = gameObject.GetComponent<FixedJoint> ();
			Rigidbody interactableRigidBody = interactable.GetComponent<Rigidbody> ();
			bool holdJointConnected = holdJoint.connectedBody == interactableRigidBody;
			if (interactableIsLiftable () && holdJointConnected) {
				return true;
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
			// TODO properly implement pushing interaction
			return true;

//			Vector3 botSize = GetComponent<Collider> ().bounds.size;
//			Vector3 interactableSize = instruction.interactable.GetComponent<Collider> ().bounds.size;
//			return botSize.magnitude / interactableSize.magnitude > kMaximumLiftableSizeMultiple;
		}

		bool withinInteractableDistance ()
		{
			Collider collider = instruction.interactable.GetComponent<Collider> ();
			Vector3 closestPoint = collider.ClosestPointOnBounds (transform.position);
			float distance = Vector3.Distance (closestPoint, transform.position);
			return distance < 0.5f; // TODO allow bot to get closer
		}

		void prepareForLiftingInteractable ()
		{
			Interactable interactable = instruction.interactable;
			Rigidbody interactableRigidBody = interactable.GetComponent<Rigidbody> ();
//			interactableRigidBody.isKinematic = true;

			NavMeshObstacle obstacle = interactable.gameObject.GetComponent<NavMeshObstacle> ();
			obstacle.enabled = false;

			if (isLookingAtInteractable () && withinInteractableDistance ()) {
				FixedJoint holdJoint = GetComponent<FixedJoint> ();
				if (holdJoint.connectedBody != null) {
					holdJoint.connectedBody = null;
				}
				holdJoint.connectedBody = interactableRigidBody;

				// TODO connect joint at bottom of object
//				Vector3 anchor = Vector3.down * interactable.GetComponent<Collider> ().bounds.size.y;
//				holdJoint.connectedAnchor = anchor;
			} else {
				moveTowardsInteractable ();
			}
		}

		void liftInteractableToRelocationPoint ()
		{
			// TODO move interactable to exact point, don't let it end up off-grid

			float distance = Vector3.Distance (transform.position, destination.transform.position);

			// TODO require certain amount of accuracy
			if (distance > 1.0f) {
				moveTowardsRelocationPoint ();
			} else {
				FixedJoint holdJoint = GetComponent<FixedJoint> ();
				holdJoint.connectedBody = null;
//				instruction.FulFill ();
				EndEmployment ();
			}
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
			Debug.DrawLine (transform.position, interactableContactPoint, looking ? Color.cyan : Color.magenta);
			return looking;
		}

		Vector3 interactableContactPoint;

		private bool rayCastAtInteractable (out Vector3 contactPoint, float distance = 2.0f)
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

			bool cast = interactableCollider.Raycast (ray, out hit, distance);

			contactPoint = ray.GetPoint (distance);
			return cast;
		}
	}
}

