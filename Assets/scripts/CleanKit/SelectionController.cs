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
		internal List<Bot> allBots = new List<Bot> ();
		internal List<Swarm> allSwarms = new List<Swarm> ();
		//		public List<Entity> availableEntities = new List<Entity> ();
		public List<Bot> selectedBots = new List<Bot> ();

		private bool isSwarmingToggled;
		private Swarm currentSwarm = null;

		// TODO use proper delegation syntax
		public BotController selectionDelegate;

		private Button swarmButton;

		void Awake ()
		{
			swarmButton = GameObject.Find ("SwarmButton").GetComponent<Button> ();
			swarmButton.GetComponent<Button> ().onClick.AddListener (() => toggleSwarming ());
		}

		private void toggleSwarming ()
		{
			isSwarmingToggled = !isSwarmingToggled;
			swarmButton.GetComponent<Image> ().color = isSwarmingToggled ? Color.red : Color.black;

			clearSelection ();
		}

		public void DidInsertBot (Bot bot)
		{
			allBots.Add (bot);

			BotCell cell = bot.cell;
			cell.transform.SetParent (transform, false);
			cell.GetComponent<Button> ().onClick.AddListener (() => didSelectCellForBot (bot));
		}

		private void didSelectCellForBot (Bot bot)
		{
			if (isSwarmingToggled) {
				addBotToCurrentSwarm (bot);
			} else {
				selectBot (bot);
			}
		}

		private void didSelectCellForSwarm (Swarm swarm)
		{
			foreach (Bot bot in swarm.bots) {
				if (isSwarmingToggled) {
					addBotToSelection (bot);
				} else {
					removeBotFromSelection (bot);
				}
			}
			swarm.cell.gameObject.SetSelected (isSwarmingToggled);
		}

		private void clearSelection ()
		{
			List<Swarm> selectedSwarms = new List<Swarm> ();
			while (selectedBots.Count > 0) {
				Bot bot = selectedBots [0];
				removeBotFromSelection (bot);
				if (selectedSwarms.Contains (bot.swarm) == false) {
					selectedSwarms.Add (bot.swarm);
				}
			}
			while (selectedSwarms.Count > 0) {
				SwarmCell cell = selectedSwarms [0].cell;
				cell.gameObject.SetSelected (false);
			}
		}

		private void selectBot (Bot bot)
		{
			if (IsBotSelected (bot)) {
				removeBotFromSelection (bot);
			} else {
				clearSelection ();
				addBotToSelection (bot);
			}
		}

		private void addBotToSelection (Bot bot)
		{
			selectedBots.Add (bot);
			BotCell cell = bot.cell;
			cell.gameObject.SetSelected (true);
			selectionDelegate.selectionControllerSelectedBot (bot);

			Debug.Log ("Selected " + bot.name);
		}

		private void removeBotFromSelection (Bot bot)
		{
			selectedBots.Remove (bot);
			BotCell cell = bot.cell;
			cell.gameObject.SetSelected (false);
			selectionDelegate.selectionControllerDeselectedBot (bot);

			Debug.Log ("Deselected " + bot.name);
		}

		private void addBotToCurrentSwarm (Bot bot)
		{
			if (currentSwarm == null) {
				currentSwarm = new Swarm ();

				SwarmCell cell = currentSwarm.cell;
				cell.transform.SetParent (transform, false);
				cell.GetComponent<Button> ().onClick.AddListener (() => didSelectCellForSwarm (currentSwarm));
				cell.gameObject.SetSelected (true);
			}

			currentSwarm.AddBot (bot);
		}

		// Public Conveniences

		public bool IsBotSelected (Bot bot)
		{
			return selectedBots.Contains (bot);
		}
	}
}