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
			bool selected = currentSwarm != swarm;

			clearSelection ();

			if (selected) {
				foreach (Bot bot in swarm.bots) {
					addBotToSelection (bot);
				}
				currentSwarm = swarm;
			}

			swarm.cell.gameObject.SetSelected (selected);
		}

		private void clearSelection ()
		{
			if (currentSwarm != null) {
				SwarmCell cell = currentSwarm.cell;
				cell.gameObject.SetSelected (false);
				currentSwarm = null;
			}

			while (selectedBots.Count > 0) {
				Bot bot = selectedBots [0];
				removeBotFromSelection (bot);
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
			if (bot.cell) {
				bot.cell.gameObject.SetSelected (true);
			}
			selectionDelegate.selectionControllerSelectedBot (bot);

			Debug.Log ("Selected " + bot.name);
		}

		private void removeBotFromSelection (Bot bot)
		{
			selectedBots.Remove (bot);
			if (bot.cell) {
				bot.cell.gameObject.SetSelected (false);
			}
			selectionDelegate.selectionControllerDeselectedBot (bot);

			Debug.Log ("Deselected " + bot.name);
		}

		private void addBotToCurrentSwarm (Bot bot)
		{
			if (currentSwarm == null) {
				Swarm swarm = new Swarm ();

				SwarmCell cell = swarm.cell;
				cell.transform.SetParent (transform, false);
				cell.GetComponent<Button> ().onClick.AddListener (() => didSelectCellForSwarm (swarm));
				cell.gameObject.SetSelected (true);

				currentSwarm = swarm;
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