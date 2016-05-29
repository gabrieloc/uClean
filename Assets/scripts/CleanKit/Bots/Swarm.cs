using UnityEngine;
using System.Collections.Generic;

namespace CleanKit
{
	public class Swarm: Interactor
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

		private Interactable interacting;

		public void BeginUsingInteractable (Interactable interactable)
		{
			string name = "Swarm (" + bots.Count + ")";
			Debug.Log (name + " is using " + interactable.name);
			interacting = interactable;
		}

		public Vector3 PrimaryContactPoint ()
		{
			// TODO Assign a proper leader and use it's center
			return bots [0].transform.position;
		}

		public bool CanRelocateInteractable (Interactable interactable)
		{
			// TODO: see if each bot is in position
			return false;
		}

		public void RelocateInteractable (Interactable interactable, Vector3 position, float distanceDelta)
		{
			// TODO: move interactable
		}

		public void PrepareForInteractable (Interactable interactable)
		{
			// TODO: move each bot into position
		}

		public void RelocateToPosition (Vector3 position, float distanceDelta)
		{
			foreach (Bot bot in bots) {
				bot.transform.position = Vector3.MoveTowards (bot.transform.position, position, distanceDelta);
			}
		}
	}
}

