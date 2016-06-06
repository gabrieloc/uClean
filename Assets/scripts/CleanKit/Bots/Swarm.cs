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

		readonly public SwarmCell cell;

		public Swarm ()
		{
			cell = SwarmCell.Instantiate ();
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

		public void IndicatorForInteractableSelected (Interactable interactable, InteractionType interactionType)
		{
			Debug.Log (name + " will " + interactionType.Description () + " " + interactable.name);
			this.interactable = interactable;
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
	}
}

