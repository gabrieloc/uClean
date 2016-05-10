using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SelectionController: MonoBehaviour 
{
	void Start() 
	{
		GameObject addButton = this.transform.Find("AddButton").gameObject;
		addButton.GetComponent<Button>().onClick.AddListener(() => insertNewBot());
	}

	public List<GameObject> selectedBots = new List<GameObject>();
	public GameObject avatarPrefab;
	public GameObject botPrefab;

	void avatarSelected(GameObject avatar) 
	{
		GameObject bot = botForAvatar(avatar);
		toggleBotSelection(bot, avatar);
	}

	GameObject botForAvatar(GameObject avatar) 
	{
		int index = System.Array.IndexOf(GameObjectExtensions.AvatarObjects(), avatar);
		return GameObjectExtensions.BotObjects()[index];
	}

	void toggleBotSelection(GameObject bot, GameObject avatar) 
	{
		bool wasSelected = selectedBots.Contains(bot);
		if (wasSelected) {
			selectedBots.Remove(bot);
		} else {
			selectedBots.Add(bot);
		}
		bot.SetSelected(!wasSelected);
		avatar.SetSelected(!wasSelected);
	}

	void insertNewBot() 
	{
		int oldBotCount = GameObjectExtensions.BotObjects().Length;
		Vector3 avatarTransform = new Vector3();
		float width = 50.0f; // TODO calculate based off space available
		avatarTransform.x = (width + 1.0f) * oldBotCount;
		// Debug.Log(avatarTransform);
		GameObject avatar = Instantiate(avatarPrefab, avatarTransform, new Quaternion()) as GameObject;
		avatar.SetSelected(false);
		avatar.transform.SetParent(this.transform, false);
		avatar.GetComponent<Button>().onClick.AddListener(() => avatarSelected(avatar));

		GameObject newBot = Instantiate(botPrefab, new Vector3(0, 10, 0), new Quaternion()) as GameObject;
		newBot.SetSelected(false);
	}
}


