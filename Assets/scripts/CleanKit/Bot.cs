using UnityEngine;
using System.Collections.Generic;

namespace CleanKit
{

	public class Bot : MonoBehaviour
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
	}
}
	