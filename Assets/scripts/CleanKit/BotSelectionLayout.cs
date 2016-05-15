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

			// TODO figure out how to resize parent

//			float intrinsicHeight = Insets.x * 2.0f;
//			float availableWidth = rect.width - Insets.y * 2.0f;

			// Bot Group Cells
			List<BotGroupCell> botGroupCells = BotGroupCell.AllObjects ();
			foreach (BotGroupCell cell in botGroupCells) {
				// TODO
			}

			// Available Bot Cells
			List<BotCell> botCells = BotCell.AllObjects ();
			foreach (BotCell cell in botCells) {
				
			}
//
//			float gutter = useSmallSize ? 4.0f : 2.0f;
//			layout.spacing = new Vector2 (gutter, gutter);
//
//			float cellLength = (h * 0.5f) - ((rows - 1) * gutter);
//			layout.cellSize = new Vector2 (cellLength, cellLength);
		}
	}
}

