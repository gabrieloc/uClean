using UnityEngine;
using System.Collections.Generic;

namespace CleanKit
{
	public class Bot : MonoBehaviour, Interactor
	{
		public Vector3 relocationPoint = Vector3.zero;

		public bool selected { get; private set; }

		public void SetSelected (bool selected)
		{
			this.selected = selected;
			gameObject.SetSelected (selected);
			if (cell) {
				cell.gameObject.SetSelected (selected);
			}
		}

		public float kRelocatableRadius = 2.0f;
		public float kRelocationSpeed = 20.0f;

		public static Bot Instantiate ()
		{
			GameObject gameObject = Instantiate (Resources.Load ("Bot"), new Vector3 (0, 10, 0), new Quaternion ()) as GameObject;
			gameObject.SetSelected (false);
			gameObject.name = BotNamer.New ();

			Bot bot = gameObject.GetComponent<Bot> ();
			bot.createCell ();
			return bot;
		}

		void Update ()
		{
			bool relocationPointSet = relocationPoint.x != 0.0f && relocationPoint.z != 0.0f;
			bool withinRelocatableRadius = Vector3.Distance (relocationPoint, transform.position) > kRelocatableRadius;
			if (interactable != null) {// && canRelocateWithInteractable () == false) {
				prepareForInteractable ();
			} else if (relocationPointSet) {// && withinRelocatableRadius) {
				moveTowardsRelocationPoint ();
			}

			if (relocationPointSet) {
				Color color = selected ? Color.blue : Color.gray;
				Debug.DrawLine (relocationPoint - new Vector3 (2, 0, 0), relocationPoint + new Vector3 (2, 0, 0), color);
				Debug.DrawLine (relocationPoint - new Vector3 (0, 0, 2), relocationPoint + new Vector3 (0, 0, 2), color);
			}
		}

		private void moveTowardsRelocationPoint ()
		{
			if (relocationPoint.Equals (Vector3.zero)) {
				return;
			}

			float distanceDelta = kRelocationSpeed * Time.deltaTime;
			Vector3 position = relocationPoint;
			position.y = 0.5f;

			if (isLiftingInteractable ()) {
				// TODO
			} else {
				transform.position = Vector3.MoveTowards (transform.position, position, distanceDelta);
				if (Vector3.Distance (transform.position, position) > 1) {
					transform.LookAt (position);
				}
			}
		}

		// Cells
		public BotCell cell { get; private set; }

		private void createCell ()
		{
			cell = BotCell.Instantiate ();
			cell.SetBotName (gameObject.name);
		}

		private void destroyCell ()
		{
			GameObject.Destroy (cell.gameObject);
			cell = null;
		}

		// Swarms

		public Swarm swarm { get; private set; }

		public bool belongsToSwarm {
			get {
				return swarm != null;
			}
		}

		public void JoinSwarm (Swarm swarm)
		{
			destroyCell ();
			this.swarm = swarm;
		}

		public void LeaveSwarm ()
		{
			createCell ();
			this.swarm = null;
		}

		// Interactor

		private Interactable interactable = null;
		public InteractionType interaction;

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
			interaction = interactionType;
			Debug.Log (name + " is " + interaction.Description () + "ing " + interactable.name);
		}

		public void RelocateToPosition (Vector3 position)
		{
			relocationPoint = position;
		}

		private void prepareForInteractable ()
		{
			switch (interaction) {
			case InteractionType.Lift:
				prepareForLifting ();
				break;
			case InteractionType.Clean:
				break;
			}
		}

		private bool isLiftingInteractable ()
		{
			Vector3 hitPoint;
			bool looking = rayCastAtInteractable (transform.TransformDirection (Vector3.up), out hitPoint, 1.0f);
			Debug.DrawLine (transform.position, hitPoint, looking ? Color.green : Color.red);
			return looking;
		}

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

		public float kLiftStrength = 1.5f;
		private Vector3 interactableContactPoint;

		private void prepareForLifting ()
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
	}
}
