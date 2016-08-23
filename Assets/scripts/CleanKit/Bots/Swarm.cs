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

		public bool IsEmployed ()
		{
			return bots [0].IsEmployed ();
		}

		public void SetSelected (bool selected)
		{
			// TODO
		}

		public void Employ (Instruction instruction)
		{
			foreach (Bot bot in bots) {
				bot.Employ (instruction);
			}
			cell.SetInteraction (instruction.interactionType);
		}

		public string Name ()
		{
			return "Swarm (" + bots.Count + ")";
		}

		public bool IsDivisible ()
		{
			return bots.Count > 1;
		}

		public Actor Bisect ()
		{
			Swarm newSwarm = new Swarm ();
			int halfIndex = bots.Count / 2;
			for (int i = 0; i < halfIndex; i++) {
				Bot bot = BotAtIndex (i);
				RemoveBot (bot);
				newSwarm.AddBot (bot);
			}
			return newSwarm;
		}

		// Interactables

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
	}
}
	