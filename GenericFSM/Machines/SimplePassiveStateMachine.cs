using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericFSM.Machines
{
	public class SimplePassiveStateMachine<TState, TCommand> : StateMachine<TState, TCommand>
		where TState : struct
		where TCommand : struct
	{
		public override void Start() {
			
		}
	}
}
