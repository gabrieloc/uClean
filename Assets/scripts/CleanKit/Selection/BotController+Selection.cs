using UnityEngine;

namespace CleanKit
{
	public partial class BotController: SelectionDelegate
	{
		private SelectionController selectionController;

		public void selectionControllerSelectedBot (Bot bot)
		{
			Debug.Log ("Selected " + bot.name);
			bot.SetSelected (true);
		}

		public void selectionControllerDeselectedBot (Bot bot)
		{
			Debug.Log ("Deselected " + bot.name);
			bot.SetSelected (false);
			storedContactPoint = Vector3.zero;
			clearInteractable (bot);
		}
	}
}

