using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace CleanKit
{
	public class SwarmCell: MonoBehaviour
	{
		public static SwarmCell Instantiate ()
		{
			GameObject gameObject = Instantiate (Resources.Load ("SwarmCell"), new Vector3 (), new Quaternion ()) as GameObject;
			gameObject.SetSelected (false);

			SwarmCell cell = gameObject.GetComponent<SwarmCell> ();
			cell.setCount (0); 
			return cell;
		}

		int count = 0;

		private void setCount (int count)
		{
			Text label = gameObject.GetComponentInChildren<Text> ();
			label.text = count.ToString ();
			name = "Swarm (" + count + ")";
		}

		internal void IncrementCount ()
		{
			setCount (++count);
		}

		internal void DecrementCount ()
		{
			setCount (--count);
		}

		public static List<SwarmCell> AllObjects ()
		{
			List<SwarmCell> objects = new List<SwarmCell> ();
			foreach (SwarmCell cell in GameObject.FindObjectsOfType<SwarmCell> ()) {
				objects.Add (cell.GetComponent<SwarmCell> ());
			}
			objects.Reverse ();
			return objects;
		}
	}

	public class Swarm
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
	}
}

