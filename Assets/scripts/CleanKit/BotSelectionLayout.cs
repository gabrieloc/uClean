using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace CleanKit
{
	// TODO make inherit from LayoutGroup
	public class BotSelectionLayout : MonoBehaviour
	{
		// TODO use proper delegate
		public SelectionController layoutDelegate;

		public Vector2 Insets = new Vector2 (4.0f, 4.0f);
		public float Gutter = 2.0f;

		public void UpdateLayout ()
		{
			Rect rect = layoutDelegate.GetComponent<RectTransform> ().rect;
			float containerHeight = rect.height;

			List<Bot> bots = layoutDelegate.allBots;
			List<BotGroup> groups = layoutDelegate.botGroups;
			int ungroupedIndex = 0;

			foreach (Bot bot in bots) {
				if (bot.belongsToGroup) {
					
				} else {
					BotCell cell = layoutDelegate.cellForBot (bot);
					RectTransform rectTransform = cell.GetComponent<RectTransform> ();

					float length = containerHeight;

					rectTransform.offsetMin = new Vector2 (length * ungroupedIndex, 0.0f);
					rectTransform.offsetMax = new Vector2 (length * (ungroupedIndex + 1), 0.0f);

					ungroupedIndex++;
				}
			}

		}
	}
}

