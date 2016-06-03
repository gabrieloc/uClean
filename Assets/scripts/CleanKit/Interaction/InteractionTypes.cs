using System.Collections.Generic;

namespace CleanKit
{
	public enum InteractionType
	{
		None,
		Move,
		Clean
	}

	static class InteractionTypeConveniences
	{
		public static string Description (this InteractionType type)
		{
			switch (type) {
			case InteractionType.None:
				return "";
			case InteractionType.Move:
				return "MOVE";
			case InteractionType.Clean:
				return "CLEAN";
			}
			return null;
		}
	}

	public partial class Interactable
	{
		private List<InteractionType> interactionTypes = new List<InteractionType> ();

		public void AddInteractionType (InteractionType interactionType)
		{
			interactionTypes.Add (interactionType);
		}
	}
}

