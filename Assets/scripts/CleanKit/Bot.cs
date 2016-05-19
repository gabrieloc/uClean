using UnityEngine;
using System.Collections.Generic;

namespace CleanKit
{
	public class BotGroup
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

	public class Bot : MonoBehaviour
	{
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
	