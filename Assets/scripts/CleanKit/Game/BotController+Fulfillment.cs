using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CleanKit
{
	public partial class BotController : FulfillmentDelegate
	{
		List<Actor> unemployedActors { get { return actors.FindAll (a => !a.IsEmployed ()); } }

		List<Actor> divisibleActors { get { return actors.FindAll (a => a.IsDivisible ()); } }

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
			List<Instruction> jobs = instructionController.AvailableInstructions ();

			if (job != null) {
				actor.Employ (job);
			} else if (jobs != null && jobs.Count > 0) {
				actor.Employ (jobs [0]);
			}
		}

		Actor createEmployableActor ()
		{
			// First, look for unemployed actors
			if (unemployedActors.Count > 0) {
				return unemployedActors [0];
			}

			// Next, look for reassignable actors
			if (divisibleActors.Count > 0) {
				Actor divisible = divisibleActors [0];
				Actor newActor = divisible.Bisect ();
				return newActor;
			}

			// If neither unemployed or reassignable actors exist, return nothing
			return null;
		}
	}
}
