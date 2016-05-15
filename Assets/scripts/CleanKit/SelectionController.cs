using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/*
 * Responsible for managing 2D UI (HUD)
 * Informs BotController of selection/deselection
 * Takes no responsibility for behaviour
*/

namespace CleanKit
{
	public class SelectionController: MonoBehaviour
	{
		private List<Bot> allBots = new List<Bot> ();
		private List<BotGroup> botGroups = new List<BotGroup> ();
		public List<Bot> selectedBots = new List<Bot> ();

		// TODO use proper delegation syntax
		public BotController selectionDelegate;

		public BotSelectionLayout layout;

		public int NumberOfGroups ()
		{
			return botGroups.Count;
		}

		public int NumberOfBotsInGroup (int groupIndex)
		{
			BotGroup group = botGroups [groupIndex];
			return group.BotCount ();
		}

		public int NumberOfUngroupedBots ()
		{
			int count = 0;
			foreach (Bot bot in allBots) {
				count += bot.group != null ? 1 : 0;
			}
			return count;
		}


		void Awake ()
		{
			layout = GetComponent<BotSelectionLayout> ();
			layout.layoutDelegate = this;
		}

		public void DidInsertBot (Bot bot)
		{
			allBots.Add (bot);

			GameObject cell = Instantiate (Resources.Load ("BotCell"), new Vector3 (), new Quaternion ()) as GameObject;
			cell.SetSelected (false);
			cell.transform.SetParent (transform, false);
			cell.GetComponent<Button> ().onClick.AddListener (() => didSelectBot (bot));

			layout.UpdateLayout ();
		}

		private void didSelectBot (Bot bot)
		{
			bool wasSelected = IsBotSelected (bot);
			if (wasSelected) {
				removeBotFromSelection (bot);
			} else {
				addBotToSelection (bot);
			}
			bot.gameObject.SetSelected (!wasSelected);

			BotCell cell = cellForBot (bot);
			cell.gameObject.SetSelected (!wasSelected);

			layout.UpdateLayout ();
		}

		private void didSelectBotGroup (BotGroup group)
		{
			foreach (Bot bot in group.bots) {
				didSelectBot (bot);
			}
		}

		/* 
		 * 	selectedBots will contain either a single ungrouped bot or a list of bots belonging to the same group
		 * 	A group should not get created until more than one bot has been selected.
		 */

		private void addBotToSelection (Bot bot)
		{
			selectedBots.Add (bot);

			if (selectedBots.Count > 1) {
				BotGroup group = bot.group;
				if (group != null) {
					group.RemoveBot (bot);
					if (group.BotCount () == 0) {
						botGroups.Remove (group);
					}
				} else {
					group = new BotGroup (selectedBots);
					botGroups.Add (group);
				}
			}

			selectionDelegate.selectionControllerSelectedBot (bot);
		}

		private void removeBotFromSelection (Bot bot)
		{
			selectedBots.Remove (bot);
			selectionDelegate.selectionControllerDeselectedBot (bot);
		}

		private BotCell cellForBot (Bot bot)
		{
			int index = allBots.IndexOf (bot);
			BotCell cell = BotCell.AllObjects () [index];
			return cell;
		}

		// Public Conveniences

		public bool IsBotSelected (Bot bot)
		{
			return selectedBots.Contains (bot);
		}
	}
}