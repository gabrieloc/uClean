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
	public interface SelectionDelegate
	{
		void selectionControllerSelectedBot (Bot bot);

		void selectionControllerDeselectedBot (Bot bot);
	}

	public class SelectionController: MonoBehaviour
	{
		internal List<Bot> allBots = new List<Bot> ();
		internal List<Swarm> allSwarms = new List<Swarm> ();
		//		public List<Bot> selectedBots = new List<Bot> ();

		private bool isSwarmingToggled;

		private Swarm selectedSwarm {
			get { return currentInteractor as Swarm; }
		}

		private Bot selectedBot {
			get { return currentInteractor as Bot; }
		}

		public Interactor currentInteractor { get; private set; }

		public SelectionDelegate selectionDelegate;

		private Button swarmButton;

		void Awake ()
		{
			swarmButton = GameObject.Find ("SwarmButton").GetComponent<Button> ();
			swarmButton.GetComponent<Button> ().onClick.AddListener (() => toggleSwarming ());
		}

		void Update ()
		{
			if (selectedSwarm != null) {
				List<Bot> bots = selectedSwarm.bots;
				foreach (Bot bot in bots) {
					int index = bots.IndexOf (bot);
					Vector3 p1 = bot.transform.position;
					Vector3 p2;

					if (index == (bots.Count - 1)) {
						p2 = bots [0].transform.position;
					} else {
						p2 = bots [index + 1].transform.position;
					}
					Debug.DrawLine (p1, p2, Color.green);
				}
			}
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
			bool selected = selectedSwarm != swarm;

			clearSelection ();

			if (selected) {
				foreach (Bot bot in swarm.bots) {
					addBotToSelection (bot);
				}
				currentInteractor = swarm;
			}

			swarm.cell.gameObject.SetSelected (selected);
		}

		private void clearSelection ()
		{
			if (selectedSwarm != null) {
				Swarm swarm = currentInteractor as Swarm;
				swarm.cell.gameObject.SetSelected (false);
				foreach (Bot bot in swarm.bots) {
					selectionDelegate.selectionControllerDeselectedBot (bot);
				}
			} else if (selectedBot != null) {
				Bot bot = currentInteractor as Bot;
				if (bot.cell) {
					bot.cell.gameObject.SetSelected (false);
				}
				selectionDelegate.selectionControllerDeselectedBot (bot);
			}
			currentInteractor = null;
		}

		private void selectBot (Bot bot)
		{
			clearSelection ();
			currentInteractor = bot;
			addBotToSelection (bot);
		}

		private void addBotToSelection (Bot bot)
		{
			if (bot.cell) {
				bot.cell.gameObject.SetSelected (true);
			}
			selectionDelegate.selectionControllerSelectedBot (bot);
		}

		private void addBotToCurrentSwarm (Bot bot)
		{
			if (selectedSwarm == null) {
				Swarm swarm = new Swarm ();

				SwarmCell cell = swarm.cell;
				cell.transform.SetParent (transform, false);
				cell.GetComponent<Button> ().onClick.AddListener (() => didSelectCellForSwarm (swarm));
				cell.gameObject.SetSelected (true);

				currentInteractor = swarm;
			}

			selectedSwarm.AddBot (bot);
			selectionDelegate.selectionControllerSelectedBot (bot);
			Debug.Log ("Selected " + bot.name);
		}

		// Public Conveniences
	}
}