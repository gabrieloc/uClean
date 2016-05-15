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
	public class BotCell: MonoBehaviour
	{
		public void SetSelected (bool selected)
		{
			// TODO
		}
	}

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

			Rect rect = GetComponent<RectTransform> ().rect;
			GridLayoutGroup layout = GetComponent<GridLayoutGroup> ();

			float h = rect.height - layout.padding.top - layout.padding.bottom;
			float w = rect.width - layout.padding.left - layout.padding.right;

			int count = allBots.Count;
			bool useSmallSize = (count * h) > w;
			int rows = useSmallSize ? 2 : 1;
			layout.constraintCount = rows;

			float gutter = useSmallSize ? 4.0f : 2.0f;
			layout.spacing = new Vector2 (gutter, gutter);
				
			float cellLength = (h * 0.5f) - ((rows - 1) * gutter);
			layout.cellSize = new Vector2 (cellLength, cellLength);

			GameObject cell = Instantiate (Resources.Load ("BotCell"), new Vector3 (), new Quaternion ()) as GameObject;
			cell.SetSelected (false);
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
			bot.gameObject.SetSelected (!wasSelected);

			BotCell cell = cellForBot (bot);
			cell.SetSelected (!wasSelected);

			layoutCells ();
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
			GameObject gameObject = GameObjectExtensions.BotCellObjects () [index];
			return gameObject.GetComponent<BotCell> ();
		}

		private void layoutCells ()
		{
			
		}

		// Public Conveniences

		public bool IsBotSelected (Bot bot)
		{
			return selectedBots.Contains (bot);
		}
	}
}