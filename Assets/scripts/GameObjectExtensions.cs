using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public static class GameObjectExtensions 
{
	public static void SetSelected(this GameObject gameObject, bool selected)
	{
		Color color = selected ? Color.blue : Color.gray;
		Renderer renderer = gameObject.GetComponent<Renderer>();
		if (renderer) {
			renderer.material.color = color;
		} else if (gameObject.GetComponent<Image>()) {
			gameObject.GetComponent<Image>().color = color;
		}
	}

	public static GameObject AvatarContainer()
	{
		return GameObject.Find("AvatarContainer");
	}

	public static GameObject[] AvatarObjects()
	{
		return GameObject.FindGameObjectsWithTag("avatar");
	}

	public static GameObject[] BotObjects()
	{
		return GameObject.FindGameObjectsWithTag("bot");
	}
	public static GameObject[] LiftableObjects()
	{
		return GameObject.FindGameObjectsWithTag("liftable");
	}
}

