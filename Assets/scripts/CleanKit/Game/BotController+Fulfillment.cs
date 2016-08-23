using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CleanKit
{
	public partial class BotController : FulfillmentDelegate
	{
		Actor[] unemployedActors { get { return Array.FindAll (actors, actorIsEmployable); } }

		Actor[] divisibleActors { get { return Array.FindAll (actors, actorIsDivisible); } }

		public void InstructionNeedsFulfillment (InstructionController controller, Instruction instruction)
		{
			Actor employableActor = createEmployableActor ();
			if (employableActor != null) {
				employActor (employableActor, instruction);
			} else {
				print ("No available actors. Instruction prioritized in queue.");
			}
		}

		public void InstructionFulfilled (InstructionController controller, Instruction instruction)
		{
			Actor unemployedActor = instruction.actor;
			employActor (unemployedActor);
		}

		public void InstructionCancelled (InstructionController controller, Instruction instruction)
		{
			Actor unemployedActor = instruction.actor;
			employActor (unemployedActor);
		}

		void employActor (Actor actor, Instruction job = null)
		{
			Instruction[] jobs = instructionController.AvailableInstructions ();

			if (job != null) {
				actor.Employ (job);
			} else if (jobs.Length > 0) {
				actor.Employ (jobs [0]);
			}
		}

		Actor createEmployableActor ()
		{
			// First, look for unemployed actors
			if (unemployedActors.Length > 0) {
				return unemployedActors [0];
			}

			// Next, look for reassignable actors
			if (divisibleActors.Length > 0) {
				Actor divisible = divisibleActors [0];
				Actor newActor = divisible.Bisect ();
				return newActor;
			}

			// If neither unemployed or reassignable actors exist, return nothing
			return null;
		}

		static bool actorIsEmployable (Actor actor)
		{
			return actor.IsEmployed () == false;
		}

		static bool actorIsDivisible (Actor actor)
		{
			return actor.IsDivisible ();
		}
	}
}
