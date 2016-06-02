using System.Collections.Generic;

namespace CleanKit
{
	public enum InteractionType
	{
		Move,
		Clean
	}

	static class InteractionTypeConveniences
	{
		public static string Description (this InteractionType type)
		{
			switch (type) {
			case InteractionType.Move:
				return "move";
			case InteractionType.Clean:
				return "clean";
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

