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
			if (relocationPointSet && withinRelocatableRadius) {
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
			if (interactable != null && canRelocateWithInteractable () == false) {
				prepareForInteractable ();
			}

			float distanceDelta = kRelocationSpeed * Time.deltaTime;
			Vector3 position = relocationPoint;
			position.y += 0.5f;
			transform.position = Vector3.MoveTowards (transform.position, position, distanceDelta);
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
			Rigidbody rigidBody = interactable.GetComponent<Rigidbody> ();
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
			// TODO determine if bot is in position and attached to liftable object
			return false;
		}

		private void prepareForLifting ()
		{
			// TODO move bot into position and attach to liftable object somehow
			Debug.Log ("Preparing for lift interaction");
		}
	}
}
	