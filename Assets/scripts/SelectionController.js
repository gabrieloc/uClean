#pragma strict
import UnityEngine.UI;
import System.Collections.Generic;

function avatarSelected(avatar: GameObject) {
	var bot = botForAvatar(avatar);
	toggleBotSelection(bot);
}

function addSelected() {
	insertNewBot();
}

// Avatars

public var avatarPrefab: GameObject;
public var botPrefab: GameObject;

private function insertNewBot() {
	var newBot = Instantiate(botPrefab, Vector3(0, 10, 0), Quaternion());

	var avatar = Instantiate(avatarPrefab, Vector3(), Quaternion());
	avatar.transform.SetParent(this.transform, false);
	var buttonComponent = avatar.GetComponent.<Button>();
	buttonComponent.onClick.AddListener(function() {
		avatarSelected(avatar);
	});
}

private function avatars() {
	return GameObject.FindGameObjectsWithTag("avatar");
};

// Bots

var selectedBots: List.<GameObject> = new List.<GameObject>();

function toggleBotSelection(bot: GameObject) {
	var isSelected = selectedBots.Contains(bot);
	if (isSelected) {
		selectedBots.Remove(bot);
	} 
	else {
		selectedBots.Add(bot);
	}
	bot.GetComponent.<Renderer>().material.color = colorForSelectionState(isSelected);
}

private function bots() {
	return GameObject.FindGameObjectsWithTag("bot");
};


private function botForAvatar(avatar: GameObject) {
	var index = System.Array.IndexOf(avatars(), avatar);
	return bots()[index];
}

// Conveniences

private function colorForSelectionState(selected): Color {
	return selected ? Color.blue : Color.gray;
}
