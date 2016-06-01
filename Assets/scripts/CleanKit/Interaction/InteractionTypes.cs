﻿using System.Collections.Generic;

namespace CleanKit
{
	public enum InteractionType
	{
		Lift,
		Clean
	}

	static class InteractionTypeConveniences
	{
		public static string Description (this InteractionType type)
		{
			switch (type) {
			case InteractionType.Lift:
				return "lift";
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
