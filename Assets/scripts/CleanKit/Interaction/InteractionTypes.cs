using System.Collections.Generic;

namespace CleanKit
{
	public class InteractionType
	{
		public string identifier { get; private set; }

		public static InteractionType Liftable ()
		{
			return new InteractionType ("lift");
		}

		public static InteractionType Cleanable ()
		{
			return new InteractionType ("clean");
		}

		private InteractionType (string identifier)
		{
			this.identifier = identifier;
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

