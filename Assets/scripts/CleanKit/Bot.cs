using UnityEngine;
using System.Collections.Generic;

namespace CleanKit
{
	public class Entity: MonoBehaviour
	{
	}

	public class BotGroup: Entity
	{
		public List<Bot> bots = new List<Bot> ();

		public BotGroup (List<Bot> initialBots)
		{
			bots = initialBots;
		}

		public int BotCount ()
		{
			return bots.Count;	
		}

		public void AddBot (Bot bot)
		{
			bots.Add (bot);
			bot.group = this;
		}

		public void RemoveBot (Bot bot)
		{
			bots.Remove (bot);
			bot.group = null;
		}

		public Bot BotAtIndex (int index)
		{
			return bots [index];
		}
	}

	public class Bot: Entity
	{
		public static Bot Instantiate ()
		{
			GameObject gameObject = Instantiate (Resources.Load ("Bot"), new Vector3 (0, 10, 0), new Quaternion ()) as GameObject;
			gameObject.SetSelected (false);
			gameObject.name = BotNamer.New ();
			Bot bot = gameObject.GetComponent<Bot> ();
			return bot;
		}

		public BotGroup group;

		public bool belongsToGroup {
			get {
				return group != null;
			}
		}
	}

	public class Interactable : MonoBehaviour
	{
	}
}
	