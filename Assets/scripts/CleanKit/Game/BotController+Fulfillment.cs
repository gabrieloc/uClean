using UnityEngine;
using System.Collections;

namespace CleanKit
{
	public partial class BotController : FulfillmentDelegate
	{

		public void InstructionNeedsFulfillment (InstructionController controller, Instruction instruction)
		{
			// TODO assign bots
		}

		public void InstructionCancelled (InstructionController controller, Instruction instruction)
		{
			// TODO re-allocate assigned bots
		}
	}
}
