using UnityEngine;
using System.Collections;

namespace CleanKit
{
	public class Instruction
	{
		public InteractionType interactionType;
		public Interactable interactable;
		public Destination destination;
		public Actor actor;

		public string name { get { return interactionType + " " + interactable.name; } }
	}
}
