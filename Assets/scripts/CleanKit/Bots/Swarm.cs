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

		// Actor Conformance

		public void SetSelected (bool selected)
		{
			// TODO
		}

		public string Name ()
		{
			return "Swarm (" + bots.Count + ")";
		}

		// Interactables

		private Interactable interactable;
		private InteractionType interaction;

		private Vector3 relocationPoint;

		public void IndicatorForInteractableSelected (Interactable interactable, InteractionType interactionType)
		{
			if (this.interactable != null && this.interactable.Equals (interactable) && interaction == interactionType) {
				return;
			}

			this.interactable = interactable;
				
			SetInteraction (interactionType);

			Debug.Log (Name () + " will " + interaction.Description () + " " + interactable.name);

			interactable.BecomeUnavailable ();
			cancelRelocation ();
		}

		public Vector3 PrimaryContactPoint ()
		{
			// TODO Assign a proper leader and use it's center
			return bots [0].transform.position;
		}

		public void RelocateToDestination (Destination newDestination)
		{
			foreach (Bot bot in bots) {
				// TODO follow a leader instead
				bot.RelocateToDestination (newDestination);
			}
		}

		public float DistanceFromDestination ()
		{			
			// TODO use a weighted average or consider making this specific to bots
			return bots [0].DistanceFromDestination ();
		}

		private void cancelRelocation ()
		{
			foreach (Bot bot in bots) {
				bot.CancelRelocation ();
			}
		}

		public void SetInteraction (InteractionType interaction)
		{
			this.interaction = interaction;
			cell.SetInteraction (interaction);
		}
	}
}
	