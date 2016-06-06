using UnityEngine;
using System.Collections.Generic;

namespace CleanKit
{
	public partial class Bot : MonoBehaviour, Actor
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
			bot.SetInteraction (InteractionType.None);
			return bot;
		}

		void Update ()
		{
			bool relocationPointSet = relocationPoint.x != 0.0f && relocationPoint.z != 0.0f;

			if (interactable != null) {
				if (canPerformInteraction ()) {
					performInteraction ();
				} else {
					prepareForInteraction ();
				}
			} else if (relocationPointSet) {
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
			moveTowardsRelocationPoint (kRelocationSpeed * Time.deltaTime);
		}

		private void moveTowardsRelocationPoint (float distanceDelta)
		{
			if (relocationPoint.Equals (Vector3.zero) || ignoreRelocationPoint) {
				return;
			}

			Vector3 position = relocationPoint;
			position.y = 0.5f;

			transform.position = Vector3.MoveTowards (transform.position, position, distanceDelta);
			if (Vector3.Distance (transform.position, position) > 1.0f) {
				transform.LookAt (new Vector3 (position.x, transform.position.y, position.y));
				Debug.DrawLine (transform.position, position, Color.blue);
			}
		}

		private bool ignoreRelocationPoint;
		private float kMinimumInteractableDistance = 1.0f;

		private void moveTowardsInteractable ()
		{
			RaycastHit hit;
			Vector3 origin = interactable.transform.position;
			int layerMask = 1 << LayerMask.NameToLayer ("Surface");
			Physics.Raycast (origin, Vector3.down, out hit, 100, layerMask);

			Vector3 position = hit.point;
			Debug.DrawLine (origin, hit.point, Color.green);

			float magnitude = Vector3.Distance (transform.position, position);
			bool canInteract = magnitude < kMinimumInteractableDistance;
			ignoreRelocationPoint = !canInteract;

			if (canInteract) {
				return;
			}

			float distanceDelta = kRelocationSpeed * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, position, distanceDelta);
			transform.LookAt (new Vector3 (position.x, transform.position.y, position.y));
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
			RelocateToPosition (Vector3.zero);
		}

		public void RelocateToPosition (Vector3 position)
		{
			relocationPoint = position;
		}
	}
}
