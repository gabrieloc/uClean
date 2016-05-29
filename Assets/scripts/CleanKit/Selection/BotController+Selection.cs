using UnityEngine;

namespace CleanKit
{
	public partial class BotController: SelectionDelegate
	{
		private SelectionController selectionController;

		public void selectionControllerSelectedBot (Bot bot)
		{
			bot.gameObject.SetSelected (true);
		}

		public void selectionControllerDeselectedBot (Bot bot)
		{
			bot.gameObject.SetSelected (false);
			clearContactPoint ();
			clearInteractableForBot (bot);
		}
	}
}

