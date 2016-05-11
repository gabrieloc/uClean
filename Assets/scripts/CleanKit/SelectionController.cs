using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace CleanKit
{
	public class SelectionController: MonoBehaviour
	{
		void Start ()
		{
			GameObject addButton = this.transform.Find ("AddButton").gameObject;
			addButton.GetComponent<Button> ().onClick.AddListener (() => insertNewBot ());
		}

		public List<GameObject> selectedBots = new List<GameObject> ();
		public Dictionary<GameObject, GameObject> availableLiftables = new Dictionary<GameObject, GameObject> ();

		public GameObject avatarPrefab;
		public GameObject botPrefab;

		void avatarSelected (GameObject avatar)
		{
			GameObject bot = botForAvatar (avatar);
			toggleBotSelection (bot, avatar);
		}

		GameObject botForAvatar (GameObject avatar)
		{
			int index = System.Array.IndexOf (GameObjectExtensions.AvatarObjects (), avatar);
			return GameObjectExtensions.BotObjects () [index];
		}

		void toggleBotSelection (GameObject bot, GameObject avatar)
		{
			bool wasSelected = selectedBots.Contains (bot);
			if (wasSelected) {
				selectedBots.Remove (bot);
			} else {
				selectedBots.Add (bot);
			}
			bot.SetSelected (!wasSelected);
			avatar.SetSelected (!wasSelected);
		}

		void insertNewBot ()
		{
			int oldBotCount = GameObjectExtensions.BotObjects ().Length;

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

			GameObject newBot = Instantiate (botPrefab, new Vector3 (0, 10, 0), new Quaternion ()) as GameObject;
			newBot.SetSelected (false);
		}

		public void SetLiftableForBot (GameObject liftable, GameObject bot)
		{
			availableLiftables [bot] = liftable;
		}

		public void ClearLiftableForBot (GameObject bot)
		{
			availableLiftables.Remove (bot);
		}
	}
}