using UnityEngine;
using System.Collections.Generic;

namespace CleanKit
{

	public class Bot : MonoBehaviour, Interactor
	{

		public static Bot Instantiate ()
		{
			GameObject gameObject = Instantiate (Resources.Load ("Bot"), new Vector3 (0, 10, 0), new Quaternion ()) as GameObject;
			gameObject.SetSelected (false);
			gameObject.name = BotNamer.New ();

			Bot bot = gameObject.GetComponent<Bot> ();
			bot.createCell ();
			return bot;
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

		public float relocationRadus = 5.0f;

		private bool canRelocateToPosition (Vector3 position)
		{
			bool positionValid = position.x != 0.0f && position.z != 0.0f;
			bool withinRelocatableRadius = Vector3.Distance (position, transform.position) > relocationRadus;
			return positionValid && withinRelocatableRadius;
		}

		// Interactor

		public void BeginUsingInteractable (Interactable interactable)
		{
			Debug.Log (name + " is interacting");
		}

		public Vector3 PrimaryContactPoint ()
		{
			return transform.position;
		}

		public bool CanRelocateInteractable (Interactable interactable)
		{
			// TODO: see if bot is in position
			return false;
		}

		public void RelocateInteractable (Interactable interactable, Vector3 position, float distanceDelta)
		{
			// TODO: move interactable
		}

		public void PrepareForInteractable (Interactable interactable)
		{
			// TODO: move into position
		}

		public void RelocateToPosition (Vector3 position, float distanceDelta)
		{
			if (canRelocateToPosition (position)) {
				transform.position = Vector3.MoveTowards (transform.position, position, distanceDelta);
			}
		}
	}
}
	