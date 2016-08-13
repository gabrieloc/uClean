using UnityEngine;
using System.Collections;

namespace CleanKit
{
	public class Instruction
	{
		public InteractionType interactionType;
		public Interactable assignee;
		public Destination destination;

		public string name { get { return interactionType + " " + assignee.name; } }
	}
}
