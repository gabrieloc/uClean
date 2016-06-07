using UnityEngine;
using System.Collections.Generic;

namespace CleanKit
{
	public class Swarm: Actor
	{
		public List<Bot> bots {
			get {
				List<Bot> bots = new List<Bot> ();
				foreach (Bot bot in GameObject.FindObjectsOfType<Bot>()) {
					if (bot.swarm == this) {
						bots.Add (bot);
					}
				}
				return bots;
			}
		}

		public string name {
			get {
				return "Swarm (" + bots.Count + ")";
			}
		}

		readonly public ActorCell cell;

		public Swarm ()
		{
			cell = ActorCell.Instantiate ();
		}

		public int BotCount ()
		{
			return bots.Count;	
		}

		public void AddBot (Bot bot)
		{
			cell.IncrementCount ();
			bot.JoinSwarm (this);
		}

		public void RemoveBot (Bot bot)
		{
			cell.DecrementCount ();
			bot.LeaveSwarm ();
		}

		public Bot BotAtIndex (int index)
		{
			return bots [index];
		}

		// Interactor

		private Interactable interactable;
		private InteractionType interaction;

		public void IndicatorForInteractableSelected (Interactable interactable, InteractionType interactionType)
		{
			if (this.interactable != null && this.interactable.Equals (interactable) && interaction == interactionType) {
				return;
			}

			this.interaction = interaction;
			SetInteraction (interactionType);

			Debug.Log (name + " will " + interaction.Description () + " " + interactable.name);

			interactable.BecomeUnavailable ();
			RelocateToPosition (Vector3.zero);
		}

		public Vector3 PrimaryContactPoint ()
		{
			// TODO Assign a proper leader and use it's center
			return bots [0].transform.position;
		}

		public void RelocateToPosition (Vector3 position)
		{
			foreach (Bot bot in bots) {
				// TODO follow a leader instead
				bot.RelocateToPosition (position);
			}
		}

		public void SetInteraction (InteractionType interaction)
		{
			this.interaction = interaction;
			cell.SetInteraction (interaction);
		}
	}
}
	