using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
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
		//		internal List<Bot> allBots = new List<Bot> ();
		//		internal List<BotGroup> botGroups = new List<BotGroup> ();
		public List<Entity> availableEntities = new List<Entity> ();
		public List<Bot> selectedBots = new List<Bot> ();

		internal bool isGroupingEnabled;

		// TODO use proper delegation syntax
		public BotController selectionDelegate;

		private Button groupButton;

		void Awake ()
		{
			groupButton = GameObject.Find ("GroupButton").GetComponent<Button> ();
			groupButton.GetComponent<Button> ().onClick.AddListener (() => toggleGrouping ());
		}

		private void toggleGrouping ()
		{
			isGroupingEnabled = !isGroupingEnabled;

			groupButton.GetComponent<Image> ().color = isGroupingEnabled ? Color.red : Color.black;
		}

		public void DidInsertBot (Bot bot)
		{
			availableEntities.Add (bot);

			BotCell cell = BotCell.Instantiate (bot);
			cell.transform.SetParent (transform, false);
			cell.GetComponent<Button> ().onClick.AddListener (() => didSelectBot (bot));
		}

		private void didSelectBot (Bot bot)
		{
			bool wasSelected = IsBotSelected (bot);
			if (wasSelected) {
				removeBotFromSelection (bot);
			} else {
				addBotToSelection (bot);
			}
			BotCell cell = cellForEntity (bot);
			cell.gameObject.SetSelected (!wasSelected);
			cell.GetComponent<Button> ().onClick.AddListener (() => didSelectBot (bot));
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

			if (selectedBots.Count > 1 && isGroupingEnabled) {

				BotGroup group = null;

				foreach (Bot selectedBot in selectedBots) {
					if (selectedBot.belongsToGroup) {
						group = selectedBot.group;
						break;
					}
				}

				if (group == null) {
					group = new BotGroup (selectedBots);
					int index = availableEntities.IndexOf (selectedBots [0]);
					availableEntities [index] = group;

					foreach (Bot selectedBot in selectedBots) {
						availableEntities.Remove (selectedBot);
						BotCell cell = cellForEntity (selectedBot);
						GameObject.Destroy (cell);
					}

					availableEntities.Add (group);

					BotCell newGroupCell = BotCell.Instantiate (group);
					newGroupCell.transform.SetParent (transform, false);
					newGroupCell.GetComponent<Button> ().onClick.AddListener (() => didSelectBotGroup (group));
				}

				BotCell groupCell = cellForEntity (group);
				groupCell.SetGroupCount (group.BotCount ());
			}

			selectionDelegate.selectionControllerSelectedBot (bot);
		}

		private void removeBotFromSelection (Bot bot)
		{
			selectedBots.Remove (bot);

			if (isGroupingEnabled && bot.belongsToGroup) {
				BotGroup group = bot.group;
				if (group.BotCount () == 0) {
					availableEntities.Remove (group);
				} else {
					group.RemoveBot (bot);
				}
			}

			selectionDelegate.selectionControllerDeselectedBot (bot);
		}

		internal BotCell cellForEntity (Entity entity)
		{
			int index = availableEntities.IndexOf (entity);
			List<BotCell> cells = BotCell.AllObjects ();
			BotCell cell = cells [index];
			return cell;
		}

		// Public Conveniences

		public bool IsBotSelected (Bot bot)
		{
			return selectedBots.Contains (bot);
		}
	}
}