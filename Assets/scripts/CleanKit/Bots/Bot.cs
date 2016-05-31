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
			position.y += 0.5f;

			if (canRelocateWithInteractable ()) {
				print ("Can relocate " + interactable);
			} else {
				transform.position = Vector3.MoveTowards (transform.position, position, distanceDelta);
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

		private bool canRelocateWithInteractable ()
		{
			if (interactable != null) {
				Vector3 direction = transform.TransformDirection (Vector3.up);
				RaycastHit hit;
				if (Physics.Raycast (transform.position, direction, out hit, 10.0f)) {
					GameObject hitObject = hit.transform.gameObject;
					return hitObject.Equals (interactable);
				}
			}
			return false;
		}

		public float kLiftStrength = 50.0f;
		public float kLiftAttemptInterval = 20.0f;
		private float lastLiftedInterval = 0.0f;
		private Vector3 interactableContactPoint;

		private void prepareForLifting ()
		{
			if (lastLiftedInterval < 0.0f && interactableContactPoint != Vector3.zero) {

				ForceMode forceMode = ForceMode.Impulse;
				Rigidbody rigidBody = interactable.GetComponent<Rigidbody> ();
				rigidBody.AddForceAtPosition (new Vector3 (0.0f, kLiftStrength, 0.0f), interactableContactPoint, forceMode);

				lastLiftedInterval = kLiftAttemptInterval;
				interactableContactPoint = Vector3.zero;
			} else {
				// Attempt to go under
				Vector3 newPosition = interactable.transform.position;
				newPosition.y = 0.5f;
				RelocateToPosition (newPosition);
				moveTowardsRelocationPoint ();
			} 

			Vector3 c = interactableContactPoint;
			Debug.DrawLine (new Vector3 (c.x, c.y, c.z), new Vector3 (c.x, c.y + 4, c.z), Color.red);
			Debug.DrawLine (new Vector3 (c.x, c.y + 4, c.z), new Vector3 (c.x - 1, c.y + 3, c.z), Color.red);
			Debug.DrawLine (new Vector3 (c.x, c.y + 4, c.z), new Vector3 (c.x + 1, c.y + 3, c.z), Color.red);

			lastLiftedInterval--;
		}

		void OnCollisionEnter (Collision other)
		{
			if (interactable != null && other.gameObject.Equals (interactable.gameObject)) {
				interactableContactPoint = other.contacts [0].point;
				print (interactableContactPoint);
			}
		}
	}
}
