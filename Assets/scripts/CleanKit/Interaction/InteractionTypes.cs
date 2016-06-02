using System.Collections.Generic;

namespace CleanKit
{
	public enum InteractionType
	{
		Lift,
		Push
	}

	static class InteractionTypeConveniences
	{
		public static string Description (this InteractionType type)
		{
			switch (type) {
			case InteractionType.Lift:
				return "lift";
			case InteractionType.Push:
				return "push";
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

