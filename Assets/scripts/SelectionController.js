#pragma strict
import UnityEngine.UI;
import System.Collections.Generic;

var selectedBots: List.<GameObject> = new List.<GameObject>();

function avatarSelected(avatar: GameObject) {
	var bot = botForAvatar(avatar);
	toggleBotSelection(bot, avatar);
}

function addSelected() {
	insertNewBot();
}

// Avatars
public var avatarPrefab: GameObject;
public var botPrefab: GameObject;

private function insertNewBot() {
	var oldBotCount = bots().length;

	var avatarTransform = Vector3();
	var width = 50.0; // TODO calculate based off space available
	avatarTransform.x = (width + 1.0) * oldBotCount;
	// Debug.Log(avatarTransform);
	var avatar = Instantiate(avatarPrefab, avatarTransform, Quaternion());
	avatar.GetComponent.<Image>().color = colorForSelectionState(false);
	avatar.transform.SetParent(this.transform, false);
	avatar.GetComponent.<Button>().onClick.AddListener(function() {
		avatarSelected(avatar);
	});

	var newBot = Instantiate(botPrefab, Vector3(0, 10, 0), Quaternion());
	newBot.GetComponent.<Renderer>().material.color = colorForSelectionState(false);
}

private function avatars() {
	return GameObject.FindGameObjectsWithTag("avatar");
};

// Bots
function toggleBotSelection(bot: GameObject, avatar: GameObject) {
	var isSelected = selectedBots.Contains(bot);
	if (isSelected) {
		selectedBots.Remove(bot);
	} 
	else {
		selectedBots.Add(bot);
	}
	bot.GetComponent.<Renderer>().material.color = colorForSelectionState(!isSelected);
	avatar.GetComponent.<Image>().color = colorForSelectionState(!isSelected);
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
