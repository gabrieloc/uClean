using UnityEngine;

namespace CleanKit
{
	public partial class BotController: SelectionDelegate
	{
		private SelectionController selectionController;

		public void selectionControllerSelectedBot (Bot bot)
		{
			Debug.Log ("Selected " + bot.name);
			bot.gameObject.SetSelected (true);
		}

		public void selectionControllerDeselectedBot (Bot bot)
		{
			Debug.Log ("Deselected " + bot.name);
			bot.gameObject.SetSelected (false);
			clearContactPoint ();
			clearInteractable (bot);
		}
	}
}

