using UnityEngine;
using System.Collections.Generic;

namespace CleanKit
{
	public interface Actor
	{
		Vector3 PrimaryContactPoint ();

		void RelocateToDestination (Destination destination);

		float DistanceFromDestination ();

		void SetSelected (bool selected);

		string Name ();

		bool IsEmployed ();

		void Employ (Instruction instruction);

		// Indicates actor can be divided into at least two actors
		bool IsDivisible ();

		// Cuts actor in half, returning new partition
		Actor Bisect ();
	}

	public interface ActorDelegate
	{
		void ActorFulfilledInstruction (Actor actor, Instruction instruction);
	}
}