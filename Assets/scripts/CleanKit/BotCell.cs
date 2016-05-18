using UnityEngine;
using System.Collections.Generic;

namespace CleanKit
{
	// TODO move all this shit to a category or something more sane

	public class BotCell: MonoBehaviour
	{
		public static List<BotCell> AllObjects ()
		{
			List<BotCell> objects = new List<BotCell> ();
			foreach (BotCell cell in GameObject.FindObjectsOfType<BotCell> ()) {
				objects.Add (cell.GetComponent<BotCell> ());
			}
			return objects;
		}
	}

	public class BotGroupCell: MonoBehaviour
	{
		public static List<BotGroupCell> AllObjects ()
		{
			List<BotGroupCell> objects = new List<BotGroupCell> ();
			foreach (BotGroupCell cell in GameObject.FindObjectsOfType<BotGroupCell> ()) {
				objects.Add (cell.GetComponent<BotGroupCell> ());
			}
			return objects;
		}
	}

}

