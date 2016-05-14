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
		void Awake ()
		{
			GameObject addButton = this.transform.Find ("AddButton").gameObject;
			addButton.GetComponent<Button> ().onClick.AddListener (() => insertNewBot ());
		}

		public List<Bot> allBots = new List<Bot> ();
		private List<Bot> selectedBots = new List<Bot> ();
		public GameObject avatarPrefab;
		public GameObject botPrefab;

		void avatarSelected (GameObject avatar)
		{
			Bot bot = botForAvatar (avatar);
			toggleBotSelection (bot, avatar);
		}

		Bot botForAvatar (GameObject avatar)
		{
			int index = System.Array.IndexOf (GameObjectExtensions.AvatarObjects (), avatar);
			return allBots [index];
		}

		void toggleBotSelection (Bot bot, GameObject avatar)
		{
			bool wasSelected = IsBotSelected (bot);
			if (wasSelected) {
				selectedBots.Remove (bot);
			} else {
				selectedBots.Add (bot);
			}
			bot.gameObject.SetSelected (!wasSelected);
			avatar.SetSelected (!wasSelected);
		}

		public bool IsBotSelected (Bot bot)
		{
			return selectedBots.Contains (bot);
		}

		// Remove this method once UI is rebuilt
		void insertNewBot ()
		{
			int oldBotCount = allBots.Count;

			GameObject avatarContainer = GameObjectExtensions.AvatarContainer ();
			Rect containerRect = avatarContainer.GetComponent<RectTransform> ().rect;

			GridLayoutGroup layout = avatarContainer.GetComponent<GridLayoutGroup> ();

			int columns = (int)(containerRect.width / containerRect.height);
			int rows = (oldBotCount / columns) + 1;
			float height = containerRect.height / rows;
			layout.cellSize = new Vector2 (50.0f, height);

			GameObject avatar = Instantiate (avatarPrefab, new Vector3 (), new Quaternion ()) as GameObject;
			avatar.SetSelected (false);
			avatar.transform.SetParent (GameObjectExtensions.AvatarContainer ().transform, false);
			avatar.GetComponent<Button> ().onClick.AddListener (() => avatarSelected (avatar));

			GameObject botGameObject = Instantiate (botPrefab, new Vector3 (0, 10, 0), new Quaternion ()) as GameObject;
			botGameObject.SetSelected (false);

			Bot newBot = botGameObject.GetComponent<Bot> ();

			allBots.Add (newBot);
		}
	}
}